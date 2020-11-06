using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using DeliveryAgent.Models;
using DeliveryAgent.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Collections.Generic;

namespace DeliveryAgent
{
    public class DeliveryAgentHostedService : BackgroundService
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly Service _service;
        public DeliveryAgentHostedService(ConsumerConfig consumerConfig, Service service)
        {
            this._consumerConfig = consumerConfig;
            this._service = service;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topicsToSubscribe = new List<string>();
            topicsToSubscribe.Add(TopicNames.DeliveryRequested);
            topicsToSubscribe.Add(TopicNames.OrderReady);

            Log.Information("DeliveryAgent subcriptions Started");


            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {                    
                    using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
                    {
                        consumer.Subscribe(topicsToSubscribe);

                        try
                        {
                            while (true)
                            {
                                try
                                {
                                    var cr = consumer.Consume(stoppingToken);
                                    Log.Information("Message received" + cr.Message.Value);

                                    var shopOrder = JsonConvert.DeserializeObject<ShopOrder>(cr.Message.Value);
                                    //Order Received
                                    DeliveryAgentOrder agentOrder = MapToAgentOrder(shopOrder);

                                    switch (cr.Topic)
                                    {
                                        case TopicNames.DeliveryRequested:
                                            agentOrder.Status = "Delivery Requested";
                                            _service.InsertDeliveryAgentOrder(agentOrder);
                                            Log.Information("Inserted Delivery Agent Order:" + agentOrder.ToJson());
                                            break;
                                        case TopicNames.OrderReady:
                                            var status = "Order Ready";
                                            _service.UpdateDeliveryAgentOrder(agentOrder, status);
                                            Log.Information("Updated Delivery Agent Order:" + agentOrder.ToJson() + ",Status:" + status);
                                            break;
                                    }
                                }
                                catch (ConsumeException e)
                                {
                                    Log.Error(e, e.Message);
                                }
                            }
                        }
                        catch (OperationCanceledException e)
                        {
                            Log.Error(e, e.Message);
                            // Ensure the consumer leaves the group cleanly and final offsets are committed.
                            consumer.Close();
                        }
                    }
                }
            });
        }

        private DeliveryAgentOrder MapToAgentOrder(ShopOrder shopOrder)
        {
            DeliveryAgentOrder order = new DeliveryAgentOrder();

            order.ShopId = shopOrder.ShopId;
            order.ShopName = shopOrder.ShopName;
            order.OrderId = shopOrder.OrderId;
            order.DeliveryAddress = shopOrder.DeliveryAddress;
            order.DeliveryAgentName = "Ram Singh";
            order.AgentId = "7777";

            return order;
        }
    }
}
