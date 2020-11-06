using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopPartner.Models
{
    public class ItemDetail
    {
        [BsonId]        
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public string Item { get; set; }
    }
}
