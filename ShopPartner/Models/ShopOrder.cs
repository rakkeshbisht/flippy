using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopPartner.Models
{
    public class ShopOrder
    {
        [BsonId]

        public ObjectId Id { get; set; }
        public string OrderId { get; set; }
        public string ShopManager { get; set; }
        public string ShopName { get; set; }
        public string Status { get; set; }
        public string RejectReason { get; set; }
        public string DeliveryAddress { get; set; }
        public List<ItemDetail> ItemList { get; set; }
        public string ShopId { get; set; }

        public ShopOrder()
        {
            Id = ObjectId.GenerateNewId();
        }
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
