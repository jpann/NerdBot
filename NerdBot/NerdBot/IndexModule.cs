using Nancy.Json;
using Nancy.ModelBinding;
using NerdBot.Messengers;
using NerdBot.Messengers.GroupMe;
using NerdBot.Mtg;

namespace NerdBot
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        public IndexModule(
            BotConfig botConfig,
            IMtgStore mtgStore, 
            IMessenger messenger, 
            IPluginManager pluginManager)
        {
            Get["/"] = parameters =>
            {
                return HttpStatusCode.NotAcceptable;
            };

            Post["/bot/{token}", true] = async (parameters, ct) =>
            {
                string sentToken = parameters.token;

                // If the passed token segment does not match the secret token, return NotAcceptable status
                if (sentToken != botConfig.SecretToken)
                {
                    return HttpStatusCode.NotAcceptable;
                }

                var message = new GroupMeMessage();

                // Bind and validate the request to GroupMeMessage
                var msg = this.BindToAndValidate(message);

                if (!ModelValidationResult.IsValid)
                    return HttpStatusCode.NotAcceptable;

                // Don't handle messages sent from ourself
                if (message.name.ToLower() == messenger.BotName.ToLower())
                    return HttpStatusCode.NotAcceptable;

                // Send to all plugins
                pluginManager.SendMessage(message, messenger);

                return HttpStatusCode.Accepted;
            };
        }
    }
}