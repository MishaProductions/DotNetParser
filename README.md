# What is this?
This is a .NET framework EXE parser that parses the IL code and reads .NET Metadata in the executable. It can also run the IL code. This library does not depend on any Windows libraries or .NET Self reflection.
Feel free to contribute.

# Will this execute my .NET Project?
Yes, it will., but only a small amount of .NET Features are working.

# What is working/implemented?
[X] If/while/for statements
[X] Console.Write/Writeline
[X] Classes
[X] Fields
[X] sbyte/byte/ushort/short/int/uint
[X] Adding/Subtracting/Multiplying/Dividing
[X] string.Length
[X] Loading sepreate DLLS

# Building
Set the DotNetParserRunner or TesterKernel project as the startup project in visual studio and click on Run.
