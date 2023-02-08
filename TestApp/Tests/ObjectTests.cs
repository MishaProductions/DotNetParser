using System;

namespace TestApp.Tests
{
    /// <summary>
    /// Any test that deals with creating objects, reading/writing to fields, or GC goes here
    /// </summary>
    public static class ObjectTests
    {
        public static string TestField = "Default Value.";
        public const string ConstString = "Constant String.";
        public static void Run()
        {
            Console.WriteLine(TestField);

            if (TestField == "Default Value.")
            {
                TestController.TestSuccess("Static field read test success");
            }
            else
            {
                TestController.TestFail("Static field read test failure");
            }

            TestField = "new value";
            if (TestField == "new value")
            {
                TestController.TestSuccess("Static field write test");
            }
            else
            {
                TestController.TestFail("Static field write test failed");
            }

            TestField = "newr value";
            if (TestField == "newr value")
            {
                TestController.TestSuccess("Static field write test (pass 2)");
            }
            else
            {
                TestController.TestFail("Static field write test (pass 2)");
            }


            IHelloWorldFunction f = new HelloWorldFunction();
            f.SayHello();
            Console.WriteLine(f.HelloMessage);

            var r = TestController.TestsRxObject();
            if (r == null)
            {
                TestController.TestFail("TestsRxObject() returned null");
            }
            else
            {
                if (r.TestProperty != "value")
                {
                    TestController.TestFail("TestsRxObject() object has incorrect property value, which is " + r.TestProperty);
                }
                else
                {
                    TestController.TestSuccess("TestsRxObject() object has correct property value");
                }
            }


            Console.WriteLine("Testing subclasses");
            MyObject.SubClass subClass = new MyObject.SubClass();
            subClass.HelloFromSubClass();

            Console.WriteLine("Testing boxing");
            int i = 5;
            object o = i;
            float fbox = 5.3f;
            object fobj = fbox;

            Console.WriteLine("Testing unboxing");
            int i2 = (int)o;
            float f2 = (float)fobj;

            if (i == i2 && fbox == f2) {
                TestController.TestSuccess("Boxing and unboxing test");
            } else {
                TestController.TestFail("Boxing and unboxing test");
            }
        }
    }

    public class TestObject
    {
        public string TestProperty;

        public TestObject(string prop)
        {
            TestProperty = prop;
        }
    }
    public interface IHelloWorldFunction
    {
        void SayHello();
        string HelloMessage { get; set; }
    }
    public class HelloWorldFunction : IHelloWorldFunction
    {
        public string HelloMessage { get => "This is a field on an interface!"; set => throw new NotImplementedException(); }

        public void SayHello()
        {
            Console.WriteLine("Hello world from an interface!");
        }
    }
    class MyObject
    {
        public string WelcomeMessage;
        public MyObject()
        {
            WelcomeMessage = "Second Message";
        }
        public void Hello(string SecondMessage)
        {
            Console.WriteLine(WelcomeMessage);
            Console.WriteLine(SecondMessage);
        }

        public class SubClass
        {
            public string Message;
            public SubClass()
            {
                Message = "Hello from the subclass!";
                Console.WriteLine("SubClass contructor");
            }
            public void HelloFromSubClass()
            {
                Console.WriteLine(Message);
            }
        }
    }
}
