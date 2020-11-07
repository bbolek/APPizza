using Amazon.Lambda.LexEvents;

namespace APIPizza.Functions.IntentHandlers
{
    public interface IIntentHandler
    {
        LexResponse HandleJob();
    }
}
