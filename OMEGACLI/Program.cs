using PowerArgs;

namespace OMEGACLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeAction<BuilderExecutor>(args);
        }
    }
}
