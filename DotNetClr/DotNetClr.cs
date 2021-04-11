#define CLR_DEBUG
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

            //Run the entry point
            RunMethod(file.EntryPoint, file);
        }

        private MethodArgStack RunMethod(DotNetMethod m, DotNetFile file)
        {
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
                    PrintColor(val, ConsoleColor.DarkCyan);
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

            //Now decompile the code and run it
            var code = new IlDecompiler(m).Decompile();

            var currentInstruction = -1;
            int i;
            for (i = 0; i < code.Length; i++)
            {
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
                                    if (meth.RVA == call.RVA && meth.Name == call.FunctionName)
                                    {
                                        m2 = meth;
                                        break;
                                    }
                                }
                            }
                        }

                        if (m2 == null)
                        {
                            Console.WriteLine($"Cannot resolve called method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}()");
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
                                    if (meth.Name == call.FunctionName && meth.Parrent.Name == call.ClassName && meth.Parrent.NameSpace == call.NameSpace)
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
                            clrError($"Cannot resolve method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}", "System.MethodNotFound");
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
                else if (item.OpCodeName == "ret")
                {
                    //Return from function
#if CLR_DEBUG
                    Console.WriteLine("[CLR] Returning from function");
#endif
                    try
                    {
                        return stack[0];
                    }
                    catch
                    {
                        return null;
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
                //Push int64
                else if (item.OpCodeName == "ldc.i8")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.Int64, value = (long)item.Operand });
                }
                else if (item.OpCodeName == "br.s")
                {


                    //find the ILInstruction that is in this position
                    int i2 = i + (byte)item.Operand;
                    //ILInstruction inst=null;
                    //for (i2 = 0; i2 < code.Length; i2++)
                    //{
                    //    var instr = code[i2];

                    //    if (instr.Position == (byte)item.Operand)
                    //    {
                    //        inst = instr;
                    //        break;
                    //    }
                    //}
                    //if (inst == null)
                    //    throw new Exception("Attempt to branch to null");
                    //i2--;
                    Console.WriteLine("branching to: IL_" + i2 + ": " + code[i2].OpCodeName);
                    i = i2;

                    //Debugger.Break();
                }
                else
                {
                    //#if CLR_DEBUG
                    PrintColor("Unsupported instruction: " + item.OpCodeName, ConsoleColor.Red);
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
        private object FirstStackItem()
        {
            try
            {
                return stack[0].value;
            }
            catch
            {
                return "";
            }
        }

        private void clrError(string message, string errorType, string stackStace = "")
        {
            Console.WriteLine($"A {errorType} has occured in {file.Backend.ClrStringsStream.GetByOffset(file.Backend.Tabels.ModuleTabel[0].Name)}. The error is: {message}");
            Console.WriteLine(stackStace);
        }
    }
}
