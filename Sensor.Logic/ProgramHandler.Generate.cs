namespace Sensor
{
    partial class ProgramHandler
    {
        public void Generate(object remote)
        {
            for (var i = 0; i < 10; i++)
            {
                if (remote != program.Remote) break;
                if (!program.Send(i + 0.1)) break;
                program.Sleep(1000);
            }
            program.Disconnect();
        }
    }
}
