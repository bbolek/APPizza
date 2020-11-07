using Amazon.Lambda.LexEvents;

namespace APIPizza.Functions.IntentHandlers
{
    internal class OrderDrinkIntentValidation : HandlerBase, IIntentHandler
    {
        public OrderDrinkIntentValidation(LexEvent request) : base(request)
        {
        }

        public LexResponse HandleJob()
        {
            return DelegateToLex();
        }
    }
}
