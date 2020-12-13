using System.Net;

namespace Manager
{
    /// <remarks>该类的所有函数均在不同的线程中被调用。</remarks>
    public class JSBinder
    {
        private readonly IProgram program;

        public JSBinder(IProgram program)
        {
            this.program = program;
        }

        public bool Connect(int port) => program.Connect(new(IPAddress.Parse("127.0.0.1"), port));

        public void Disconnect() => program.Disconnect();
    }
}
