using System.IO;
using System.Net;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sensor.Test
{
    [TestClass]
    public class TestGenerate
    {
        [TestMethod]
        public void Test()
        {
            var program = new Program();
            var handler = new ProgramHandler(program);
            handler.Generate(null);
            program.CloseWriter();
        }
    }

    internal class Program : IProgram
    {
        private readonly StreamWriter writer;

        public Program()
        {
            writer = new("test.txt");
        }

        public void Sleep(int milliseconds) { }

        public bool Send(dynamic data)
        {
            var a = ((double)data).ToString();
            writer.WriteLine(a);
            return true;
        }

        public void CloseWriter() => writer.Close();
        public object Remote => null;

        public int Port => throw new System.NotImplementedException();
        public IPAddress RemoteAddress => throw new System.NotImplementedException();
        public void Disconnect() {}
        public void Exit() => throw new System.NotImplementedException();
    }
}
