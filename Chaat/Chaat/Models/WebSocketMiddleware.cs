using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Chaat.Models.Users;
using Microsoft.AspNetCore.Identity;
using Chaat.Connection;
using Chaat.Data;

namespace Chaat.Models
{
    public partial class WebSocketMiddleware
    {
        private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        private readonly RequestDelegate _next;
        private ClockThread thread;
        private readonly ApplicationDbContext _context;
        private HttpContext httpContext;
        private ConcurrentDictionary<string, User> users;
        private ConcurrentDictionary<string, User> usersOnline = new ConcurrentDictionary<string, User>();
        private ConcurrentDictionary<string, Conversation> conversations = new ConcurrentDictionary<string, Conversation>();
        private bool threadOnlineActive;                                                                                                //CZY WĄTEK SPRAWDZANIA USEROW ONLINE JUZ WYSTARTOWAŁ
        public WebSocketMiddleware(RequestDelegate next, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _next = next;
            _context = context;         
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }
            httpContext = context;
            if (!threadOnlineActive)
            {
                await CheckOnlineUsers(context);                                                                    //URUCHOM WĄTEK SPRAWDZAJĄCY USEROW ONLINE
                threadOnlineActive = true;
            }


            CancellationToken ct = context.RequestAborted;
            WebSocket currentSocket = await context.WebSockets.AcceptWebSocketAsync();

            var socketId = UserID(context);
            WebSocket value = null;
            _sockets.TryRemove(socketId, out value);                                                                //Sockety sie nie resetuja, więc trzeba usunąć stary socket
            _sockets.TryAdd(socketId, currentSocket);
            await AddUserToActiveUsers(socketId);                                                                   //DODANIE UŻYTKOWNIKA DO USEROW ONLINE

            //PĘTLA ACTIVE SOCKET
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                var response = await ReceiveStringAsync(currentSocket, ct);

                var message = JsonConvert.DeserializeObject<Connection.Message>(response);
                message.SenderName = users[UserID(context)].Name;
                message.RecieverID = users[message.SocketTo].Id;                                                       //ID OSOBY DO KTOREJ PISZEMY

                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }
                    continue;
                }

                response = JsonConvert.SerializeObject(message);                                                        //ZAMIANA RESPONSE NA JSON

                WebSocket socketTo;                                                                                     //ZMIENA PRZYPISANIA SOCKETA ODBIORCY
                bool errorSend = false;
                try
                {
                    socketTo = _sockets[message.SocketTo];
                }
                catch
                {                    
                    socketTo = null;                                                                                        //UŻYTKOWNIK DO KTOREGO PISZEMY JEST OFFLINE
                }

                if (socketTo != null)
                {
                    if (socketTo.State == WebSocketState.Open)
                    {
                        await SendStringAsync(socketTo, response, ct);                                                      //ODBIORCA ONLINE
                    }
                    else
                    {
                        errorSend = true;
                    }
                }
                else
                {
                    errorSend = true;
                }
                if (errorSend)
                {
                    //METODA ZAPISU DO BAZY DANYCH
                }

                if (socketTo == null || socketTo != currentSocket)
                {
                    //AddMessageToConversation(socketId, message);
                    await SendStringAsync(currentSocket, response, ct);
                }
            }

            WebSocket dummy;
            _sockets.TryRemove(socketId, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }

        //WYŚLIJ WIADOMOŚC
        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }






    }

}
