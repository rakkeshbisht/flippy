using Confluent.Kafka;
using DeliveryAgent.Models;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Linq;
using MongoDB.Bson;
using System.Collections.Generic;
using Serilog;

namespace DeliveryAgent.Services
{
    public class Service
    {
        private readonly IMongoCollection<DeliveryAgentOrder> _deliveryAgentOrders;
        private readonly ProducerConfig _producerConfig;

        public Service(IDatabaseSettings settings, ProducerConfig producerConfig)
        {
            _producerConfig = producerConfig;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _deliveryAgentOrders = database.GetCollection<DeliveryAgentOrder>(settings.CollectionName);
        }

        public List<DeliveryAgentOrder> GetAllAgentOrders(string agentId, int per_page, int page, out long totalItems)
        {
            totalItems = _deliveryAgentOrders.CountDocuments(new BsonDocument());
            return _deliveryAgentOrders.Find<DeliveryAgentOrder>(co => co.AgentId == agentId).Skip((page - 1) * per_page).Limit(per_page).ToList();
        }

        public DeliveryAgentOrder GetSpecificAgentOrder(string orderId, string agentId) =>
          _deliveryAgentOrders.Find<DeliveryAgentOrder>(co => co.OrderId == orderId && co.AgentId == agentId).FirstOrDefault();

        public DeliveryAgentOrder UpdateAgentOrder(string orderId, string agentId, string status)
        {
            var update = Builders<DeliveryAgentOrder>.Update.Set("Status", status);
            var result = _deliveryAgentOrders.UpdateOne(x => x.OrderId == orderId && x.AgentId == agentId, update);

            if (result.IsAcknowledged)
            {
                return GetSpecificAgentOrder(orderId, agentId);
            }

            return null;
        }

        public DeliveryAgentOrder InsertDeliveryAgentOrder(DeliveryAgentOrder customerOrder)
        {
            _deliveryAgentOrders.InsertOne(customerOrder);

            return customerOrder;
        }


        public void PublishMessage(DeliveryAgentOrder deliveryAgentOrder, string topic)
        {
            string payload = Newtonsoft.Json.JsonConvert.SerializeObject(deliveryAgentOrder);

            using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                try
                {
                    p.Produce(topic, new Message<Null, string> { Value = payload });
                }
                catch (ProduceException<Null, string> e)
                {
                    Log.Error(e, e.Message);
                }
            }
        }


        public void UpdateDeliveryAgentOrder(DeliveryAgentOrder order, string status)
        {
            var filter = Builders<DeliveryAgentOrder>.Filter.Eq("OrderId", order.OrderId);
            var update = Builders<DeliveryAgentOrder>.Update.Set("Status", status);

            _deliveryAgentOrders.UpdateOne(filter, update);
        }

    }
}
