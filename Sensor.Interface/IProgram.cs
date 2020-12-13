using System;
using System.Net;

namespace Sensor
{
    /// <summary>
    /// 包含与本程序有关的信息与操作。
    /// </summary>
    /// <remarks>该接口的所有方法和属性都是线程安全的。</remarks>
    public interface IProgram
    {
        /// <summary>
        /// 获取本程序监听的端口。
        /// </summary>
        /// <remarks>若未处于监听状态，则返回0。</remarks>
        int Port { get; }

        /// <summary>
        /// 获取客户端的IP地址。
        /// </summary>
        /// <remarks>若没有与客户端连接，则返回null。</remarks>
        IPAddress? RemoteAddress { get; }

        /// <summary>
        /// 获取与本程序连接的远程主机的对象。
        /// </summary>
        /// <remarks>若没有与客户端连接，则返回null。</remarks>
        object? Remote { get; }

        /// <summary>
        /// 使主线程休眠指定的时间。
        /// </summary>
        /// <param name="milliseconds">主线程休眠的毫秒数。</param>
        void Sleep(int milliseconds);

        /// <summary>
        /// 若与远程主机有连接，则向远程主机发送数据。
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <returns>是否成功发送</returns>
        /// <exception cref="OutOfMemoryException">发送的数据量超过1KB。</exception>
        bool Send(dynamic data);

        /// <summary>
        /// 若与远程主机有连接，则断开与远程主机的连接。
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 开始结束程序的流程。
        /// </summary>
        void Exit();
    }
}
