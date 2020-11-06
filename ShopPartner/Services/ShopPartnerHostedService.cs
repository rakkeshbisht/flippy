using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using ShopPartner.Models;
using ShopPartner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopPartner
{
    public class ShopPartnerHostedService : BackgroundService
    {
        private readonly ConsumerConfig consumerConfig;

        private readonly Service _service;
        public ShopPartnerHostedService(ConsumerConfig consumerConfig, Service service)
        {
            this.consumerConfig = consumerConfig;

            this._service = service;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {

                Log.Information("ShopPartner subcriptions started");

                while (!stoppingToken.IsCancellationRequested)
                {        
                    
                    using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                    {
                        consumer.Subscribe(TopicNames.OrderPlaced);

                        try
                        {
                            while (true)
                            {
                                try
                                {
                                    var cr = consumer.Consume(stoppingToken);
                                    Log.Information("Message received" + cr.Message.Value);

                                    //Order Received
                                    CustomerOrder customerorder = JsonConvert.DeserializeObject<CustomerOrder>(cr.Message.Value);
                                    ShopOrder order = MapToShopOrder(customerorder);
                                    _service.InsertShopOrder(order);
                                    Log.Information("Inserted Shop Order:" + order.ToJson());
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, ex.Message);
                                }
                            }
                        }
                        catch (OperationCanceledException ex)
                        {
                            Log.Error(ex, ex.Message);
                            // Ensure the consumer leaves the group cleanly and final offsets are committed.
                            consumer.Close();
                        }
                    }
                }
            });
        }


        private static ShopOrder MapToShopOrder(CustomerOrder customerorder)
        {
            ShopOrder order = new ShopOrder();

            order.ShopId = customerorder.ShopId;
            order.OrderId = customerorder.Order.OrderId;
            order.ShopManager = "Anil Singh";
            order.ShopName = "Reliance Retail";
            order.Status = "Order Received";
            order.DeliveryAddress = customerorder.Order.DeliveryAddress;
            order.ItemList = customerorder.Order.ItemList;

            return order;
        }

    }
}
