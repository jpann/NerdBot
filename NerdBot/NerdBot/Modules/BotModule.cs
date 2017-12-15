using System;
using System.Linq;
using Nancy;
using Nancy.Extensions;
using Nancy.ModelBinding;
using NerdBot.Parsers;
using NerdBot.Reporters;
using NerdBotCommon.Factories;
using NerdBotCommon.Messengers.GroupMe;
using SimpleLogging.Core;

namespace NerdBot.Modules
{
    public class BotModule : NancyModule
    {
        public BotModule(
            BotConfig botConfig,
            IMessengerFactory messengerFactory,
            IPluginManager pluginManager,
            ICommandParser commandParser,
            ILoggingService loggingService,
            IReporter reporter) : base("/bot")
        {
            #region Bot Route
            Post["/{token}", true] = async (parameters, ct) =>
            {
                try
                {
                    // Get the request's body as a string, for logging
                    string request_string = this.Request.Body.AsString();

                    string sentToken = parameters.token;

                    // If the passed token segment does not match the secret token, return NotAcceptable status
                    if (botConfig.BotRoutes.FirstOrDefault(r => r.SecretToken == sentToken) == null)
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
                    if (message.name.ToLower() == botConfig.BotName.ToLower())
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
                            // Get instance of IMessenger for this bot route
                            var botMessenger = messengerFactory.Create(sentToken);

                            loggingService.Trace("Received command: {0}", command.Cmd);

                            if (command.Cmd.ToLower() == "help")
                            {
                                bool helpHandled = await pluginManager.HandledHelpCommand(command, botMessenger);
                            }
                            else
                            {
                                // If a message is in a command format '<cmd>\s[message]', 
                                //  have the plugin manager see if any loaded plugins are set to respond to that command
                                bool handled = await pluginManager.HandleCommand(command, message, botMessenger);

                                if (!handled)
                                    pluginManager.SendMessage(message, botMessenger);
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