using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Shared;

namespace Manager
{
    partial class Program
    {
        private Socket? socket = null;
        private readonly object socketLocker = new();

        public IPEndPoint? RemoteEndPoint
        {
            get
            {
                lock (socketLocker)
                {
                    return socket?.RemoteEndPoint as IPEndPoint;
                }
            }
        }

        public bool Connect(IPEndPoint endPoint)
        {
            lock (socketLocker)
            {
                if (socket is not null) return false;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(endPoint);
                }
                catch (SocketException)
                {
                    socket.Close();
                    socket = null;
                    return false;
                }
            }
            _ = Task.Run(StartLoop);
            return true;
        }
        public void Disconnect()
        {
            lock (socketLocker) socket?.Close();
        }
        public bool Send(dynamic data) => Tcp.Send(socket, socketLocker, data);

        private void StartLoop()
        {
            var socket = this.socket!;
            handler.OnConnected();
            while (true)
            {
                dynamic data;
                try
                {
                    data = Tcp.Receive(socket);
                }
                catch (SocketException)
                {
                    socket.Close();
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
#pragma warning disable IDE0058 // 永远不会使用表达式值
                handler.OnReceived(data);
#pragma warning restore IDE0058 // 永远不会使用表达式值
            }
            lock (socketLocker) this.socket = null;
            handler.OnDisconnected();
        }
    }
}