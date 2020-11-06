using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryAgent.Models
{
    public class ShopOrder
    {        
        public string OrderId { get; set; }
        public string ShopManager { get; set; }
        public string ShopName { get; set; }
        public string Status { get; set; }
        public string RejectReason { get; set; }
        public string DeliveryAddress { get; set; }
        public List<ItemDetail> ItemList { get; set; }
        public string ShopId { get; set; }
        
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
    public class ItemDetail
    {
        [BsonId]
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public string Item { get; set; }
    }
}
