using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace Shared
{
    internal static class Tcp
    {
        private static readonly byte[] emptyBuffer = Array.Empty<byte>();
        private static readonly byte[] lengthBuffer = new byte[4];

        public static bool Send(Socket socket, object locker, dynamic data)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(data);
            var lengthBytes = BitConverter.GetBytes(bytes.Length);
            lock (locker)
            {
                if (socket is null) return false;
                try
                {
                    _ = socket.Send(lengthBytes);
                    _ = socket.Send(bytes);
                }
                catch (SocketException)
                {
                    return false;
                }
            }
            return true;
        }

        public static dynamic Receive(Socket socket)
        {
            Wait(socket);
            _ = socket.Receive(lengthBuffer);
            var length = BitConverter.ToInt32(lengthBuffer);
            var buffer = new byte[length];
            var totalLength = 0;
            while (totalLength < length)
            {
                Wait(socket);
                totalLength += socket.Receive(buffer, totalLength, length - totalLength, SocketFlags.None);
            }
            return JsonSerializer.Deserialize<dynamic>(buffer)!;
        }

        private static void Wait(Socket socket)
        {
            while (socket.Available == 0)
            {
                _ = socket.Send(emptyBuffer);
                Thread.Sleep(10);
            }
        }
    }
}
