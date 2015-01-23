using System.Linq;
using Nancy.ModelBinding;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;
using NerdBot.Parsers;
using SimpleLogging.Core;

namespace NerdBot
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
            ILoggingService loggingService)
        {
            Get["/"] = parameters =>
            {
                loggingService.Warning("GET request from {0}: Path '{1}' was invalid.",
                        this.Request.UserHostAddress,
                        this.Request.Path);
                
                return HttpStatusCode.NotAcceptable;
            };

            Post["/bot/{token}", true] = async (parameters, ct) =>
            {
                string sentToken = parameters.token;

                // If the passed token segment does not match the secret token, return NotAcceptable status
                if (sentToken != botConfig.SecretToken)
                {
                    loggingService.Warning("POST request from {0}: Token '{1}' was invalid.", 
                        this.Request.UserHostAddress,
                        sentToken);

                    return HttpStatusCode.NotAcceptable;
                }

                var message = new GroupMeMessage();

                // Bind and validate the request to GroupMeMessage
                var msg = this.BindToAndValidate(message);

                if (!ModelValidationResult.IsValid)
                {
                    loggingService.Warning("POST request from {0}: Message was invalid.",
                        this.Request.UserHostAddress);

                    return HttpStatusCode.NotAcceptable;
                }

                // Don't handle messages sent from ourself
                if (message.name.ToLower() == messenger.BotName.ToLower())
                    return HttpStatusCode.NotAcceptable;

                // Parse the command, if any
                var command = commandParser.Parse(message.text);
                if (command != null)
                {
                    if (!string.IsNullOrEmpty(command.Cmd))
                    {
                        if (command.Cmd.ToLower() == "help")
                        {
                            bool helpHandled = await pluginManager.HandledHelpCommand(command, messenger);
                        }
                        else
                        {
                            // If a message is in a command format '<cmd>\s[message]', 
                            //  have the plugin manager see if any loaded plugins are set to respond to that command
                            bool handled = await pluginManager.HandleCommand(command, message, messenger);
                        }
                    }
                }

                return HttpStatusCode.Accepted;
            };
        }
    }
}