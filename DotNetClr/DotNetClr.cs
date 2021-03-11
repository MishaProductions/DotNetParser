using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;

namespace DotNetClr
{
    public class DotNetClr
    {
        private DotNetFile file;
        private string EXEPath;
        private Dictionary<string, DotNetFile> dlls = new Dictionary<string, DotNetFile>();
        private List<MethodArgStack> stack = new List<MethodArgStack>();
        private int CurrentStackItem = 0;
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

        private void RunMethod(DotNetMethod m, DotNetFile file)
        {
            Console.WriteLine($"[CLR] Running Method: {m.Parrent.NameSpace}.{m.Parrent.Name}.{m.Name}()");

            //Make sure that RVA is not zero. If its zero, than its extern
            if (m.RVA == 0)
            {
                if (m.Name == "WriteLine")
                {
                    Console.WriteLine((string)FirstStackItem());
                    return;
                }
                else if (m.Name == "ClrHello")
                {
                    Console.WriteLine("[CLR] Hello!");
                    return;
                }
                else
                {
                    throw new Exception("Unknown internal method: " + m.Name);
                }
            }
            var code = new IlDecompiler(m).Decompile();
            foreach (var item in code)
            {
                if (item.OpCodeName == "ldstr")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.String, value = (string)item.Operand });
                    CurrentStackItem++;
                }
                else if (item.OpCodeName == "nop")
                {
                    //Don't do anything
                }
                else if (item.OpCodeName == "call")
                {
                    var call = (InlineMethodOperandData)item.Operand;

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
                            return;
                        }
                        if (m2.IsExtern)
                            Console.WriteLine($"method: {m2.Name} is extern");
                        //Call it
                        RunMethod(m2, m.Parrent.File);
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
                            RunMethod(m2, m.Parrent.File);
                        }
                        else
                        {
                            clrError($"Cannot resolve method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}", "System.MethodNotFound");
                            return;
                        }
                    }

                    //Clear the stack
                    stack.Clear();
                    CurrentStackItem = 0;
                }
            }
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
