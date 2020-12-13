using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

using Shared;

namespace Sensor
{
    internal class Program : IProgram
    {
        private readonly ProgramHandler handler;
        private readonly TcpListener listener;
        private Socket? client = null;
        private readonly object clientLocker = new();

        public Program()
        {
            handler = new(this);
            listener = new(IPAddress.Any, 0);
        }

        public void Run()
        {
            listener.Start(1);
            try
            {
                handler.StartListening();
                StartAcceptLoop();
            }
            catch (ExitException) { }
            lock (clientLocker)
            {
                if (client is not null)
                {
                    client.Close();
                    handler.OnDisconnected();
                }
            }
            listener.Stop();
            handler.BeforeExit();
        }

        public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;
        public IPAddress? RemoteAddress
        {
            get
            {
                lock (clientLocker) return (client?.RemoteEndPoint as IPEndPoint)?.Address;
            }
        }
        public object? Remote => client;

        public void Sleep(int milliseconds) => Thread.Sleep(milliseconds);
        public bool Send(dynamic data) => Tcp.Send(client, clientLocker, data);
        public void Disconnect()
        {
            lock (clientLocker) client?.Close();
        }
        public void Exit() => throw new ExitException();

        private void StartAcceptLoop()
        {
            while (true)
            {
                while (!listener.Pending()) Thread.Sleep(10);
                try
                {
                    lock (clientLocker) client = listener.AcceptSocket();
                } catch (SocketException)
                {
                    continue;
                }
                try
                {
                    handler.OnConnected();
                    StartReceiveLoop();
                }
                catch (SocketException)
                {
                    client.Close();
                }
                catch (ObjectDisposedException) { }
                lock (clientLocker) client = null;
                handler.OnDisconnected();
            }
        }
        private void StartReceiveLoop()
        {
            while (true)
            {
                var data = Tcp.Receive(client!);
#pragma warning disable IDE0058 // 永远不会使用表达式值
                handler.OnReceived(data);
#pragma warning restore IDE0058 // 永远不会使用表达式值
            }
        }
    }

    internal class ExitException : Exception { }
}
