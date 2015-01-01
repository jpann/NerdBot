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
        public IndexModule(IMtgContext context, IMessenger messenger, IPluginManager pluginManager)
        {
            Get["/"] = parameters =>
            {
                return HttpStatusCode.NotAcceptable;
            };

            Post["/"] = parameters =>
            {
                var message = new GroupMeMessage();

                // Bind and validate the request to GroupMeMessage
                var msg = this.BindToAndValidate(message);

                if (!ModelValidationResult.IsValid)
                    return HttpStatusCode.NotAcceptable;

                // Send to all plugins
                pluginManager.SendMessage(message, messenger);

                return HttpStatusCode.Accepted;
            };
        }
    }
}