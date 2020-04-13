using ConnectumAPI.Persistence;
using ConnectumAPI.Persistence.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                search.Interest = null;
                searchTwo.Interest = null;
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
        public Task FinishGreet(Connection greeter, Connection greeted)
        {
            Message output = new Message();
            output.connectionID = greeter.ConnectionID;
            output.name = greeter.Name;
            output.message = "You are now chatting with " + output.name;
            return Clients.Client(greeted.ConnectionID).SendAsync("StartMessage", output);
        }
        public string[] parseInterests(string interests)
        {
            if (interests == null)
            {
                return null;
            }
            return interests.Split("||"); 
        }
        public bool compareInterests(string[] interestPool, string interest)
        {
            if (interestPool == null || interest == null)
            {
                return false;
            }

            foreach (var i in interestPool)
            {
                if (i == interest)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task ConnectUsersTogether(string interests, string chatType)
        {
            if (interests == null || chatType == null)
            {
                Message output = new Message();
                output.connectionID = Context.ConnectionId;
                output.name = "You";
                output.message = "Invalid search query!";
                await Clients.Client(Context.ConnectionId).SendAsync("DenialMessage", output);
                return;
            }
            var currentUser = _db.Connections.FirstOrDefault(a => a.ConnectionID == Context.ConnectionId);
            var parsedString = parseInterests(interests);
            Connection possibleMatch = null;
            Connection[] matches = null;
            matches = _db.Connections.Where(a => (a.Partner == null && a.SearchType == chatType && a.ConnectionID != Context.ConnectionId)).ToArray();
            foreach (var i in parsedString)
            {
                foreach (var j in matches)
                {
                    if (compareInterests(parseInterests(j.Interest), i))
                    {
                        possibleMatch = j;
                        break;
                    }
                }
                if (possibleMatch != null)
                {
                    break;
                }
            }
            
            if (possibleMatch != null && currentUser != null)
            {//Will start the process of matching the users together. 
                possibleMatch.Partner = currentUser.ConnectionID;
                currentUser.Partner = possibleMatch.ConnectionID;
                _db.Connections.Update(possibleMatch);
                _db.Connections.Update(currentUser);
                _db.SaveChanges();
                possibleMatch.PartnerName = currentUser.Name;
                currentUser.PartnerName = possibleMatch.Name;
                currentUser.Interest = interests;
                _db.Connections.Update(possibleMatch);
                _db.Connections.Update(currentUser);
                _db.SaveChanges();
                await FinishGreet(possibleMatch, currentUser);
                await FinishGreet(currentUser, possibleMatch);
            }
            else if (possibleMatch != null && currentUser == null)
            {//If there is an error and the searcher is not in the database
                Message output = new Message();
                output.connectionID = Context.ConnectionId;
                output.name = "You";
                output.message = "Critical Error: Current connection is not found in database...";
                await Clients.Client(Context.ConnectionId).SendAsync("DenialMessage", output);
            }
            else
            {//Will start the process of making the searcher watch their connection until someone 'claims' them
                currentUser.Interest = interests;
                currentUser.SearchType = chatType;
                _db.Connections.Update(currentUser);
                _db.SaveChanges();
                Message output = new Message();
                output.connectionID = Context.ConnectionId;
                output.name = "You";
                output.message = "Search submitted. Please wait to be connected or search for another interest...";
                await Clients.Client(Context.ConnectionId).SendAsync("DenialMessage", output);
                return;
            }
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
