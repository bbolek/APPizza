using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using APIPizza.Functions.Enums;

namespace APIPizza.Functions.IntentHandlers
{
    public class QueryOrder : HandlerBase,IIntentHandler
    {
        private readonly Random _random = new Random();

        public QueryOrder(LexEvent request) : base(request)
        {
        }

        public LexResponse HandleJob()
        {
            var orderId = Request.CurrentIntent.Slots["orderId"];
            var message = "Your order is not found!";
            using var connection = CreateConnection();
            var command = new SqlCommand("Select id, topping, pizzaSize, pizzaCount, crust, date from orders where id = @id order by date desc", connection);
            command.Parameters.AddWithValue("@id", orderId);
            using (command)
            {
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var date = reader.GetDateTime(5);
                        var minutesTillOrder = Math.Round(DateTime.UtcNow.Subtract(date).TotalMinutes, MidpointRounding.ToEven);
                        var minutesToArrive = (30 - minutesTillOrder) < 0 ? "soon" : $"in {30 - minutesTillOrder + _random.Next(0, 10)} minutes";
                        message = $"It has ben {minutesTillOrder} minutes since you gave order. It will be there {minutesToArrive}, we hope you will enjoy it.";
                        break;
                    }
                }
                connection.Close();
            }

            return GenerateSimpleLexResponse(
                message,
                FulfillmentState.Fulfilled,
                DialogActionType.Close);
        }
    }
}