namespace Manager
{
    /// <summary>
    /// 处理网络相关事件
    /// </summary>
    public class ProgramHandler
    {
        private readonly IProgram program;

        public ProgramHandler(IProgram program)
        {
            this.program = program;
        }

        /// <summary>
        /// 在与远程主机建立连接后被调用。
        /// </summary>
        public void OnConnected()
        {

        }

        /// <summary>
        /// 在与远程主机的连接断开后被调用。
        /// </summary>
        public void OnDisconnected()
        {
            program.ExecuteJs("setContent('已断开')");
        }

        /// <summary>
        /// 在接收到远程主机发送的数据后被调用。
        /// </summary>
        /// <param name="data">远程主机发送的数据</param>
        public void OnReceived(dynamic data)
        {
            program.ExecuteJs($"setContent('{data}')");
        }
    }
}
