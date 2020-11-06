// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace LoginAppService
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "flippy.api",
                    DisplayName = "Flippy API",
                    Description = "Allow the application to access Flippy API on your behalf",
                    Scopes = new List<string> { "flippy.api.getallcustomerorders",
                                                "flippy.api.createorder",
                                                "flippy.api.getcustomerorder"},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> {"role"}
                },
                new ApiResource
                {
                    Name = "shoppartner.api",
                    DisplayName = "ShopPartner API",
                    Description = "Allow the application to access ShopPartner API on your behalf",
                    Scopes = new List<string> { "shoppartner.api.getallshoporders",
                                                "shoppartner.api.getspecificshoporder",
                                                "shoppartner.api.acceptorder",
                                                "shoppartner.api.rejectorder",
                                                "shoppartner.api.orderready"},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> {"role"}
                },
                new ApiResource
                {
                    Name = "deliveryagent.api",
                    DisplayName = "DeliveryAgent API",
                    Description = "Allow the application to access DeliveryAgent API on your behalf",
                    Scopes = new List<string> { "deliveryagent.api.getallagentorders",
                                                "deliveryagent.api.getspecificagentorder",
                                                "deliveryagent.api.arrivedatshop",
                                                "deliveryagent.api.pickedupdelivery",
                                                "deliveryagent.api.orderdelivered"},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> {"role"}
                }
            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("flippy.api.getallcustomerorders", "Get all customer orders by FlippyAPI"),
                new ApiScope("flippy.api.createorder", "Create new Order by FlippyAPI"),
                new ApiScope("flippy.api.getcustomerorder", "Get a single customer Order by FlippyAPI"),

                new ApiScope("shoppartner.api.getallshoporders", "ShopPartnerAPI  getallshoporders"),
                new ApiScope("shoppartner.api.getspecificshoporder","ShopPartnerAPI getspecificshoporder"),
                new ApiScope("shoppartner.api.acceptorder","ShopPartnerAPI acceptorder"),
                new ApiScope("shoppartner.api.rejectorder","ShopPartnerAPI rejectorder"),
                new ApiScope("shoppartner.api.orderready","ShopPartnerAPI orderready"),

                new ApiScope("deliveryagent.api.getallagentorders", "DeliveryAgentAPI  getallagentorders"),
                new ApiScope("deliveryagent.api.getspecificagentorder","DeliveryAgentAPI getspecificagentorder"),
                new ApiScope("deliveryagent.api.arrivedatshop","DeliveryAgentAPI arrivedatshop"),
                new ApiScope("deliveryagent.api.pickedupdelivery","DeliveryAgentAPI pickedupdelivery"),
                new ApiScope("deliveryagent.api.orderdelivered","DeliveryAgentAPI orderdelivered")
            };
        }


        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "Flippy",
                ClientName = "Flippy Mobile Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("ClientSecret".Sha256())
                },
                 AllowedScopes = new List<string> { "flippy.api.getallcustomerorders",
                                                    "flippy.api.createorder",
                                                    "flippy.api.getcustomerorder" }
            },
            new Client
            {
                ClientId = "ShopPartner",
                ClientName = "ShopPartner Desktop Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("ClientSecret".Sha256())
                },
                AllowedScopes = new List<string> {"shoppartner.api.getallshoporders",
                                                "shoppartner.api.getspecificshoporder",
                                                "shoppartner.api.acceptorder",
                                                "shoppartner.api.rejectorder",
                                                "shoppartner.api.orderready" }
            },
            new Client
            {
                ClientId = "DeliveryAgent",
                ClientName = "DeliveryAgent Mobile Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("ClientSecret".Sha256())
                },
                AllowedScopes = new List<string> { "deliveryagent.api.getallagentorders",
                                                   "deliveryagent.api.getspecificagentorder",
                                                   "deliveryagent.api.arrivedatshop",
                                                   "deliveryagent.api.pickedupdelivery",
                                                   "deliveryagent.api.orderdelivered"
                }
             }
        };
    }

    public class Users
    {
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
            new TestUser {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "rakesh.bisht",
                Password = "password",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "rakesh.bisht@cognizant.com"),
                    new Claim(JwtClaimTypes.Role, "admin")
                }
            }
        };
        }
    }
}