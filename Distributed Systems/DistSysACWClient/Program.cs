using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistSysACWClient
{
    #region Task 10 and beyond
    class UserID
    {
        public static string APIKey { get; set; }
        public static string UserName { get; set; }
    }
    class Client
    {
        static readonly HttpClient client = new HttpClient();
        static async Task RunAsync(string response)
        {
            if (response == "TalkBack Hello")
            {
                await GetHello();
            }
            else if (response.Contains("User Get"))
            {
                string username = GetUsername(response);
                await GetUser(username);
            }
            else if (response.Contains("User Post"))
            {
                string username = GetUsername(response);
                await PostUser(username);
            }
            else if (response.Contains("User Set"))
            {
                string[] split = new string[4];
                split = response.Split(" ");
                string username = split[2];
                string APIKey = split[3];
                UserSet(username, APIKey);
            }
            else if(response.Contains("User Delete"))
            {
                await UserDelete();
            }
            else if (response.Contains("Protected Hello"))
            {
                await ProtectedHello();
            }
            else if (response.Contains("Protected SHA1"))
            {
                string[] split = new string[3];
                split = response.Split(" ");
                string String = split[2];
                await ProtectedSha1(String);
            }
            else if (response.Contains("Protected SHA256"))
            {
                string[] split = new string[3];
                split = response.Split(" ");
                string String = split[2];
                await ProtectedSha256(String);
            }

        }
        static string Firsttime()
        {  
            Console.WriteLine("Hello. What would you like to do?");
            string response = Console.ReadLine();
            if (response == "Exit")
            {
                return "exit";
            }
            RunAsync(response).GetAwaiter().GetResult();
            return "ok";
        }
        static string NextTime()
        {
            Console.WriteLine("What would you like to do next?");
            string response = Console.ReadLine();
            if (response == "Exit")
            {
                return "exit";
            }
            Console.Clear();
            RunAsync(response).GetAwaiter().GetResult();
            return "ok";
        }

        static void Main()
        {
            string input = Firsttime();
            while(input == "ok")
            {
                input = NextTime();
            }
            if (input == "Exit")
            {
                Environment.Exit(0);
            }
        }

        static async Task GetHello()
        {
            //http://distsysacw.azurewebsites.net/9940872/Api/Talkback/Hello
            try
            {
                Console.WriteLine("...please Wait...");
                HttpResponseMessage responseMessage = await client.GetAsync("http://localhost:5000/api/Talkback/Hello");
                string ServerResponse = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine(ServerResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }
        static async Task GetUser(string Username)
        {
            try
            {
                Console.WriteLine("...please Wait...");
                HttpResponseMessage responseMessage = await client.GetAsync("http://localhost:5000/api/user/new?username=" + Username);
                string ServerResponse = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine(ServerResponse);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }
        static async Task PostUser(string username)
        {
            try
            {
                Console.WriteLine("...please Wait...");
                var json = JsonConvert.SerializeObject(username);
                HttpRequestMessage RequestMessage = new HttpRequestMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                HttpResponseMessage message = await client.PostAsync("http://localhost:5000/api/user/new", RequestMessage.Content);
                
                string serverResponse = await message.Content.ReadAsStringAsync();
                if (message.IsSuccessStatusCode == true)
                {
                    UserID.UserName = username;
                    UserID.APIKey = serverResponse;
                    Console.WriteLine("Got API Key");
                }
                else
                {
                    Console.WriteLine(serverResponse);
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }
        static void UserSet(string username, string APIKey)
        {
            UserID.UserName = username;
            UserID.APIKey = APIKey;
            Console.WriteLine("Stored");
        }

        static async Task ProtectedHello()
        {
            string response = CheckUser();
            if (response == "Blank")
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            else if (response == "Not Blank")
            {
                string apikey = UserID.APIKey;
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                UriBuilder builder = new UriBuilder("http://localhost:5000/api/protected/hello");
                requestMessage.RequestUri = builder.Uri;
                requestMessage.Method = HttpMethod.Get;
                requestMessage.Headers.Add("x-api-key", apikey);
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);
                string responsestring = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(responsestring);
            }
        }
        static async Task ProtectedSha1(string String)
        {
            string response = CheckUser();
            if (response == "Blank")
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            else if (response == "Not Blank")
            {
                string apikey = UserID.APIKey;
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                UriBuilder builder = new UriBuilder("http://localhost:5000/api/protected/sha1");
                builder.Query = "?message=" + String;
                requestMessage.RequestUri = builder.Uri;
                requestMessage.Method = HttpMethod.Get;
                requestMessage.Headers.Add("x-api-key", apikey);
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);
                string responsestring = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(responsestring);
            }
        }
        static async Task ProtectedSha256(string String)
        {
            string response = CheckUser();
            if (response == "Blank")
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            else if (response == "Not Blank")
            {
                string apikey = UserID.APIKey;
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                UriBuilder builder = new UriBuilder("http://localhost:5000/api/protected/sha256");
                builder.Query = "?message=" + String;
                requestMessage.RequestUri = builder.Uri;
                requestMessage.Method = HttpMethod.Get;
                requestMessage.Headers.Add("x-api-key", apikey);
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);
                string responsestring = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(responsestring);
            }
        }

        static async Task UserDelete()
        {
            string response = CheckUser();
            if (response == "Blank")
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return;
            }
            else if(response == "Not Blank")
            { 
                string apikey = UserID.APIKey;
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                UriBuilder builder = new UriBuilder("http://localhost:5000/api/user/removeuser");
                builder.Query = "?username=" + UserID.UserName; 
                requestMessage.RequestUri = builder.Uri;
                requestMessage.Method = HttpMethod.Delete;
                requestMessage.Headers.Add("x-api-key", apikey);
                HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);
                string responsestring = await httpResponse.Content.ReadAsStringAsync();
                if (responsestring == "true")
                {
                    Console.WriteLine("true");
                }
                else if (responsestring == "false")
                {
                    Console.WriteLine("false");
                }
            }

        }

        static string CheckUser()
        {
            string check;
            if (UserID.APIKey == null && UserID.UserName == null)
            {
                check = "Blank";
                return check;
            }
            else
            {
                check = "Not Blank";
                return check;
            }
        }

        static string GetUsername(string response)
        {
            string[] split = new string[3];
            split = response.Split(" ");
            string username = split[2];
            return username;
        }

    }
    #endregion
}
