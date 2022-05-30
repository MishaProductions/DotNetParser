# DotNetParser
This project allows you to run .NET executables inside of C#.
Discord server: http://discord.gg/SeDrYk79W8

# Will this execute my .NET Project?
Yes, it will. However, only a limited subset of .NET Features are working.

# What is working/implemented?
- [X] if/while/for statements
- [X] Console.Write/Writeline
- [X] Classes
- [X] Fields
- [X] sbyte/byte/ushort/short/int/uint/long/ulong/float/double
- [X] Adding/Subtracting/Multiplying/Dividing/Remainder
- [X] Bitwise Operations (and, or, xor, not, shift left, shift right)
- [X] string.Length
- [X] Loading separate DLLS
- [X] Actions
- [X] Basic generic support
- [X] Really basic reflection
- [X] Sub-classes
- [X] Arrays

# Building
Set the DotNetParserRunner or TesterKernel project as the startup project in Visual Studio and click on Run.

# How to use?
## Basics
### Setting up
First you need to create an instance of `DotNetFile` and pass the byte array with the contents of your C# executable. Starting from some .NET core version, the EXE file was made into a stub which just runs `dotnet app.dll`. You need to use that dll file for it to work.
```csharp
var fl = new DotNetFile(@"c:\app.dll");
```
You can access all of the classes from the Types property in the ``fl`` variable.

Next, create an instance of ``DotNetClr``. This runs the .NET code.
```csharp
var clr = new DotNetClr(fl, @"c:\framework\");
```
`C:\framework` points to a folder where a basic corlib is made. You can find one under the releases.
### Executing
Then you can run the Start method to begin executing the code.
```csharp
clr.Start();
```

## Marshaling
### Calling methods
#### "CLR" side
You can register an "internal" method which can be called from your application which you want to run.

```csharp
clr.RegisterCustomInternalMethod("TestsComplete", TestsComplete);
```

```csharp
private static void TestsComplete(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
{
    Console.WriteLine("All Tests Completed.");
}
```
Inside of the function, the Stack variable represents all of the parameters. returnValue represents the return value of the internal method. Ignore it if the return type is void.

#### Application side
To invoke this newly created method, add this code:
```csharp
TestsComplete();

[MethodImpl(MethodImplOptions.InternalCall)]
public static extern void TestsComplete();
```
### Passing objects from the CLR to the application
#### "CLR" side
```csharp
 clr.RegisterCustomInternalMethod("TestsRxObject", TestRxObject);
 ```
 
 Now the method:
```csharp
private static void TestRxObject(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
{
  var cctor = m.GetMethod("DotNetparserTester", "TestObject", ".ctor");
  if (cctor == null)
      throw new NullReferenceException();
  var s = new CustomList<MethodArgStack>();
  s.Add(MethodArgStack.String("value"));
  returnValue = clr.CreateObject(cctor, s);
}
```
First we get the constructor method (normally named `.ctor`). Then we build the list of parameters and call clr.CreateObject();
#### Application side
```csharp
  public class TestObject
    {
        public string TestProperty;

        public TestObject(string prop)
        {
            TestProperty = prop;
        }
    }
```
```csharp
[MethodImpl(MethodImplOptions.InternalCall)]
public static extern TestObject TestsRxObject();
```
```csharp
var m = TestsRxObject();
```
