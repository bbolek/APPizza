using System;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using APIPizza.Functions.Enums;
using APIPizza.Functions.IntentHandlers;

// Assembly attribute to enable the Lambda function's JSON request to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace APIPizza.Functions
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public LexResponse FunctionHandler(LexEvent request, ILambdaContext context)
        {
            LambdaLogger.Log(JsonSerializer.Serialize(request));

            if (request.SessionAttributes == null || !request.SessionAttributes.ContainsKey("Initialized"))
            {
                var validationResult = new InitialValidation(request).HandleJob();
                if (validationResult.DialogAction.FulfillmentState == FulfillmentState.Failed.ToString())
                {
                    return validationResult;
                }
            }

            IIntentHandler validator = null;
            switch (request.CurrentIntent.Name)
            {
                case "OrderPizza":
                    switch (request.InvocationSource)
                    {
                        case "DialogCodeHook":
                            validator = new OrderPizzaIntentValidation(request);
                            break;
                        case "FulfillmentCodeHook":
                            validator = new SaveOrder(request);
                            break;
                    }

                    break;
                case "QueryOrder":
                    validator = new QueryOrder(request);
                    break;
                default:
                    return new HandlerBase(request).DelegateToLex();
            }

            return validator?.HandleJob();
        }
    }
}
