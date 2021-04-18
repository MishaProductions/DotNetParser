//#define CLR_DEBUG
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DotNetClr
{
    public class DotNetClr
    {
        private DotNetFile file;
        private string EXEPath;
        private Dictionary<string, DotNetFile> dlls = new Dictionary<string, DotNetFile>();
        private List<MethodArgStack> stack = new List<MethodArgStack>();
        private List<MethodArgStack> Localstack = new List<MethodArgStack>();
        private bool Running = false;
        private List<CallStackItem> CallStack = new List<CallStackItem>();
        public DotNetClr(DotNetFile exe, string DllPath)
        {
            if (!Directory.Exists(DllPath))
            {
                throw new DirectoryNotFoundException(DllPath);
            }
            EXEPath = DllPath;
            init(exe);
        }
        private void init(DotNetFile p)
        {
            file = p;
            dlls.Clear();
            dlls.Add("main_exe", p);
        }

        public void Start()
        {
            try
            {
                if (file.EntryPoint == null)
                {
                    clrError("The entry point was not found.", "System.EntryPointNotFoundException");
                    file = null;
                    return;
                }
            }
            catch (Exception x)
            {
                clrError("The entry point was not found. Internal error: " + x.Message, "System.EntryPointNotFoundException");
                file = null;
                return;
            }
            //Resolve all of the DLLS
            foreach (var item in file.Backend.Tabels.AssemblyRefTabel)
            {
                var fileName = file.Backend.ClrStringsStream.GetByOffset(item.Name);
                string fullPath = "";

                if (File.Exists(Path.Combine(EXEPath, fileName + ".exe")))
                {
                    fullPath = Path.Combine(EXEPath, fileName + ".exe");
                }
                else if (File.Exists(Path.Combine(EXEPath, fileName + ".dll")))
                {
                    fullPath = Path.Combine(EXEPath, fileName + ".dll");
                }
                else
                {
                    clrError("File: " + fileName + ".dll does not exist in " + EXEPath + "!", "System.FileNotFoundException");
                    return;
                }
                Console.WriteLine("[CLR] Loading: " + Path.GetFileName(fullPath));
                try
                {
                    dlls.Add(fileName, new DotNetFile(fullPath));
                }
                catch (Exception x)
                {
                    clrError("File: " + fileName + " has an unknown error in it. The error is: " + x.Message, "System.UnknownClrError");
                    return;
                }
            }
            Running = true;
            //Run the entry point
            RunMethod(file.EntryPoint, file);
        }

        private MethodArgStack RunMethod(DotNetMethod m, DotNetFile file)
        {
            if (!Running)
                return null;
#if CLR_DEBUG
            Console.WriteLine("===========================");
            Console.WriteLine($"[CLR] Running Method: {m.Parrent.NameSpace}.{m.Parrent.Name}.{m.Name}()");
            if (stack.Count != 0)
                Console.WriteLine("Following items are on the stack:");
            foreach (var item in stack)
            {
                if (item.type == StackItemType.String)
                {
                    Console.WriteLine("String: \"" + (string)item.value + "\"");
                }
                else if (item.type == StackItemType.Int32)
                {
                    Console.WriteLine("Int32: " + (int)item.value);
                }
            }
            Console.WriteLine("===========================");
            Console.WriteLine("FUNCTION Output");
#endif


            //Make sure that RVA is not zero. If its zero, than its extern
            if (m.RVA == 0)
            {
                if (m.Name == "WriteLine")
                {
                    var s = stack[0];
                    string val = "<NULL>";
                    if (s.type == StackItemType.Int32)
                    {
                        val = ((int)s.value).ToString();
                    }
                    else if (s.type == StackItemType.Int64)
                    {
                        val = ((long)s.value).ToString();
                    }
                    else if (s.type == StackItemType.String)
                    {
                        val = (string)s.value;
                    }
                    Console.WriteLine(val);
                }
                else if (m.Name == "Clear")
                {
                    Console.Clear();
                }
                else if (m.Name == "Write")
                {
                    var s = stack[0];
                    string val = "<NULL>";
                    if (s.type == StackItemType.Int32)
                    {
                        val = ((int)s.value).ToString();
                    }
                    else if (s.type == StackItemType.Int64)
                    {
                        val = ((long)s.value).ToString();
                    }
                    else if (s.type == StackItemType.String)
                    {
                        val = (string)s.value;
                    }
                    Console.Write(val);
                }
                else if (m.Name == "ClrHello")
                {
                    PrintColor("[CLR] Hello!", ConsoleColor.Green);
                }
                else if (m.Name == "ClrConcatString")
                {
                    string returnVal = "";
                    foreach (var item in stack)
                    {
                        if (item.type == StackItemType.String)
                            returnVal += (string)item.value;
                    }

                    return new MethodArgStack() { type = StackItemType.String, value = (string)returnVal };
                }
                else
                {
                    throw new Exception("Unknown internal method: " + m.Name);
                }
                return null;
            }

            CallStack.Add(new CallStackItem() { method = m });

            //Now decompile the code and run it
            var decompiler = new IlDecompiler(m);
            var code = decompiler.Decompile();

            var currentInstruction = -1;
            int i;
            for (i = 0; i < code.Length; i++)
            {
                if (!Running)
                    return null;
                var item = code[i];
                currentInstruction++;
                if (item.OpCodeName == "ldstr")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.String, value = (string)item.Operand });
#if CLR_DEBUG
                    Console.WriteLine("[CLRDEBUG] Pushing: " + (string)item.Operand);
#endif
                }
                else if (item.OpCodeName == "nop")
                {
                    //Don't do anything
                }
                else if (item.OpCodeName == "call")
                {
                    var call = (InlineMethodOperandData)item.Operand;
                    MethodArgStack returnValue;
                    if (call.RVA != 0)
                    {
                        //Local/Defined method
                        DotNetMethod m2 = null;
                        foreach (var item2 in dlls)
                        {
                            foreach (var item3 in item2.Value.Types)
                            {
                                foreach (var meth in item3.Methods)
                                {
                                    if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature)
                                    {
                                        m2 = meth;
                                        break;
                                    }
                                }
                            }
                        }

                        if (m2 == null)
                        {
                            Console.WriteLine($"Cannot resolve called method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}(). Function signature is {call.Signature}");
                            return null;
                        }


                        //Call it
                        returnValue = RunMethod(m2, m.Parrent.File);
                    }
                    else
                    {
                        //Attempt to resolve it
                        DotNetMethod m2 = null;
                        foreach (var item2 in dlls)
                        {
                            foreach (var item3 in item2.Value.Types)
                            {
                                foreach (var meth in item3.Methods)
                                {
                                    if (meth.Name == call.FunctionName && meth.Parrent.Name == call.ClassName && meth.Parrent.NameSpace == call.NameSpace && meth.Signature == call.Signature)
                                    {
                                        m2 = meth;
                                        break;
                                    }
                                }
                            }
                        }

                        if (m2 != null)
                        {
                            //Call it
                            returnValue = RunMethod(m2, m.Parrent.File);
                        }
                        else
                        {
                            clrError($"Cannot resolve method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}. Method signature is {call.Signature}", "System.MethodNotFound");
                            return null;
                        }
                    }

                    //Clear the stack, and if the return value is not null, add it to the stack
                    stack.Clear();
                    if (returnValue != null)
                    {
                        stack.Add(returnValue);
                    }
                }
                else if (item.OpCodeName == "ldnull")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.ldnull, value = null });
                }
                else if (item.OpCodeName == "ret")
                {
                    //Return from function
#if CLR_DEBUG
                    Console.WriteLine("[CLR] Returning from function");
#endif
                    //Successful return
                    CallStack.RemoveAt(CallStack.Count-1);
                    try
                    {
                        return stack[0];
                    }
                    catch
                    {
                        return null;
                    }
                }
                else if (item.OpCodeName == "stloc.0")
                {
                    try
                    {
                        var oldItem = stack[0];
                        Localstack.Add(oldItem);
                        stack.RemoveAt(0);
                        ;
                    }
                    catch
                    {

                    }
                }
                else if (item.OpCodeName == "ldloc.0")
                {
                    try
                    {
                        var oldItem = Localstack[0];
                        stack.Add(oldItem);
                        Localstack.RemoveAt(0);
                    }
                    catch
                    {

                    }

                }
                else if (item.OpCodeName == "ldc.i4")
                {
                    //Puts an int32 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)item.Operand });
                }
                //Push int32
                else if (item.OpCodeName == "ldc.i4.1")
                {
                    //Puts an int32 with value 1 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)1 });
                }
                else if (item.OpCodeName == "ldc.i4.s")
                {
                    //Push an int32 onto the stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)item.Operand });
                }
                //Push int64
                else if (item.OpCodeName == "ldc.i8")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.Int64, value = (long)item.Operand });
                }
                else if (item.OpCodeName == "br.s")
                {
                    //find the ILInstruction that is in this position
                    int i2 = item.Position + (int)item.Operand + 1;
                    ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                    if (inst == null)
                        throw new Exception("Attempt to branch to null");


#if CLR_DEBUG
                    Console.WriteLine("branching to: IL_" + inst.Position + ": " + inst.OpCodeName);
#endif
                    i = inst.RelPosition;
                    continue;
                }
                else if (item.OpCodeName == "ldc.i4.0")
                {
                    //Push 0 as int32 onto the stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)0 });
                }
                else if (item.OpCodeName == "throw")
                {
                    //Throw Exception
                    var exp = stack[0];

                    if (exp.type == StackItemType.ldnull)
                    {
                        string stackTrace = "";
                        CallStack.Reverse();
                        foreach (var itm in CallStack)
                        {
                            stackTrace += itm.method.Parrent.NameSpace + "." + itm.method.Parrent.Name + "." + itm.method.Name+"()\n";
                        }
                        clrError("Null.", "System.NullRefrenceException", stackTrace);
                        return null;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    //#if CLR_DEBUG
                    PrintColor("Unsupported instruction: " + item.OpCodeName, ConsoleColor.Red);
                    PrintColor("Application Terminated.", ConsoleColor.Red);
                    return null;
                    //#endif
                }
            }
            return null;
        }
        private void PrintColor(string s, ConsoleColor c)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(s);
            Console.ForegroundColor = old;
        }

        private void clrError(string message, string errorType, string stackStace = "")
        {
            Running = false;
            PrintColor($"A {errorType} has occured in {file.Backend.ClrStringsStream.GetByOffset(file.Backend.Tabels.ModuleTabel[0].Name)}. The error is: {message}", ConsoleColor.Red);
            PrintColor(stackStace, ConsoleColor.Red);
        }
    }
}
