using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopPartner.Models
{
    public class CustomerOrder
    {
        [BsonId]        
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public Order Order { get; set; }
        public string Status { get; set; }
    }
}
