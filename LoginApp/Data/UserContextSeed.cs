using LoginAppService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAppService.Data
{
    public class UserContextSeed
    {
        public static void SeedData(IMongoCollection<User> userCollection)
        {
            bool userExist= userCollection.Find(co => co.username == "rakesh.bisht" && co.password == "password").Any();
            if (!userExist)
            {
                userCollection.InsertManyAsync(GetUsers());
            }
        }

        private static IEnumerable<User> GetUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    username = "rakesh.bisht",
                    password = "password"
                }
            };
        }
    }
}
