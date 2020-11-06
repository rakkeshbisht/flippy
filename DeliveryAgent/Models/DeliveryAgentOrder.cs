using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryAgent.Models
{
    public class DeliveryAgentOrder
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string AgentId { get; set; }
        public string OrderId { get; set; }
        public string AgentName { get; set; }
        public string CustomerName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryAgentName { get; set; }

        public DeliveryAgentOrder()
        {
            Id = ObjectId.GenerateNewId();
        }
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
