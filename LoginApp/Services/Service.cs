using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Linq;
using MongoDB.Bson;
using System.Collections.Generic;
using LoginAppService.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using LoginAppService.Data;
using Serilog;

namespace LoginAppService.Services
{
    public class Service
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _config;
        private string dockerHost = string.Empty;

        public Service(IDatabaseSettings settings, IConfiguration config)
        {
            _config = config;
            dockerHost = config.GetValue<string>("DOCKER_HOST");
            var client = new MongoClient("mongodb://"+ dockerHost + ":27017");
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.CollectionName);

            UserContextSeed.SeedData(_users);
        }

        public User GetUser(User user)
        {
            return _users.Find<User>(co => co.username == user.username && co.password == user.password).FirstOrDefault();
        }

        public User GetUser(string username)
        {
            return _users.Find<User>(co => co.username == username).FirstOrDefault();
        }               

        public async Task<UserToken> GetTokenAsync(string clientId, string clientsecret, string scope)
        {
            string data = string.Format("grant_type=client_credentials&scope={0}&client_id={1}&client_secret={2}", scope, clientId, clientsecret);                 
               
            var result = await PostAsync<UserToken>("http://" + dockerHost + ":5000/connect/token", data);
            return result;            
        }

        public async Task<TResult> PostAsync<TResult>(string uri, string data)
        {
            TResult result = default(TResult);
            try
            {

                HttpClient httpClient = CreateHttpClient(string.Empty);              

                var content = new StringContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage response = await httpClient.PostAsync(uri, content);

                await HandleResponse(response);
                string serialized = await response.Content.ReadAsStringAsync();

                result = await Task.Run(() =>
                Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(serialized));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }

            return result;
        }

        private HttpClient CreateHttpClient(string token = "")
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }
        private void AddBasicAuthenticationHeader(HttpClient httpClient, string clientId, string clientSecret)
        {
            if (httpClient == null)
                return;

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                return;

            httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }
    }

    public class ServiceAuthenticationException : Exception
    {
        public string Content { get; }

        public ServiceAuthenticationException()
        {
        }

        public ServiceAuthenticationException(string content)
        {
            Content = content;
        }
    }

    public class HttpRequestExceptionEx : HttpRequestException
    {
        public System.Net.HttpStatusCode HttpCode { get; }
        public HttpRequestExceptionEx(System.Net.HttpStatusCode code) : this(code, null, null)
        {
        }

        public HttpRequestExceptionEx(System.Net.HttpStatusCode code, string message) : this(code, message, null)
        {
        }

        public HttpRequestExceptionEx(System.Net.HttpStatusCode code, string message, Exception inner) : base(message,
            inner)
        {
            HttpCode = code;
        }

    }

}
