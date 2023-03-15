using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ClientCommands
{
   public class ClientServerCommand
    {
        Socket client { get; set; }
        Socket server { get; set; }

       public string _answer = "";
       public string error = "";
       byte[] buffer = new byte[1024];
        public bool ConnectServer(IPEndPoint point, Socket clientSocket)
        {
            try
            {
                client = clientSocket;
                client.BeginConnect(point, (IAsyncResult result) =>
                {
                    server = (Socket)result.AsyncState;
                    if (server.Connected)
                    {
                        server.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveMessage, server);
                        /* ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
                         Task<int> message = server.ReceiveAsync(segment, SocketFlags.None);
                        if(message.IsCompleted)
                         {
                             _answer += Encoding.UTF8.GetString(segment.ToArray());
                         }*/
                    }
                }, client);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return _answer.StartsWith("Connected") ? true : false;
        }
        public void SendMessage(string message)
        {
            if (server.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
                client.SendAsync(segment, SocketFlags.None);
            }
        }
        public bool ServerIsConnected()
        {
            return server != null ? true : false;
        }
        void ReceiveMessage(IAsyncResult result)
        {
            Socket server = (Socket)result.AsyncState;
            server.EndReceive(result);
            _answer = Encoding.UTF8.GetString(buffer);
        }
    }
}