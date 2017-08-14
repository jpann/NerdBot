using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Routing;
using Nancy.Security;
using NerdBotCommon.Admin;
using NerdBotCommon.Http;
using NerdBotCommon.Importer;
using NerdBotCommon.Importer.DataReaders;
using NerdBotCommon.Importer.Mapper;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using SimpleLogging.Core;

namespace NerdBotCommon.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule(
            IMtgStore mtgStore,
            ILoggingService loggingService,
            ICardPriceStore priceStore,
            IHttpClient httpClient,
            IMtgDataReader mtgDataReader,
            IImporter importer)
            : base("/admin")
        {
            Get["/"] = parameters =>
            {
                if (this.Context.CurrentUser == null)
                {
                    return this.Context.GetRedirect("/admin/login");
                }
                else
                {
                    return View["admin.html"];
                }
            };

            Get["/login"] = parameters =>
            {
                return View["login.html"];
            };


            Get["/logout"] = parameters =>
            {
                return this.LogoutAndRedirect("/admin/login/");
            };


            Post["/login"] = args =>
            {
                var userGuid = UserMapper.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);

                if (userGuid == null)
                {
                    return this.Context.GetRedirect("/admin/login");
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry, "/admin");
            };

            #region Sets
            Get["/sets", true] = async (parameters, ct) =>
            {
                this.RequiresAuthentication();

                var sets = await mtgStore.GetSets();

                return View["listSets.html", new
                {
                    Sets = sets.Select(s => new
                    {
                        Name = s.Name,
                        Code = s.Code,
                        Block = s.Block,
                        Type = s.Type,
                        ReleasedOn = s.ReleasedOn.ToShortDateString(),
                        ReleasedOnSort = s.ReleasedOn,
                        Symbol = s.SetAsKeyRuneIcon
                    }).OrderByDescending(s => s.ReleasedOnSort)
                }];

            };

            Get["/set/{set}", true] = async (parameters, ct) =>
            {
                this.RequiresAuthentication();

                string setCode = parameters.set;

                var set = await mtgStore.GetSetByCode(setCode);
                var cards = await mtgStore.GetCardsBySet(set.Name);

                return View["set.html", new
                {
                    Set = set,
                    Cards = cards.OrderBy(c => c.MultiverseId)
                }];

            };
            #endregion

            #region Add Set
            Get["/addset"] = parameters =>
            {
                this.RequiresAuthentication();

                return View["addSet.html"];
            };

            Post["/addset", true] = async (parameters, ct) =>
            {
                this.RequiresAuthentication();

                var setData = this.Bind<SetAddData>();

                if (string.IsNullOrEmpty(setData.url))
                {
                    return Response.AsJson(new
                    {
                        Error = "No url provided.",
                        Status = "Error"
                    }).StatusCode = HttpStatusCode.BadRequest;
                }

                string data = await httpClient.GetAsJson(setData.url);

                if (string.IsNullOrEmpty(data))
                {
                    return Response.AsJson(new
                    {
                        Error = "Url contains invalid data or no data.",
                        Status = "Error"
                    }).StatusCode = HttpStatusCode.BadRequest;
                }

                var set = mtgDataReader.ReadSet(data);
                var cards = mtgDataReader.ReadCards(data);

                if (set == null)
                {
                    return Response.AsJson(new
                    {
                        Error = "No set defined",
                        Status = "Error"
                    }).StatusCode = HttpStatusCode.BadRequest;
                }

                if (set.Type.ToLower() == "promo")
                {
                    return Response.AsJson(new
                    {
                        Error = "Set is promo, skipping",
                        Status = "Error"
                    }).StatusCode = HttpStatusCode.BadRequest;
                }

                var status = await importer.Import(set, cards);

                if (status != null)
                {
                    return Response.AsJson(new
                    {
                        Status = "Success",
                        CardsInserted = status.ImportedCards.Select(
                            c =>
                                new
                                {
                                    Name = c.Name,
                                    MultiverseId = c.MultiverseId,
                                    Cost = c.Cost,
                                    Type = c.FullType,
                                    Img = c.Img,
                                    Set = c.SetId
                                }).ToList(),
                        CardsFailed = status.FailedCards.Select(
                            c =>
                                new
                                {
                                    Name = c.Name,
                                    MultiverseId = c.MultiverseId,
                                    Cost = c.Cost,
                                    Type = c.FullType,
                                    Img = c.Img,
                                    Set = c.SetId
                                }).ToList()
                    });
                }
                else
                {
                    return Response.AsText("Import failed").StatusCode = HttpStatusCode.BadRequest;
                }
                
            };

            #endregion
        }
    }
}