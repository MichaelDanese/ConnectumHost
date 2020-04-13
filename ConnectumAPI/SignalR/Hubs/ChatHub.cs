using ConnectumAPI.Persistence;
using ConnectumAPI.Persistence.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            Connection adder = new Connection();
            adder.ConnectionID = Context.ConnectionId;
            int name = Int32.Parse(Context.User.Identity.Name);
            var obj = _db.Users.FirstOrDefault(a => a.UserId == name);
            adder.Name = obj.UserName;
            _db.Connections.Add(adder);
            _db.SaveChanges();
            
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var connectionId = Context.ConnectionId;
            var obj = _db.Connections.FirstOrDefault(a => a.ConnectionID == connectionId);
            var partner = obj.Partner;
            _db.Connections.Remove(obj);
            _db.SaveChanges();
            var search = _db.Connections.FirstOrDefault(a => a.ConnectionID == partner);
            if (search != null)
            {
                var one = search.ConnectionID;
                var two = search.Partner;
                search.Partner = null;
                _db.Update(search);
                _db.SaveChanges();
                await DisconnectToUser(one);
            }
            await base.OnDisconnectedAsync(ex);
        }
        public async Task DisconnectToUser(string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("Disconnected");

        }
        public async void EndChat(string connectionId)
        {
            var search = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId);
            var searchTwo = _db.Connections.FirstOrDefault(a => a.ConnectionID == connectionId);
            if (search != null && searchTwo != null)
            {
                var one = search.ConnectionID;
                var two = search.Partner;
                search.Partner = null;
                searchTwo.Partner = null;
                _db.Update(search);
                _db.Update(searchTwo);
                _db.SaveChanges();
                await DisconnectToUser(one);
                await DisconnectToUser(two);
            }

        }
        public Task SendMessageToUser( string connectionId, string input)
        {
            Message output = new Message();
            output.connectionID = connectionId;
            output.name = "Stranger";
            output.message = input;
            return Clients.Client(connectionId).SendAsync("ReceiveMessage", output);

        }
        public Task GreetUser(string connectionId)
        {
            if (connectionId == Context.ConnectionId)
            {
                Message outputs = new Message();
                outputs.connectionID = "";
                outputs.name = "You";
                outputs.message = "You cannot connect to yourself!";
                return Clients.Client(Context.ConnectionId).SendAsync("DenialMessage", outputs);
            }
            var obj = _db.Connections.FirstOrDefault(a => a.ConnectionID == connectionId);
            var objTwo = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId);

            if (obj != null && objTwo != null && obj.Partner == null && objTwo.Partner == null)
            {
                obj.Partner = objTwo.ConnectionID;
                _db.Connections.Update(obj);
                objTwo.Partner = obj.ConnectionID;
                _db.Connections.Update(objTwo);
                _db.SaveChanges();
                Message outputs = new Message();
                outputs.connectionID = Context.ConnectionId;
                outputs.name = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId).Name; ;
                outputs.message = "You are now chatting with " + outputs.name;
                return Clients.Client(connectionId).SendAsync("GreetMessage", outputs);
            }
            Message output = new Message();
            output.connectionID = "";
            output.name = "You";
            output.message = connectionId + " has already started a conversation with another user.";
            return Clients.Client(Context.ConnectionId).SendAsync("DenialMessage", output);
        }
        public Task FinishGreet(string client)
        {
            Message output = new Message();
            output.connectionID = Context.ConnectionId;
            output.name = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId).Name;
            output.message = "You are now chatting with " + output.name;
            return Clients.Client(client).SendAsync("StartMessage", output);
        }
        public Task PostInterest(string interests)
        {
            SearchQuery values = new SearchQuery();
            var obj = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId);
            if (obj != null)
            {
                obj.Interest = interests;
                _db.Connections.Update(obj);
                _db.SaveChanges();
                
                var objList = _db.Connections.Where(a => a.Interest == interests).ToList();
                
                if (objList != null)
                {
                    values.found = "true";
                    values.message = "Success! These are the users found with similar interests...";
                    List<string> lister = new List<string>();
                    foreach (var i in objList)
                    {
                        if (i.ConnectionID != Context.ConnectionId)
                        {
                            lister.Add(i.ConnectionID);
                        } 
                    }
                    values.results = lister.ToArray();
                    if (values.results.Length == 0)
                    {
                        values.found = "false";
                        values.message = "No users were found with similar interests. Please wait or search another tag...";
                    }
                }
                else
                {
                    values.message = "No users were found with similar interests. Please wait or search another tag...";
                    values.found = "false";
                }

            }
            else
            {
                values.message = "Error! Cannot find you in database...";
                values.found = "false";
            }
            var json = JsonConvert.SerializeObject(values);
            return Clients.Client(Context.ConnectionId).SendAsync("ShowConnections", json);


        }
    }
    public class Message
    {
        [JsonProperty("connectionID")]
        public string connectionID { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
    }
    public class SearchQuery
    {
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("results")]
        public string[] results { get; set; }
        [JsonProperty("found")]
        public string found { get; set; }
    }
}
