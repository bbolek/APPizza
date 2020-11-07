using Amazon.Lambda.LexEvents;
using System;
using Amazon.Lambda.Core;
using Amazon.Lex;
using Enums = APIPizza.Functions.Enums;

namespace APIPizza.Functions.IntentHandlers
{
    class OrderPizzaIntentValidation : HandlerBase, IIntentHandler
    {
        public OrderPizzaIntentValidation(LexEvent request) : base(request)
        {
        }

        public LexResponse HandleJob()
        {

            try
            {
                var topping = Request.CurrentIntent.Slots["topping"];
                var pizzaCount = Request.CurrentIntent.Slots["pizzaCount"];
                var pizzaSize = Request.CurrentIntent.Slots["pizzaSize"];
                var crust = Request.CurrentIntent.Slots["crust"];

                ValidateTopping(topping);

                return DelegateToLex();
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Exception :" + e.Message);
                return GenerateElicitSlotLexResponse(e.Message, Request.CurrentIntent.Name, "topping");
            }
        }

        private void ValidateTopping(string topping)
        {
            if (topping == "Pineapple")
            {
                Request.CurrentIntent.Slots.Remove("topping");
                throw new Exception("Really? Pineapple? Do you even exist? Can you try another topping like pepperoni?");
            }
        }

        
    }
}
