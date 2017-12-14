using NerdBotCommon.Messengers;

namespace NerdBotCommon.Factories
{
    public interface IMessengerFactory
    {
        IMessenger Create();
        IMessenger Create(string name);
    }
}