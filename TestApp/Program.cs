using TestApp.Tests;

namespace TestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            IfTests.Run();
            MathTests.Run();
            ControlFlowTests.Run();
            ObjectTests.Run();
            NumberTests.Run();
            StringTests.Run();
            ReflectionTests.Run();
            ArrayTests.Run();
            ListTests.Run();
            DictionaryTests.Run();
            ArrayTests.Run();

            TestController.TestsComplete();
        }
    }
}
