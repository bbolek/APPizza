using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using APIPizza.Functions.Enums;

namespace APIPizza.Functions.IntentHandlers
{
    public class SaveOrder : HandlerBase,IIntentHandler
    {
        private readonly Random _random = new Random();

        public SaveOrder(LexEvent request) : base(request)
        {
            
        }

        public LexResponse HandleJob()
        {
            if (Request.CurrentIntent.ConfirmationStatus == ConfirmationStatus.Confirmed.ToString("g"))
            {
                var topping = Request.CurrentIntent.Slots["topping"];
                var pizzaCount = Request.CurrentIntent.Slots["pizzaCount"];
                var pizzaSize = Request.CurrentIntent.Slots["pizzaSize"];
                var crust = Request.CurrentIntent.Slots["crust"];
                var id = _random.Next(1, 10000);

                using var connection = CreateConnection();
                var command = new SqlCommand($"Insert into Orders (id, topping, pizzaSize, pizzaCount, crust, date)  values (@id, @topping, @pizzaSize, @pizzaCount, @crust, @date)", connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@topping", topping);
                command.Parameters.AddWithValue("@pizzaSize", pizzaSize);
                command.Parameters.AddWithValue("@pizzaCount", pizzaCount);
                command.Parameters.AddWithValue("@crust", crust);
                command.Parameters.AddWithValue("@date", DateTime.UtcNow);

                using (command)
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return GenerateSimpleLexResponse(
                        $"Your order has been processed, Order number : {id}, please note this order number",
                        FulfillmentState.Fulfilled,
                        DialogActionType.Close);
                }

            }
            else
            {
                return GenerateSimpleLexResponse(
                    "Your order is not processed.",
                    FulfillmentState.Failed,
                    DialogActionType.Close);
            }
        }
    }
}