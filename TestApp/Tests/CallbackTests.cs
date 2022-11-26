using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    /// <summary>
    /// Tests that test Action<> or Func<>
    /// </summary>
    public static class CallbackTests
    {
        public static Action<string> console;
        public static void Run()
        {
            Action<string> action = (string parm) => Console.WriteLine(parm);
            action.Invoke("hi there :)");
            Action<string, string> action2 = (string parm, string b) => Console.WriteLine(parm + b);
            action2.Invoke("hi there ", ":)");
            Action<string, string, string> action3 = (string parm, string b, string c) => Console.WriteLine(parm + b + c);
            action3.Invoke("hi there ", ":", ")");
            Action<string, string, string, string> action4 = (string parm, string b, string c, string d) => Console.WriteLine(parm + b + c + d);
            action4.Invoke("hi there", " ", ":", ")");
            Action<string, string, string, string, int> action5 = (string parm, string b, string c, string d, int f) =>
            {
                Console.WriteLine(parm + b + c + d);
                Console.WriteLine(f);
            };
            action5.Invoke("hi there", " ", ":", ")", 5);
            ActionAsMethodArgTest(new string[] { "hi", "bye" }, action, action);
            ActionAsMethodArgTest2(new string[] { "a", "b" });







            Func<string> func0 = GetTestMessage;
            var val = func0();
            if (val == "Func test")
            {
                TestController.TestSuccess("Func<string> works correctly");
            }
            else
            {
                TestController.TestFail("Func<string> works incorrectly");
            }

            Func<string, string> func1 = GetTestMessage2;
            var val2 = func1("arg");
            if (val2 == "this function has 2 args")
            {
                TestController.TestSuccess("Func<string, string> works correctly");
            }
            else
            {
                TestController.TestFail("Func<string, string> works incorrectly");
            }
        }


        private static string GetTestMessage2(string arg)
        {
            return "this function has 2 " + arg + "s";
        }

        private static string GetTestMessage()
        {
            return "Func test";
        }


        private static void ActionAsMethodArgTest(string[] vs, Action<string> action1, Action<string> action2)
        {
            ActionAsMethodArgTestB(vs, action2, action1);
        }
        private static void ActionAsMethodArgTestB(string[] vs, Action<string> action1, Action<string> action2)
        {
            console = action1;

            console(vs[0]);
            console(vs[1]);
        }
        private static void ActionAsMethodArgTest2(string[] vs)
        {
            console(vs[0]);
            console(vs[1]);
        }
    }
}
