using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopPartner.Models
{
    public class Order
    {
        [BsonId]        
        public string OrderId { get; set; }

        public string RejectReason { get; set; }

        public string DeliveryAddress { get; set; }

        public List<ItemDetail> ItemList { get; set; }      
        
    }
}

