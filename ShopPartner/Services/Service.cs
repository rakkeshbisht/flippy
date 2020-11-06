using Confluent.Kafka;
using ShopPartner.Models;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Linq;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace ShopPartner.Services
{
    public class Service
    {
        private readonly IMongoCollection<ShopOrder> _shopOrders;
        private readonly ProducerConfig _producerConfig;
        private string dockerHost = string.Empty;
        private IConfiguration _config;

        public Service(IDatabaseSettings settings, ProducerConfig producerConfig, IConfiguration config)
        {
            _config = config;
            dockerHost = _config.GetValue<string>("DOCKER_HOST");
            _producerConfig = producerConfig;            
            var client = new MongoClient("mongodb://" + dockerHost + ":27017");
            var database = client.GetDatabase(settings.DatabaseName);

            _shopOrders = database.GetCollection<ShopOrder>(settings.CollectionName);
        }

        public List<ShopOrder> GetAllShopOrders(string shopId, int per_page, int page, out long totalItems)
        {
            totalItems = _shopOrders.CountDocuments(new BsonDocument());

            return _shopOrders.Find<ShopOrder>(co => co.ShopId == shopId).Skip((page - 1) * per_page).Limit(per_page).ToList();
        }

        public ShopOrder GetSpecificShopOrder(string orderId, string shopId) =>
           _shopOrders.Find<ShopOrder>(co => co.OrderId == orderId && co.ShopId == shopId).FirstOrDefault();

        public ShopOrder GetCustomerOrder(string customerId) =>
              _shopOrders.Find<ShopOrder>(co => co.OrderId == customerId).FirstOrDefault();

        public ShopOrder InsertShopOrder(ShopOrder customerOrder)
        {
            _shopOrders.InsertOne(customerOrder);

            return customerOrder;
        }

        public ShopOrder UpdateOrder(string orderId, string shopId, string status)
        {
            var update = Builders<ShopOrder>.Update.Set("Status", status);
            var result = _shopOrders.UpdateOne(x => x.OrderId == orderId && x.ShopId == shopId, update);

            if (result.IsAcknowledged)
            {
                var order = GetSpecificShopOrder(orderId, shopId);
                //Order Ready
                order.Status = status;
                UpdateShopOrder(order, order.Status);

                return order;
            }

            return null;
        }

        public void PublishMessage(ShopOrder shopOrder, string topic)
        {
            string payload = Newtonsoft.Json.JsonConvert.SerializeObject(shopOrder);

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

        public void UpdateShopOrder(ShopOrder order, string status)
        {
            var filter = Builders<ShopOrder>.Filter.Eq("CustomerId", order.OrderId);
            var update = Builders<ShopOrder>.Update.Set("Status", status);

            _shopOrders.UpdateOne(filter, update);
        }
    }
}
