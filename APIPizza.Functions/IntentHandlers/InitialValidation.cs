using Amazon.Lambda.LexEvents;
using System;
using System.Collections.Generic;
using System.Net.Cache;
using Amazon.Lambda.Core;
using APIPizza.Functions.IntentHandlers;
using DialogActionType = APIPizza.Functions.Enums.DialogActionType;
using FulfillmentState = APIPizza.Functions.Enums.FulfillmentState;

namespace APIPizza.Functions.IntentHandlers
{
    internal class InitialValidation : HandlerBase, IIntentHandler
    {
        
        private static readonly Random s_Random = new Random();

        public InitialValidation(LexEvent request) : base(request)
        {
        }


        public LexResponse HandleJob()
        {
            try
            {
                CheckWorkingTimes();
                //CheckGeography();
                AddInitializedFlag(Request);
                return DelegateToLex();
            }
            catch (Exception e)
            {
                return GenerateSimpleLexResponse(e.Message, FulfillmentState.Failed, DialogActionType.Close);
            }
        }

        private void AddInitializedFlag(LexEvent request)
        {
            if (request.SessionAttributes == null)
            {
                request.SessionAttributes = new Dictionary<string, string>();
            }
            request.SessionAttributes.Add("Initialized", true.ToString());
        }

        private static void CheckWorkingTimes()
        {
            var date = DateTime.UtcNow;
            if (date.Hour > 22 || date.Hour < 10)
            {
                throw new Exception("Sorry we do not serve at this time, please try after 10 am.");
            }
        }


        private static void CheckGeography()
        {
            //Geography check (Random)
            if (s_Random.Next(0, 100) < 20)
            {
                throw new Exception("Sorry we do not serve to your area, we will get in touch when we start to serve :)");
            }
        }

        
    }
}
