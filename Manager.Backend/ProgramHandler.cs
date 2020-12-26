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
            program.ExecuteJs("setContent('已建立连接')");
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
            string D = data;
            string year = D.Substring(0, 3);
            string month = D.Substring(4, 5);
            string day = D.Substring(6,7);
            string hour = D.Substring(8,9);
            string minute = D.Substring(10, 11);
            int temp = Convert.ToInt32(D.Substring(13, 14));
            string Bool = D.Substring(12, 12);
            if (Bool == "-")
            {
                temp = 0 - temp;
            }
            Sql.Execute("INSERT INTO data(year,month,day,hour,minute,temperature)  VALUES(@0,@1,@2,@3,@4,@5)",year, month, day, hour, minute, temp);
            program.ExecuteJs($"setContent('{data}')");
        }
    }
}
