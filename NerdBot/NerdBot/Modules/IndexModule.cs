using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Routing;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.Reporters;
using NerdBotCommon.Web.Queries;
using SimpleLogging.Core;

namespace NerdBotCommon.Modules
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        public IndexModule(
            BotConfig botConfig,
            IMtgStore mtgStore,
            IMessenger messenger,
            IPluginManager pluginManager,
            ICommandParser commandParser,
            ILoggingService loggingService,
            IReporter reporter,
            ICardPriceStore priceStore)
        {
            Get["/"] = parameters =>
            {
                loggingService.Warning("GET request from {0}: Path '{1}' was invalid.",
                        this.Request.UserHostAddress,
                        this.Request.Path);

                return View["index/index.html"];
            };

            #region Card Search Route
            Post["/api/search/{term}", true] = async (parameters, ct) =>
            {
                int limit = 200;

                string term = parameters.term;

                if (string.IsNullOrEmpty(term))
                {
                    return HttpStatusCode.NotAcceptable;
                }

                var cards = await mtgStore.SearchCards(term, limit);

                if (cards == null)
                {
                    string msg = string.Format("No cards found using name '{0}'", term);

                    loggingService.Error(msg);

                    return Response.AsJson(new
                    {
                        SearchTerm = term,
                        Limit = limit,
                        Cards = new List<Card>()
                    }).StatusCode = HttpStatusCode.NotAcceptable;
                }

                return Response.AsJson(new
                {
                    SearchTerm = term,
                    Limit = limit,
                    Cards = cards.Select(c => new
                    {
                        Name = c.Name,
                        Code = c.SetId,
                        Set = c.SetName,
                        Cost = c.Cost,
                        Type = c.FullType,
                        Rarity = c.Rarity,
                        Img = c.Img,
                        MultiverseId = c.MultiverseId,
                        SearchName = c.SearchName,
                        Symbol = c.SetAsKeyRuneIcon,
                        Desc = c.Desc,
                        CMC = c.Cmc
                    }).OrderByDescending(c => c.SearchName)
                });
            };

            //
            // Testing out paging
            Post["/api/0.x/search", true] = async (parameters, ct) =>
            {
                int limit = 1000;

                //string term = parameters.term;

                var query = this.Bind<SearchQuery>();

                if (query == null)
                    return HttpStatusCode.NotAcceptable;

                if (string.IsNullOrEmpty(query.SearchTerm))
                {
                    return HttpStatusCode.NotAcceptable;
                }

                var cards = await mtgStore.SearchCards(query.SearchTerm, query.Page, limit);

                if (cards == null)
                {
                    string msg = string.Format("No cards found using name '{0}'", query.SearchTerm);

                    loggingService.Error(msg);

                    return Response.AsJson(new
                    {
                        SearchTerm = query.SearchTerm,
                        Limit = limit,
                        Cards = new List<Card>()
                    }).StatusCode = HttpStatusCode.NotAcceptable;
                }

                return Response.AsJson(new
                {
                    SearchTerm = query.SearchTerm,
                    Limit = limit,
                    Cards = cards.Select(c => new
                    {
                        Name = c.Name,
                        Code = c.SetId,
                        Set = c.SetName,
                        Cost = c.Cost,
                        Type = c.FullType,
                        Rarity = c.Rarity,
                        Img = c.Img,
                        MultiverseId = c.MultiverseId,
                        SearchName = c.SearchName,
                        Symbol = c.SetAsKeyRuneIcon,
                        Desc = c.Desc
                    }).OrderByDescending(c => c.SearchName)
                });
            };
            #endregion

            // priceincreases route
            Get["/priceincreases/"] = parameters =>
            {
                int limit = 10;

                List<CardPrice> prices = priceStore.GetCardsByPriceIncrease(limit);

                return Response.AsJson<List<CardPrice>>(prices);
            };

            // pricedecreases route
            Get["/pricedecreases/"] = parameters =>
            {
                int limit = 10;

                List<CardPrice> prices = priceStore.GetCardsByPriceDecrease(limit);

                return Response.AsJson<List<CardPrice>>(prices);
            };

            // Ruling route
            Get["/ruling/{id:int}", true] = async (parameters, ct) =>
            {
                int cardMultiverseId = parameters.id;

                var card = await mtgStore.GetCard(cardMultiverseId);
                
                if (card == null)
                {
                    string msg = string.Format("No card found using multiverseId '{0}'", cardMultiverseId);

                    loggingService.Error(msg);

                    return msg;
                }

                var set = await mtgStore.GetSetByCode(card.SetId);

                if (set == null)
                {
                    string msg = string.Format("No set found using code '{0}'", card.SetId);

                    loggingService.Error(msg);

                    return msg;
                }

                return View["index/ruling.sshtml", new
                {
                    Card = card,
                    SetCode = !string.IsNullOrEmpty(set.GathererCode) ? set.GathererCode : set.Code
                }];
            };

            // Get search results
            Get["/search/{name}", true] = async (parameters, ct) =>
            {
                int limit = 100;

                string name = parameters.name;

                if (string.IsNullOrEmpty(name))
                {
                    return HttpStatusCode.Accepted;
                }

                var cards = await mtgStore.SearchCards(name, limit);

                if (cards == null)
                {
                    string msg = string.Format("No cards found using name '{0}'", name);

                    loggingService.Error(msg);

                    return msg;
                }

                return View["index/search.sshtml", new
                {
                    SearchTerm = name, 
                    Cards = cards
                }];
            };

            #region Bot Route
            Post["/bot/{token}", true] = async (parameters, ct) =>
            {
                try
                {
                    // Get the request's body as a string, for logging
                    string request_string = this.Request.Body.AsString();

                    string sentToken = parameters.token;

                    // If the passed token segment does not match the secret token, return NotAcceptable status
                    if (sentToken != botConfig.SecretToken)
                    {
                        string errMsg = string.Format("POST request from {0}: Token '{1}' was invalid.\nREQUEST = {2}",
                            this.Request.UserHostAddress,
                            sentToken,
                            request_string);

                        loggingService.Warning(errMsg);
                        reporter.Warning(errMsg);

                        return HttpStatusCode.NotAcceptable;
                    }

                    var message = new GroupMeMessage();

                    // Bind and validate the request to GroupMeMessage
                    var msg = this.BindToAndValidate(message);

                    if (!ModelValidationResult.IsValid)
                    {
                        string errMsg = string.Format("POST request from {0}: Message was invalid.\nREQUEST = {1}",
                            this.Request.UserHostAddress,
                            request_string);

                        loggingService.Warning(errMsg);
                        reporter.Warning(errMsg);

                        return HttpStatusCode.NotAcceptable;
                    }

                    // Don't handle messages sent from ourself
                    if (message.name.ToLower() == messenger.BotName.ToLower())
                        return HttpStatusCode.NotAcceptable;

                    if (string.IsNullOrEmpty(message.text))
                    {
                        loggingService.Debug("POST request from {0}: Message text is empty or null.\nREQUEST = {1}",
                            this.Request.UserHostAddress,
                            request_string);

                        return HttpStatusCode.NotAcceptable;
                    }

                    loggingService.Trace("MSG: From: {0} [UID: {1}]; Body: {2}", 
                        message.name,
                        message.user_id,
                        message.text);

                    // Parse the command
                    var command = commandParser.Parse(message.text);
                    if (command != null)
                    {
                        if (!string.IsNullOrEmpty(command.Cmd))
                        {
                            loggingService.Trace("Received command: {0}", command.Cmd);

                            if (command.Cmd.ToLower() == "help")
                            {
                                bool helpHandled = await pluginManager.HandledHelpCommand(command, messenger);
                            }
                            else
                            {
                                // If a message is in a command format '<cmd>\s[message]', 
                                //  have the plugin manager see if any loaded plugins are set to respond to that command
                                bool handled = await pluginManager.HandleCommand(command, message, messenger);

                                if (!handled)
                                    pluginManager.SendMessage(message, messenger);
                            }
                        }
                    }

                    return HttpStatusCode.Accepted;
                }
                catch (Exception er)
                {
                    reporter.Error("MAIN ERROR", er);
                    loggingService.Error(er, string.Format("** MAIN ERROR: {0}", er.Message));

                    return HttpStatusCode.BadGateway;
                }
            };
            #endregion
        }
    }
}