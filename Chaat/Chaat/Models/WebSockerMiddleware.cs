using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Chaat.Models.Users;
using Chaat.Connection;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

//METODY DO WEBSOCKETMIDDLEWEARE
namespace Chaat.Models
{
    public partial class WebSocketMiddleware
    {
        //URUCHOMIENIE WĄTKU SPRAWDZAJĄCEGO KTO JEST ONLINE
        private async Task CheckOnlineUsers(HttpContext context)
        {
            if (users == null)
            {
                var users = await _context.ChatUsers.ToListAsync<User>();
                thread = new ClockThread(CheckWhoIsOnline, true, 1000);
                this.users = new ConcurrentDictionary<string, User>();
                foreach (var user in users)
                {
                    this.users.TryAdd(user.AspUserID, user);
                }
                thread.Start();
            }

        }
        //KTO JEST W TEJ CHWILI ONLINE
        void CheckWhoIsOnline()
        {
            thread.Interval = 5000;
            string json = "";
            User fakeUser;
            foreach (var sckt in _sockets)
            {
                if (sckt.Value.State != WebSocketState.Open)
                {
                    usersOnline.TryRemove(sckt.Key, out fakeUser);
                    continue;
                }
                json += users[sckt.Key].Id + ",";

            }

            string response;
            var message = new Message() { Body = json, PhotoLink = "none", SenderID = -1, SocketTo = "0" };
            response = JsonConvert.SerializeObject(message);

            foreach (var sckt in _sockets)
            {
                var socketState = sckt.Value.State;
                if (sckt.Value.State != WebSocketState.Open)
                {
                    continue;
                }

                SendStringAsync(sckt.Value, response, httpContext.RequestAborted);
            }
        }


        //DODAJ WŁAŚNIE POŁĄCZONEGO USERA DO SŁOWNIKA USEROW ONLINE
        async Task AddUserToActiveUsers(string userID)
        {
            var user = await _context.ChatUsers.SingleOrDefaultAsync(u => u.AspUserID == userID);
            usersOnline.TryAdd(userID, user);
        }

        //SPRAWDŹ ID USERA KTORY SIE POŁĄCZYŁ
        private string UserID(HttpContext context)
        {
            var claimsIdentity = (ClaimsIdentity)context.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }

        void AddMessageToConversation(string senderId, Message message)
        {
            var name = message.SenderID > message.RecieverID ? message.SenderID.ToString() + "_" + message.RecieverID.ToString() : message.RecieverID.ToString() + "_" + message.SenderID.ToString();
            Conversation conversation = null;
            conversations.TryGetValue(name, out conversation);
            if (conversation == null)
            {
                //Zanim doda sprawdź bazę danych czy ma taką konwersacje                
                conversation = new Conversation() { Name = name };
                conversations.TryAdd(name, conversation);
            }
            if (conversation.Messages == null)
            {
                conversation.Messages = new List<Message>();
            }
            conversation.Messages.Add(message);
        }

    }

}
