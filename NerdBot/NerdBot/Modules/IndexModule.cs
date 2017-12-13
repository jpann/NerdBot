using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses;
using NerdBot.Parsers;
using NerdBot.Reporters;
using NerdBot.Web.Queries;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using SimpleLogging.Core;

namespace NerdBot.Modules
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
            Get["/api/search/{term?}", true] = async (parameters, ct) =>
            {
                var sw = Stopwatch.StartNew();

                int limit = 200;

                string term = parameters.term;

                if (string.IsNullOrEmpty(term) || term.StartsWith("?"))
                {
                    return Response.AsJson(new
                    {
                        SearchTerm = "",
                        Limit = limit,
                        Cards = new List<Card>()
                    });
                }

                var db_cards = await mtgStore.FullTextSearch(term, limit);

                if (db_cards == null)
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

                // Get price information
                var cards = db_cards.Select(c => new
                {
                    Name = c.Name,
                    Code = c.SetId,
                    Set = c.SetName,
                    Cost = c.Cost,
                    CostSymbols = c.CostWithSymbols,
                    Type = c.FullType,
                    Rarity = c.Rarity,
                    Img = c.Img,
                    MultiverseId = c.MultiverseId,
                    SearchName = c.SearchName,
                    Symbol = c.SetAsKeyRuneIcon,
                    Desc = c.Desc,
                    DescSymbols = c.DescWithSymbols,
                    ConvertedManaCost = c.Cmc,
                    Prices = GetCardPrice(priceStore, c.MultiverseId)
                }).OrderByDescending(c => c.SearchName);
            
                sw.Stop();

                return Response.AsJson(new
                {
                    SearchTerm = term,
                    Limit = limit,
                    Elapsed = sw.Elapsed.ToString(),
                    Cards = cards, 
                });
            };

            //
            // Testing out paging
            Post["/api/0.x/search", true] = async (parameters, ct) =>   
            {
                var sw = Stopwatch.StartNew();

                int limit = 1000;

                //string term = parameters.term;

                var query = this.Bind<SearchQuery>();

                if (query == null)
                    return HttpStatusCode.NotAcceptable;

                if (string.IsNullOrEmpty(query.SearchTerm))
                {
                    return HttpStatusCode.NotAcceptable;
                }

                var db_cards = await mtgStore.SearchCards(query.SearchTerm, query.Page, limit);

                if (db_cards == null)
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

                var cards = db_cards.Select(c => new
                {
                    Name = c.Name,
                    Code = c.SetId,
                    Set = c.SetName,
                    Cost = c.Cost,
                    CostSymbols = c.CostWithSymbols,
                    Type = c.FullType,
                    Rarity = c.Rarity,
                    Img = c.Img,
                    MultiverseId = c.MultiverseId,
                    SearchName = c.SearchName,
                    Symbol = c.SetAsKeyRuneIcon,
                    Desc = c.Desc,
                    DescSymbols = c.DescWithSymbols,
                    ConvertedManaCost = c.Cmc,
                    Prices = GetCardPrice(priceStore, c.MultiverseId)
                }).OrderByDescending(c => c.SearchName);

                sw.Stop();

                return Response.AsJson(new
                {
                    SearchTerm = query.SearchTerm,
                    Limit = limit,
                    Elapsed = sw.Elapsed.ToString(),
                    Cards = cards
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

            // Price changes
            Get["/price-changes", true] = async (parameters, ct) =>
            {
                int limit = 100;

                var sw = Stopwatch.StartNew();

                List<CardPrice> db_increases = priceStore.GetCardsByPriceIncrease(limit);
                List<CardPrice> db_decreases = priceStore.GetCardsByPriceDecrease(limit);

                var increases = db_increases.Select(c => new
                {
                    Name = c.Name,
                    Code = c.SetCode,
                    Symbol = c.SetAsKeyRuneIcon,
                    MultiverseId = c.MultiverseId,
                    PriceDiff = c.PriceDiff,
                    PriceDiffValue = c.PriceDiffValue,
                    PriceMid = c.PriceMid,
                    PriceFoil = c.PriceFoil,
                    Img = c.ImageUrl,
                    LastUpdated = c.LastUpdated.ToShortDateString(),
                    Url = c.Url
                });

                var decreases = db_decreases.Select(c => new
                {
                    Name = c.Name,
                    Code = c.SetCode,
                    Symbol = c.SetAsKeyRuneIcon,
                    MultiverseId = c.MultiverseId,
                    PriceDiff = c.PriceDiff,
                    PriceDiffValue = c.PriceDiffValue,
                    PriceMid = c.PriceMid,
                    PriceFoil = c.PriceFoil,
                    Img = c.ImageUrl,
                    LastUpdated = c.LastUpdated.ToShortDateString(),
                    Url = c.Url
                });

                sw.Stop();

                return View["index/price-changes.html", new
                {
                    Elapsed = sw.Elapsed.ToString(),
                    Limit = limit,
                    Increased = increases,
                    Decreased = decreases
                }];
            };

            // Ruling route
            Get["/ruling/{id:int}", true] = async (parameters, ct) =>
            {
                var sw = Stopwatch.StartNew();

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

                // Get price information
                var cardPrice = priceStore.GetCardPrice(card.MultiverseId);

                sw.Stop();

                return View["index/ruling.html", new
                {
                    Elapsed = sw.Elapsed.ToString(),
                    Card = card,
                    SetCode = !string.IsNullOrEmpty(set.GathererCode) ? set.GathererCode : set.Code,
                    CardPrices = new
                    {
                        Url = cardPrice != null ? cardPrice.Url : "",
                        Low = cardPrice != null ? cardPrice.PriceLow : "",
                        Mid = cardPrice != null ? cardPrice.PriceMid : "",
                        Foil = cardPrice != null ? cardPrice.PriceFoil : "",
                        Diff = cardPrice != null ? cardPrice.PriceDiff : "",
                    }
                }];
            };

            // Get search results
            Get["/search/{name}", true] = async (parameters, ct) =>
            {
                var sw = Stopwatch.StartNew();

                int limit = 100;

                string name = parameters.name;

                if (string.IsNullOrEmpty(name))
                {
                    return HttpStatusCode.Accepted;
                }

                var cards = await mtgStore.FullTextSearch(name, limit);

                if (cards == null)
                {
                    string msg = string.Format("No cards found using name '{0}'", name);

                    loggingService.Error(msg);

                    return msg;
                }

                sw.Stop();

                return View["index/search.html", new
                {
                    SearchTerm = name, 
                    Limit = limit,
                    Elapsed = sw.Elapsed.ToString(),
                    Cards = cards
                }];
            };

            #region Sets
            Get["/sets", true] = async (parameters, ct) =>
            {
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
                string setCode = parameters.set;

                var set = await mtgStore.GetSetByCode(setCode);

                // If set doesnt exist, redirect back to sets list
                if (set == null)
                {
                    return Response.AsRedirect("/sets", RedirectResponse.RedirectType.Temporary);
                }

                var db_cards = await mtgStore.GetCardsBySet(set.Name);

                // Get price information
                var cards = db_cards.Select(c => new
                {
                    Name = c.Name,
                    Code = c.SetId,
                    Set = c.SetName,
                    Cost = c.Cost,
                    CostSymbols = c.CostWithSymbols,
                    Type = c.FullType,
                    Rarity = c.Rarity,
                    Img = c.Img,
                    MultiverseId = c.MultiverseId,
                    SearchName = c.SearchName,
                    Symbol = c.SetAsKeyRuneIcon,
                    Desc = c.Desc,
                    DescSymbols = c.DescWithSymbols,
                    ConvertedManaCost = c.Cmc,
                    SetSymbol = c.SetAsKeyRuneIcon
                }).OrderByDescending(c => c.SearchName);

                return View["set.html", new
                {
                    Set = set,
                    Cards = cards
                }];

            };
            #endregion

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

        private dynamic GetCardPrice(ICardPriceStore priceStore, int multiverseId)
        {
            var price = priceStore.GetCardPrice(multiverseId);

            return new
            {
                Url = price != null ? price.Url : "",
                Low = price != null ? price.PriceLow : "",
                Mid = price != null ? price.PriceMid : "",
                Foil = price != null ? price.PriceFoil : "",
                Diff = price != null ? price.PriceDiff : "",
            };
        }
    }
}