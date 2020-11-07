using System;
using System.Data.SqlClient;
using System.Net.Cache;
using Amazon.Lambda.LexEvents;
using APIPizza.Functions.Enums;

namespace APIPizza.Functions.IntentHandlers
{
    public class HandlerBase
    {
        protected LexEvent Request { get; }

        public HandlerBase(LexEvent request)
        {
            Request = request;
        }

        protected static LexResponse GenerateSimpleLexResponse(string message, FulfillmentState fulfillmentState, DialogActionType type)
        {
            return new LexResponse()
            {
                DialogAction = new LexResponse.LexDialogAction()
                {
                    Message = new LexResponse.LexMessage()
                    {
                        Content = message,
                        ContentType = "PlainText"
                    },
                    FulfillmentState = fulfillmentState.ToString("g"),
                    Type = type.ToString("g")
                }
            };
        }

        protected LexResponse GenerateElicitSlotLexResponse(string message, string intentName, string slotToElicit)
        {
            return new LexResponse()
            {
                DialogAction = new LexResponse.LexDialogAction()
                {
                    IntentName = intentName,
                    Message = new LexResponse.LexMessage()
                    {
                        Content = message,
                        ContentType = "PlainText"
                    },
                    SlotToElicit = slotToElicit,
                    Slots = Request.CurrentIntent.Slots,
                    Type = DialogActionType.ElicitSlot.ToString("g")
                }
            };
        }

        public LexResponse DelegateToLex()
        {
            return new LexResponse()
            {
                
                DialogAction = new LexResponse.LexDialogAction()
                {
                    Slots = Request.CurrentIntent.Slots,
                    Type = DialogActionType.Delegate.ToString("g")
                },
                SessionAttributes = Request.SessionAttributes
            };
        }

        

        protected static SqlConnection CreateConnection()
        {
            var server = Environment.GetEnvironmentVariable("DB_ENDPOINT");
            var database = Environment.GetEnvironmentVariable("DATABASE");
            var username = Environment.GetEnvironmentVariable("USER");
            var pwd = Environment.GetEnvironmentVariable("PASSWORD");
            var connectionString = $"Data Source={server};Initial Catalog={database};User ID={username};Password={pwd}";
            var connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
