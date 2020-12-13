using System;
using System.Threading.Tasks;

namespace Sensor
{
    /// <summary>
    /// 处理程序的相关事件。
    /// </summary>
    /// <remarks>该文件已有的所有函数均在主线程中被调用。</remarks>
    public partial class ProgramHandler
    {
        private readonly IProgram program;

        public ProgramHandler(IProgram program)
        {
            this.program = program;
        }

        /// <summary>
        /// 在该程序开始监听某个端口上的连接时被调用。
        /// </summary>
        public void StartListening()
        {
            Console.WriteLine("传感器已启动。");
            Console.WriteLine($"正在监听端口{program.Port}上的连接。");
        }

        /// <summary>
        /// 在该程序即将退出时被调用。
        /// </summary>
        public void BeforeExit()
        {

        }

        /// <summary>
        /// 在该程序被某个远程主机连接后被调用。
        /// </summary>
        public void OnConnected()
        {
            Console.WriteLine($"已与IP为{program.RemoteAddress}上的远程主机建立连接。");
            _ = Task.Run(() => Generate(program.Remote));
        }

        /// <summary>
        /// 在该程序与远程主机的连接断开后被调用。
        /// </summary>
        public void OnDisconnected()
        {
            Console.WriteLine("与远程主机的连接已断开。");
        }

        /// <summary>
        /// 在该程序接收到远程主机发送的数据后被调用。
        /// </summary>
        /// <param name="data">远程主机发送的数据</param>
        public void OnReceived(dynamic data)
        {

        }
    }
}
