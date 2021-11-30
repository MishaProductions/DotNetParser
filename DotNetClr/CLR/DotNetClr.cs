using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace libDotNetClr
{
    /// <summary>
    /// DotNetCLR Class
    /// </summary>
    public partial class DotNetClr
    {
        private DotNetFile file;
        private string EXEPath;
        /// <summary>
        /// Is the CLR running?
        /// </summary>
        private bool Running = false;
        /// <summary>
        /// Loaded DLLS
        /// </summary>
        private Dictionary<string, DotNetFile> dlls = new Dictionary<string, DotNetFile>();
        /// <summary>
        /// Stack
        /// </summary>
        private CustomList<MethodArgStack> stack = new CustomList<MethodArgStack>();
        /// <summary>
        /// Stack used in stdloc.0 instructions
        /// </summary>
        private MethodArgStack[] Localstack = new MethodArgStack[256];
        /// <summary>
        /// Callstack
        /// </summary>
        private List<CallStackItem> CallStack = new List<CallStackItem>();
        /// <summary>
        /// List of custom internal methods
        /// </summary>
        private Dictionary<string, ClrCustomInternalMethod> CustomInternalMethods = new Dictionary<string, ClrCustomInternalMethod>();
        public DotNetClr(DotNetFile exe, string DllPath)
        {
            if (!Directory.Exists(DllPath))
            {
                throw new DirectoryNotFoundException(DllPath);
            }
            if (exe == null)
            {
                throw new ArgumentException(nameof(exe));
            }

            EXEPath = DllPath;
            Init(exe);
        }
        private void Init(DotNetFile p)
        {
            file = p;
            dlls.Clear();
            dlls.Add("main_exe", p);

            RegisterAllInternalMethods();
        }


        /// <summary>
        /// Starts the .NET Executable
        /// </summary>
        public void Start(string[] args = null)
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
            InitAssembly(file, true);
            PrintColor("Jumping to entry point", ConsoleColor.DarkMagenta);
            //stack.Clear(); //make sure we have a fresh stack
            Running = true;

            //Run the entry point
            var Startparams = new CustomList<MethodArgStack>();
            if (args != null)
            {
                MethodArgStack[] itms = new MethodArgStack[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    itms[i] = MethodArgStack.String(args[i]);
                }
                Startparams.Add(new MethodArgStack() { ArrayLen = 1, type = StackItemType.Array, ArrayItems = itms });
            }
            stack.Clear();
            Localstack = new MethodArgStack[256];
            RunMethod(file.EntryPoint, file, Startparams);
        }
        private void InitAssembly(DotNetFile file, bool InitCorLib)
        {
            if (InitCorLib)
                ResolveDLL("mscorlib"); //Always resolve mscorlib, incase the exe uses .net core
            //Resolve all of the DLLS
            foreach (var item in file.Backend.Tabels.AssemblyRefTabel)
            {
                var fileName = file.Backend.ClrStringsStream.GetByOffset(item.Name);
                ResolveDLL(fileName);
            }
            Running = true;
            //Call all static contructors
            foreach (var dll in dlls)
            {
                foreach (var t in dll.Value.Types)
                {
                    foreach (var m in t.Methods)
                    {
                        if (m.Name == ".cctor" && m.IsStatic)
                        {
                            Console.WriteLine("Creating " + t.FullName + "." + m.Name);
                            RunMethod(m, file, stack);
                            //stack.Clear();
                        }
                    }
                }
            }
        }
        private void ResolveDLL(string fileName)
        {
            string fullPath = "";
            if (dlls.ContainsKey(fileName))
            {
                //already loaded
                return;
            }

            if (File.Exists(Path.Combine(EXEPath, fileName + ".exe")))
            {
                fullPath = Path.Combine(EXEPath, fileName + ".exe");
            }
            else if (File.Exists(Path.Combine(EXEPath, fileName + ".dll")))
            {
                fullPath = Path.Combine(EXEPath, fileName + ".dll");
            }
            else if (File.Exists(fileName + ".exe"))
            {
                fullPath = fileName + ".exe";
            }
            else if (File.Exists(fileName + ".dll"))
            {
                fullPath = fileName + ".dll";
            }
            else
            {
                //Console.WriteLine("File: " + fileName + ".dll does not exist in " + EXEPath + "!", "System.FileNotFoundException");
                //Console.WriteLine("DotNetParser will not be stopped.");
                //  return;
            }
            //try
            //{
            if (!string.IsNullOrEmpty(fullPath))
            {
                var file2 = new DotNetFile(fullPath);
                InitAssembly(file2, false);
                dlls.Add(fileName, file2);
                PrintColor("[OK] Loaded assembly: " + fileName, ConsoleColor.Green);
            }

            else
            {
                PrintColor("[ERROR] Load failed: " + fileName, ConsoleColor.Red);
            }
            //}
            // catch (Exception x)
            //{
            //    clrError("File: " + fileName + " has an unknown error in it. The error is: " + x.Message, "System.UnknownClrError");
            //   throw;
            //    return;
            //}
        }
        /// <summary>
        /// Runs a method
        /// </summary>
        /// <param name="m">The method</param>
        /// <param name="file">The file of the method</param>
        /// <param name="oldStack">Old stack</param>
        /// <returns>Returns the return value</returns>
        private MethodArgStack RunMethod(DotNetMethod m, DotNetFile file, CustomList<MethodArgStack> parms, bool addToCallStack = true)
        {
            if (m.Name == ".ctor" && m.Parrent.FullName == "System.Object")
                return null;
            if (!Running)
                return null;
            #region Internal methods
            //Make sure that RVA is not zero. If its zero, than its extern
            if (m.IsInternalCall | m.IsImplementedByRuntime)
            {
                string properName = m.Name;
                if (m.IsImplementedByRuntime)
                {
                    properName = m.Parrent.FullName.Replace(".", "_") + "." + m.Name + "_impl";
                }
                foreach (var item in CustomInternalMethods)
                {
                    if (item.Key == properName)
                    {
                        MethodArgStack a = null;
                        item.Value.Invoke(parms.ToArray(), ref a, m);

                        //Don't forget to remove item parms
                        if (m.AmountOfParms == 0)
                        {
                            //no need to do anything
                        }
                        else
                        {
                            int StartParmIndex = -1;
                            int EndParmIndex = -1;
                            int paramsRead = 0;
                            bool foundEndIndex = false;
                            if (m.AmountOfParms > 0)
                            {
                                var endParam = m.SignatureInfo.Params[m.SignatureInfo.Params.Count - 1];
                                for (int i4 = stack.Count - 1; i4 >= 0; i4--)
                                {
                                    var stackitm = stack[i4];
                                    if (stackitm.type == endParam.type | stackitm.type == StackItemType.ldnull && StartParmIndex == -1 && !foundEndIndex)
                                    {
                                        if (endParam.IsClass)
                                        {
                                            if (endParam.ClassType != stackitm.ObjectType)
                                            {
                                                continue;
                                            }
                                        }
                                        EndParmIndex = i4;
                                        foundEndIndex = true;
                                        if (m.AmountOfParms == 1)
                                        {
                                            StartParmIndex = i4;
                                            break;
                                        }
                                    }
                                    if (EndParmIndex != -1 && StartParmIndex == -1)
                                    {
                                        paramsRead++;
                                    }
                                    if (EndParmIndex != -1 && paramsRead >= m.AmountOfParms)
                                    {
                                        StartParmIndex = i4;
                                        break;
                                    }
                                }
                            }
                            if (StartParmIndex == -1)
                            {
                                if (m.AmountOfParms == 1)
                                    return a;
                                else
                                {
                                    //clrError("Failed to find parameters after exectuting an internal method!", "internal CLR error");
                                    return a;
                                }
                            }

                            if (m.AmountOfParms == 1)
                            {
                                stack.RemoveAt(StartParmIndex);
                            }
                            else
                            {
                                stack.RemoveRange(StartParmIndex, EndParmIndex - StartParmIndex + 1);
                            }
                        }


                        if (m.Name.Contains("ToString"))
                        {
                            stack.RemoveAt(stack.Count - 1);
                        }
                        return a;
                    }
                }

                clrError("Cannot find internal method: " + properName, "internal CLR error");
                return null;
            }
            else if (m.RVA == 0)
            {
                clrError($"Cannot find the method body for {m.Parrent.FullName}.{m.Name}", "System.Exception");
                return null;
            }
            #endregion
            if (addToCallStack)
            {
                //Add this method to the callstack.
                CallStack.Add(new CallStackItem() { method = m });
            }

            //Now decompile the code and run it
            var decompiler = new IlDecompiler(m);
            foreach (var item in dlls)
            {
                decompiler.AddRefernce(item.Value);
            }
            var code = decompiler.Decompile();
            int i;
            for (i = 0; i < code.Length; i++)
            {
                if (!Running)
                    return null;
                var item = code[i];
                #region Ldloc / stloc
                if (item.OpCodeName == "stloc.s")
                {
                    if (ThrowIfStackIsZero(stack, "stloc.s")) return null;

                    var oldItem = stack[stack.Count - 1];
                    Localstack[(byte)item.Operand] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "stloc.0")
                {
                    if (ThrowIfStackIsZero(stack, "stloc.0")) return null;

                    var oldItem = stack[stack.Count - 1];
                    Localstack[0] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "stloc.1")
                {
                    if (ThrowIfStackIsZero(stack, "stloc.1")) return null;
                    var oldItem = stack[stack.Count - 1];

                    Localstack[1] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "stloc.2")
                {
                    if (ThrowIfStackIsZero(stack, "stloc.2")) return null;
                    var oldItem = stack[stack.Count - 1];

                    Localstack[2] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "stloc.3")
                {
                    if (ThrowIfStackIsZero(stack, "stloc.3")) return null;
                    var oldItem = stack[stack.Count - 1];

                    Localstack[3] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldloc.s")
                {
                    var oldItem = Localstack[(byte)item.Operand];
                    stack.Add(oldItem);
                }
                else if (item.OpCodeName == "ldloca.s")
                {
                    var oldItem = Localstack[(byte)item.Operand];
                    stack.Add(oldItem);
                }
                else if (item.OpCodeName == "ldloc.0")
                {
                    var oldItem = Localstack[0];
                    stack.Add(oldItem);
                    // Localstack[0] = null;
                }
                else if (item.OpCodeName == "ldloc.1")
                {
                    var oldItem = Localstack[1];
                    stack.Add(oldItem);

                    //Localstack[1] = null;
                }
                else if (item.OpCodeName == "ldloc.2")
                {
                    var oldItem = Localstack[2];
                    stack.Add(oldItem);
                    //Localstack[2] = null;
                }
                else if (item.OpCodeName == "ldloc.3")
                {
                    var oldItem = Localstack[3];
                    stack.Add(oldItem);
                    //Localstack[3] = null;
                }
                #endregion
                #region ldc* opcodes
                //Push int32
                else if (item.OpCodeName == "ldc.i4")
                {
                    //Puts an int32 onto the arg stack
                    stack.Add(MethodArgStack.Int32((int)item.Operand));
                }
                else if (item.OpCodeName == "ldc.i4.0")
                {
                    //Puts an 0 onto the arg stack
                    stack.Add(MethodArgStack.Int32(0));
                }
                else if (item.OpCodeName == "ldc.i4.1")
                {
                    //Puts an int32 with value 1 onto the arg stack
                    stack.Add(MethodArgStack.Int32(1));
                }
                else if (item.OpCodeName == "ldc.i4.2")
                {
                    //Puts an int32 with value 2 onto the arg stack
                    stack.Add(MethodArgStack.Int32(2));
                }
                else if (item.OpCodeName == "ldc.i4.3")
                {
                    //Puts an int32 with value 3 onto the arg stack
                    stack.Add(MethodArgStack.Int32(3));
                }
                else if (item.OpCodeName == "ldc.i4.4")
                {
                    //Puts an int32 with value 4 onto the arg stack
                    stack.Add(MethodArgStack.Int32(4));
                }
                else if (item.OpCodeName == "ldc.i4.5")
                {
                    //Puts an int32 with value 5 onto the arg stack
                    stack.Add(MethodArgStack.Int32(5));
                }
                else if (item.OpCodeName == "ldc.i4.6")
                {
                    //Puts an int32 with value 6 onto the arg stack
                    stack.Add(MethodArgStack.Int32(6));
                }
                else if (item.OpCodeName == "ldc.i4.7")
                {
                    //Puts an int32 with value 7 onto the arg stack
                    stack.Add(MethodArgStack.Int32(7));
                }
                else if (item.OpCodeName == "ldc.i4.8")
                {
                    //Puts an int32 with value 8 onto the arg stack
                    stack.Add(MethodArgStack.Int32(8));
                }
                else if (item.OpCodeName == "ldc.i4.m1")
                {
                    //Puts an int32 with value -1 onto the arg stack
                    stack.Add(MethodArgStack.Int32(-1));
                }
                else if (item.OpCodeName == "ldc.i4.s")
                {
                    //Push an int32 onto the stack
                    stack.Add(MethodArgStack.Int32((int)(sbyte)(byte)item.Operand));
                }
                //Push int64
                else if (item.OpCodeName == "ldc.i8")
                {
                    stack.Add(MethodArgStack.Int64((long)item.Operand));
                }
                //push float64
                else if (item.OpCodeName == "ldc.r4")
                {
                    //Puts an float32 with value onto the arg stack
                    stack.Add(MethodArgStack.Float32((float)item.Operand));
                }
                //Push float64
                else if (item.OpCodeName == "ldc.r8")
                {
                    //Puts an float32 with value onto the arg stack
                    if (item.Operand is float)
                    {
                        stack.Add(MethodArgStack.Float64((float)item.Operand));
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                #endregion
                #region conv* opcodes
                else if (item.OpCodeName == "conv.i4")
                {
                    if (ThrowIfStackIsZero(stack, "conv.i4")) return null;

                    var numb = stack[stack.Count - 1];
                    if (numb.type == StackItemType.Int32)
                    {
                        //We don't need to do anything because it's already int32
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                #endregion
                #region Math
                else if (item.OpCodeName == "add")
                {
                    var a = stack[stack.Count - 2];
                    var b = stack[stack.Count - 1];
                    if (a.type != StackItemType.Int32)
                    {
                        clrError("invaild datas a", "fatal stack fault");
                        return null;
                    }
                    if (b.type != StackItemType.Int32)
                    {
                        clrError("invaild datas b", "fatal stack fault");
                        return null;
                    }

                    var numb1 = (int)a.value;
                    var numb2 = (int)b.value;
                    var result = numb1 + numb2;
                    stack.RemoveRange(stack.Count - 2, 2);
                    stack.Add(MethodArgStack.Int32(result));
                }
                else if (item.OpCodeName == "sub")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    var result = numb1 - numb2;
                    stack.RemoveRange(stack.Count - 2, 2);
                    stack.Add(MethodArgStack.Int32(result));
                }
                else if (item.OpCodeName == "div")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;

                    //TODO: Check if dividing by zero
                    var result = numb1 / numb2;
                    stack.RemoveRange(stack.Count - 2, 2);
                    stack.Add(MethodArgStack.Int32(result));
                }
                else if (item.OpCodeName == "mul")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    var result = numb1 * numb2;
                    stack.RemoveRange(stack.Count - 2, 2);
                    stack.Add(MethodArgStack.Int32(result));
                }
                else if (item.OpCodeName == "ceq")
                {
                    if (stack.Count < 2)
                        throw new Exception("There has to be 2 or more items on the stack for ceq instruction to work!");
                    var numb1 = stack[stack.Count - 2].value;
                    var numb2 = stack[stack.Count - 1].value;
                    int Numb1;
                    int Numb2;

                    if (numb1 is int)
                    {
                        Numb1 = (int)numb1;
                    }
                    else if (numb1 is char)
                    {
                        Numb1 = (int)(char)numb1;
                    }
                    else
                    {
                        clrError("Do not know where to branch, as the stack is corrupt", "Internal CLR error");
                        return null;
                    }

                    if (numb2 is int)
                    {
                        Numb2 = (int)numb2;
                    }
                    else if (numb2 is char)
                    {
                        Numb2 = (int)(char)numb2;
                    }
                    else
                    {
                        clrError("Do not know where to branch, as the stack is corrupt", "Internal CLR error");
                        return null;
                    }

                    stack.RemoveRange(stack.Count - 2, 2);
                    ;
                    if (Numb1 == Numb2)
                    {
                        //push 1
                        stack.Add(MethodArgStack.Int32(1));
                    }
                    else
                    {
                        //push 0
                        stack.Add(MethodArgStack.Int32(0));
                    }
                }
                else if (item.OpCodeName == "cgt")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    stack.RemoveRange(stack.Count - 2, 2);
                    if (numb1 > numb2)
                    {
                        //push 1
                        stack.Add(MethodArgStack.Int32(1));
                    }
                    else
                    {
                        //push 0
                        stack.Add(MethodArgStack.Int32(0));
                    }
                }
                else if (item.OpCodeName == "clt")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    stack.RemoveRange(stack.Count - 2, 2);
                    if (numb1 < numb2)
                    {
                        //push 1
                        stack.Add(MethodArgStack.Int32(1));
                    }
                    else
                    {
                        //push 0
                        stack.Add(MethodArgStack.Int32(0));
                    }
                }
                #endregion
                #region Branch instructions
                else if (item.OpCodeName == "br.s")
                {
                    //find the ILInstruction that is in this position
                    int i2 = item.Position + (int)item.Operand + 1;
                    ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                    if (inst == null)
                        throw new Exception("Attempt to branch to null");
                    i = inst.RelPosition - 1;
                }
                else if (item.OpCodeName == "brfalse.s")
                {
                    if (stack.Count == 0)
                    {
                        clrError("Do not know if I should branch, because there is nothing on the stack. Instruction: brfalse.s", "Internal");
                        return null;
                    }
                    var s = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                    bool exec = false;
                    if (s.value == null)
                        exec = true;
                    else
                    {
                        try
                        {
                            if ((int)s.value == 0)
                                exec = true;
                        }
                        catch { }
                    }
                    if (exec)
                    {
                        // find the ILInstruction that is in this position
                        int i2 = item.Position + (int)item.Operand + 1;
                        ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                        if (inst == null)
                            throw new Exception("Attempt to branch to null");
                        i = inst.RelPosition - 1;
                    }
                }
                else if (item.OpCodeName == "brtrue.s")
                {
                    if (stack[stack.Count - 1].value == null)
                        continue;
                    bool exec = false;
                    if (stack[stack.Count - 1].type != StackItemType.Int32)
                    {
                        if (stack[stack.Count - 1].type != StackItemType.ldnull)
                        {
                            exec = true;
                        }
                    }
                    else
                    {
                        if ((int)stack[stack.Count - 1].value == 1)
                        {
                            exec = true;
                        }
                    }
                    if (exec)
                    {
                        // find the ILInstruction that is in this position
                        int i2 = item.Position + (int)item.Operand + 1;
                        ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                        if (inst == null)
                            throw new Exception("Attempt to branch to null");
                        stack.RemoveAt(stack.Count - 1);
                        i = inst.RelPosition - 1;
                    }
                    else
                    {
                        stack.RemoveAt(stack.Count - 1);
                    }
                }
                #endregion
                #region Misc
                else if (item.OpCodeName == "ldstr")
                {
                    stack.Add(MethodArgStack.String((string)item.Operand));
                }
                else if (item.OpCodeName == "nop")
                {
                    //Don't do anything
                }
                else if (item.OpCodeName == "conv.i8")
                {
                    var itm = stack[stack.Count - 1];
                    if (itm.type == StackItemType.Int32)
                    {
                        itm.value = (long)(int)itm.value;
                        itm.type = StackItemType.Int64;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    stack.RemoveAt(stack.Count - 1);
                    stack.Add(itm);
                }
                else if (item.OpCodeName == "ldsfld")
                {
                    //get value from static field
                    DotNetField f2 = null;
                    FieldInfo info = item.Operand as FieldInfo;
                    foreach (var f in m.Parrent.File.Types)
                    {
                        foreach (var tttt in f.Fields)
                        {
                            if (tttt.IndexInTabel == info.IndexInTabel && tttt.Name == info.Name)
                            {
                                f2 = tttt;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(info.Class) && f2 == null)
                    {
                        throw new Exception("Could not find the field");
                    }
                    if (f2 == null)
                    {
                        foreach (var dll in dlls)
                        {
                            foreach (var type in dll.Value.Types)
                            {
                                foreach (var f in type.Fields)
                                {
                                    if (f.Name == info.Name && type.Name == info.Class && type.NameSpace == info.Namespace)
                                    {
                                        f2 = f;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (f2 == null)
                        throw new Exception("Cannot find the static field to read from.");

                    StaticField f3 = null;
                    foreach (var f in StaticFieldHolder.staticFields)
                    {
                        if (f.theField.Name == f2.Name && f.theField.ParrentType.FullName == f2.ParrentType.FullName)
                        {
                            f3 = f;
                            break;
                        }
                    }
                    if (f3 == null)
                    {
                        PrintColor("WARN: Cannot find static field value. Static field name: " + f2.ToString(), ConsoleColor.Yellow);
                        stack.Add(MethodArgStack.ldnull);
                    }
                    else
                    {
                        stack.Add(f3.value);
                    }
                }
                else if (item.OpCodeName == "stsfld")
                {
                    FieldInfo info = item.Operand as FieldInfo;
                    //write value to field.
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.File.Types)
                    {
                        foreach (var tttt in f.Fields)
                        {
                            if (tttt.IndexInTabel == info.IndexInTabel && tttt.Name == info.Name)
                            {
                                f2 = tttt;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(info.Class) && f2 == null)
                    {
                        throw new Exception("Could not find the field");
                    }
                    if (f2 == null)
                    {
                        foreach (var dll in dlls)
                        {
                            foreach (var type in dll.Value.Types)
                            {
                                foreach (var f in type.Fields)
                                {
                                    if (f.Name == info.Name && type.Name == info.Class && type.NameSpace == info.Namespace)
                                    {
                                        f2 = f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (f2 == null)
                    {
                        throw new InvalidOperationException("Cannot find the field to write to.");
                    }
                    StaticField f3 = null;
                    foreach (var f in StaticFieldHolder.staticFields)
                    {
                        if (f.theField.Name == f2.Name && f.theField.ParrentType.FullName == f2.ParrentType.FullName)
                        {
                            f3 = f;

                            f.value = stack[stack.Count - 1];
                            break;
                        }
                    }

                    if (f3 == null)
                    {
                        //create field
                        StaticFieldHolder.staticFields.Add(new StaticField() { theField = f2, value = stack[stack.Count - 1] });
                    }
                    if (f2 == null)
                        throw new Exception("Cannot find the field.");
                    f2.Value = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "call")
                {
                    var call = (InlineMethodOperandData)item.Operand;
                    if (!InternalCallMethod(call, m, addToCallStack, false, false))
                    {
                        return null;
                    }
                }
                else if (item.OpCodeName == "ldnull")
                {
                    stack.Add(MethodArgStack.Null());
                }
                else if (item.OpCodeName == "throw")
                {
                    //Throw Exception
                    var exp = stack[stack.Count - 1];

                    if (exp.type == StackItemType.ldnull)
                    {
                        clrError("Object reference not set to an instance of an object.", "System.NullReferenceException");
                        return null;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (item.OpCodeName == "ret")
                {
                    //Return from function
                    MethodArgStack a = null;
                    if (stack.Count != 0 && m.HasReturnValue)
                    {
                        a = stack[stack.Count - 1];
                        if (addToCallStack)
                            CallStack.RemoveAt(CallStack.Count - 1);


                        stack.RemoveAt(stack.Count - 1);
                    }

                    return a;
                }
                else if (item.OpCodeName == "newobj")
                {
                    var call = (InlineMethodOperandData)item.Operand;
                    //Local/Defined method
                    DotNetMethod m2 = null;
                    foreach (var item2 in dlls)
                    {
                        foreach (var item3 in item2.Value.Types)
                        {
                            foreach (var meth in item3.Methods)
                            {
                                if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && item3.Name == call.ClassName)
                                {
                                    if (call.ParamListIndex != 0)
                                    {
                                        if (meth.ParamListIndex == call.ParamListIndex)
                                        {
                                            m2 = meth;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        m2 = meth;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (m2 == null)
                    {
                        clrError($"Cannot find the called constructor: {call.NameSpace}.{call.ClassName}.{call.FunctionName}(). Function signature is {call.Signature}", "CLR internal error");
                        return null;
                    }

                    MethodArgStack a = new MethodArgStack() { ObjectContructor = m2, ObjectType = m2.Parrent, type = StackItemType.Object, value = new ObjectValueHolder() };

                    //init fields
                    foreach (var f in m2.Parrent.Fields)
                    {
                        var h = a.value as ObjectValueHolder;
                        switch (f.ValueType.type)
                        {
                            case StackItemType.String:
                                h.Fields.Add(f.Name, MethodArgStack.Null());
                                break;
                            case StackItemType.Int32:
                                h.Fields.Add(f.Name, MethodArgStack.Int32(0));
                                break;
                            case StackItemType.Int64:
                                h.Fields.Add(f.Name, MethodArgStack.Int64(0));
                                break;
                            case StackItemType.Float32:
                                h.Fields.Add(f.Name, MethodArgStack.Float32(0));
                                break;
                            case StackItemType.Float64:
                                h.Fields.Add(f.Name, MethodArgStack.Float64(0));
                                break;
                            case StackItemType.Object:
                                h.Fields.Add(f.Name, MethodArgStack.Null());
                                break;
                            case StackItemType.Array:
                                h.Fields.Add(f.Name, MethodArgStack.Null());
                                break;
                            default:
                                throw new NotImplementedException();
                                break;
                        }
                    }



                    //Call the contructor
                    if (!InternalCallMethod(call, m, true, true, true, a))
                    {
                        Console.WriteLine("Error occured, returning null.");
                        return null; //error occured
                    }

                    stack.Add(a);
                }
                else if (item.OpCodeName == "stfld")
                {
                    //write value to field.
                    FieldInfo info = item.Operand as FieldInfo;
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.File.Types)
                    {
                        foreach (var tttt in f.Fields)
                        {
                            if (tttt.IndexInTabel == info.IndexInTabel && tttt.Name == info.Name)
                            {
                                f2 = tttt;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(info.Class) && f2 == null)
                    {
                        throw new Exception("Could not find the field");
                    }
                    if (f2 == null)
                    {
                        foreach (var dll in dlls)
                        {
                            foreach (var type in dll.Value.Types)
                            {
                                foreach (var f in type.Fields)
                                {
                                    if (f.Name == info.Name && type.Name == info.Class && type.NameSpace == info.Namespace)
                                    {
                                        f2 = f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (f2 == null)
                    {
                        clrError("Failed to resolve field for writing.", "");
                        return null;
                    }
                    MethodArgStack obj = null;
                    //TODO: do we need to do this? Probably not
                    foreach (var t in stack)
                    {
                        if (t.type == StackItemType.Object)
                        {
                            if (t.ObjectType == f2.ParrentType)
                            {
                                obj = t;
                                break;
                            }
                        }
                    }
                    if (obj == null)
                    {
                        clrError("Failed to find correct type in the stack", "");
                        return null;
                    }

                    var data = (ObjectValueHolder)obj.value;
                    if (data.Fields.ContainsKey(f2.Name))
                    {
                        data.Fields[f2.Name] = stack[stack.Count - 1];
                    }
                    else
                    {
                        data.Fields.Add(f2.Name, stack[stack.Count - 1]);
                    }
                    obj.value = data;
                    stack[0] = obj;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldfld")
                {
                    //read value from field.
                    FieldInfo info = item.Operand as FieldInfo;
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.File.Types)
                    {
                        foreach (var tttt in f.Fields)
                        {
                            if (tttt.IndexInTabel == info.IndexInTabel && tttt.Name == info.Name)
                            {
                                f2 = tttt;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(info.Class) && f2 == null)
                    {
                        throw new Exception("Could not find the field");
                    }
                    if (f2 == null)
                    {
                        foreach (var dll in dlls)
                        {
                            foreach (var type in dll.Value.Types)
                            {
                                foreach (var f in type.Fields)
                                {
                                    if (f.Name == info.Name && type.Name == info.Class && type.NameSpace == info.Namespace)
                                    {
                                        f2 = f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (f2 == null)
                    {
                        clrError("Failed to resolve field for writing.", "");
                        return null;
                    }
                    MethodArgStack obj = null;
                    //TODO: do we need to do this? Probably not
                    for (int i6 = stack.Count - 1; i6 >= 0; i6--)
                    {
                        var itm = stack[i6];
                        if (itm.type == StackItemType.Object && itm.ObjectType == f2.ParrentType)
                        {
                            obj = itm;
                            break;
                        }
                    }

                    if (obj == null)
                    {
                        clrError("Object to read from not found!", "CLR internal error");
                        return null;
                    }

                    var data = (ObjectValueHolder)obj.value;
                    if (data.Fields.ContainsKey(f2.Name))
                    {
                        stack.Add(data.Fields[f2.Name]);
                    }
                    else
                    {
                        clrError("Reading from non existant field on object " + obj.ObjectType.FullName + ", field name is " + f2.Name, "CLR internal error");
                    }
                }
                else if (item.OpCodeName == "ldarg.0")
                {
                    if (parms.Count == 0)
                        continue;
                    stack.Add(parms[0]);
                }
                else if (item.OpCodeName == "ldarg.1")
                {
                    if (parms.Count == 0)
                        continue;

                    stack.Add(parms[1]);
                }
                else if (item.OpCodeName == "ldarg.2")
                {
                    if (parms.Count == 0)
                        continue;

                    stack.Add(parms[2]);
                }
                else if (item.OpCodeName == "ldarg.3")
                {
                    if (parms.Count == 0)
                        continue;

                    stack.Add(parms[3]);
                }
                else if (item.OpCodeName == "ldarg.s")
                {
                    if (parms.Count == 0)
                        continue;

                    stack.Add(parms[(byte)item.Operand]);
                }
                else if (item.OpCodeName == "callvirt")
                {
                    var call = (InlineMethodOperandData)item.Operand;
                    if (!InternalCallMethod(call, m, addToCallStack, true, false))
                    {
                        return null;
                    }
                }
                #endregion
                #region Arrays
                else if (item.OpCodeName == "newarr")
                {
                    var arrayLen = stack[stack.Count - 1];
                    var array = Arrays.AllocArray((int)arrayLen.value);

                    stack.RemoveAt(stack.Count - 1);
                    stack.Add(new MethodArgStack() { type = StackItemType.Array, value = array.Index});
                }
                else if (item.OpCodeName == "ldlen")
                {
                    MethodArgStack array = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                    stack.Add(MethodArgStack.Int32(Arrays.ArrayRefs[Arrays.GetIndexFromRef(array)].Length));
                }
                else if (item.OpCodeName == "dup")
                {
                    stack.Add(stack[stack.Count - 1]);
                }
                else if (item.OpCodeName == "pop")
                {
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldelem.ref")
                {
                    var index = stack[stack.Count - 1];
                    var array = stack[stack.Count - 2];
                    if (array.type != StackItemType.Array)
                    {
                        clrError("Expected array, but got something else. Fault instruction name: ldelem.ref", "Internal CLR error"); return null;
                    }
                    if (index.type != StackItemType.Int32)
                    {
                        clrError("Expected Int32, but got something else. Fault instruction name: ldelem.ref", "Internal CLR error");
                        return null;
                    }
                    var idx = (int)index.value;
                    stack.RemoveAt(stack.Count - 1); //Remove index
                    stack.RemoveAt(stack.Count - 1); //Remove array

                    stack.Add(Arrays.ArrayRefs[Arrays.GetIndexFromRef(array)].Items[idx]);
                }
                else if (item.OpCodeName == "stelem.ref")
                {
                    var val = stack[stack.Count - 1];
                    var index = stack[stack.Count - 2];
                    var array = stack[stack.Count - 3];
                    //if (array.type != StackItemType.Array) { clrError("Expected array, but got something else. Fault instruction name: stelem.ref", "Internal CLR error"); return null; }
                    if (index.type != StackItemType.Int32) { clrError("Expected Int32, but got something else. Fault instruction name: stelem.ref", "Internal CLR error"); return null; }

                    Arrays.ArrayRefs[Arrays.GetIndexFromRef(array)].Items[(int)index.value] = val;

                    stack.RemoveAt(stack.Count - 1); //Remove value
                    stack.RemoveAt(stack.Count - 1); //Remove index
                    stack.RemoveAt(stack.Count - 1); //Remove array ref
                }
                else if (item.OpCodeName == "stelem")
                {
                    var val = stack[stack.Count - 1];
                    var index = stack[stack.Count - 2];
                    var array = stack[stack.Count - 3];
                    //if (array.type != StackItemType.Array) { clrError("Expected array, but got something else. Fault instruction name: stelem", "Internal CLR error"); return null; }
                    array.ArrayItems[(int)index.value] = val;

                    stack.RemoveAt(stack.Count - 1); //Remove value
                }
                #endregion
                #region Reflection
                else if (item.OpCodeName == "ldtoken")
                {
                    var index = (int)item.Operand & 0xFF;
                    if (index >= file.Backend.Tabels.TypeRefTabel.Count)
                    {
                        index = file.Backend.Tabels.TypeRefTabel.Count;
                    }
                    var typeRef = file.Backend.Tabels.TypeRefTabel[index - 1];
                    var name = file.Backend.ClrStringsStream.GetByOffset(typeRef.TypeName);
                    var Typenamespace = file.Backend.ClrStringsStream.GetByOffset(typeRef.TypeNamespace);
                    bool found = false;
                    foreach (var dll in dlls)
                    {
                        foreach (var t in dll.Value.Types)
                        {
                            if (t.Name == name && t.NameSpace == Typenamespace)
                            {
                                var handle = CreateType("System", "RuntimeTypeHandle");
                                WriteStringToType(handle, "_name", t.Name);
                                WriteStringToType(handle, "_namespace", t.NameSpace);
                                stack.Add(handle);
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        clrError("Unable to resolve type: " + Typenamespace + "." + name, "CLR internal error");
                        return null;
                    }
                }
                else if (item.OpCodeName == "ldftn")
                {
                    var call = item.Operand as InlineMethodOperandData; //a method
                    var obj2 = stack[stack.Count - 1]; //the compiler generated object to call the method on
                    DotNetMethod m2 = null;

                    if (call.RVA == 0) throw new NotImplementedException();
                    else
                    {
                        foreach (var item2 in dlls)
                        {
                            foreach (var item3 in item2.Value.Types)
                            {
                                foreach (var meth in item3.Methods)
                                {
                                    var fullName = call.NameSpace + "." + call.ClassName;
                                    if (string.IsNullOrEmpty(call.NameSpace))
                                        fullName = call.ClassName;

                                    if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && meth.Parrent.FullName == fullName)
                                    {
                                        m2 = meth;
                                        break;
                                    }
                                }
                            }
                        }

                        if (m2 == null)
                        {
                            clrError($"Cannot resolve virtual called method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}(). Function signature is {call.Signature}", "");
                            return null;
                        }
                    }

                    var ptr = CreateType("System", "IntPtr");
                    (ptr.value as ObjectValueHolder).Fields.Add("PtrToMethod", new MethodArgStack() { value = m2, type = StackItemType.MethodPtr });
                    stack.RemoveAt(stack.Count - 1); //remove the object
                    stack.Add(ptr);
                }
                else if (item.OpCodeName == "leave.s")
                {
                    // find the ILInstruction that is in this position
                    int i2 = item.Position + (int)item.Operand + 1;
                    ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                    if (inst == null)
                        throw new Exception("Attempt to branch to null");
                    stack.RemoveAt(stack.Count - 1);
                    i = inst.RelPosition - 1;
                }
                else if (item.OpCodeName == "stind.i4")
                {
                    var val = stack[stack.Count - 1];
                    var ptr = stack[stack.Count - 2];
                    if (ptr.type != StackItemType.Int32) throw new InvalidOperationException("Invaild pointer!");

                    //Because we ignore the stack after calling the method, and remove everything from it, we need to find the value
                    bool found = false;
                    for (int i7 = stack.Count - 3; i7 >= 0; i7--)
                    {
                        var itm = stack[i7];
                        if (itm.type == val.type && itm.value == ptr.value)
                        {
                            stack[i7] = val;
                            found = true;
                            break;
                        }
                    }
                    stack[stack.Count - 2] = val; //just in case
                    stack.RemoveAt(stack.Count - 1); //remove value

                    if (!found)
                    {
                        clrError("Failed to find target value of pointer", "internal error clr");
                    }
                }
                #endregion
                else
                {
                    clrError("Unsupported opcode: " + item.OpCodeName, "CLR Internal error");
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns true if there is a problem
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool ThrowIfStackIsZero(CustomList<MethodArgStack> stack, string instruction)
        {
            if (stack.Count == 0)
            {
                clrError("Fatal error: The " + instruction + " requires more than 1 items on the stack.", "Internal");
                return true;
            }
            return false;
        }
        #region Utils
        private void PrintColor(string s, ConsoleColor c)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(s);
            Console.ForegroundColor = old;
        }
        private void clrError(string message, string errorType)
        {
            //Running = false;
            PrintColor($"A {errorType} has occured in {file.Backend.ClrStringsStream.GetByOffset(file.Backend.Tabels.ModuleTabel[0].Name)}. The error is: {message}", ConsoleColor.Red);

            CallStack.Reverse();
            string stackTrace = "";
            foreach (var itm in CallStack)
            {
                stackTrace += "at " + itm.method.Parrent.NameSpace + "." + itm.method.Parrent.Name + "." + itm.method.Name + "()\n";
            }
            if (stackTrace.Length > 0)
            {
                stackTrace = stackTrace.Substring(0, stackTrace.Length - 1); //Remove last \n
                //PrintColor(stackTrace, ConsoleColor.Red);
            }
        }
        public void RunMethodInDLL(string NameSpace, string TypeName, string MethodName)
        {
            stack.Clear();
            foreach (var dll in dlls)
            {
                foreach (var type in dll.Value.Types)
                {
                    foreach (var method in type.Methods)
                    {
                        if (type.NameSpace == NameSpace && type.Name == TypeName && method.Name == MethodName)
                        {
                            RunMethod(method, dll.Value, stack);
                            break;
                        }
                    }
                }
            }
            throw new Exception("Cannot find the method!");
        }

        public bool InternalCallMethod(InlineMethodOperandData call, DotNetMethod m, bool addToCallStack, bool IsVirt, bool isConstructor, MethodArgStack constructorObj = null)
        {
            MethodArgStack returnValue;
            DotNetMethod m2 = null;
            if (call.RVA != 0)
            {
                //Local/Defined method
                foreach (var item2 in dlls)
                {
                    foreach (var item3 in item2.Value.Types)
                    {
                        foreach (var meth in item3.Methods)
                        {
                            var s = call.NameSpace + "." + call.ClassName;
                            if (string.IsNullOrEmpty(call.NameSpace))
                                s = call.ClassName;

                            if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && meth.Parrent.FullName == s)
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
                    return false;
                }
            }
            else
            {
                if (call.NameSpace == "System" && call.ClassName == "Object" && call.FunctionName == ".ctor")
                {
                    return true; //Ignore
                }
                //Attempt to resolve it
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
                if (m2 == null)
                {
                    clrError($"Cannot resolve method: {call.NameSpace}.{call.ClassName}.{call.FunctionName}. Method signature is {call.Signature}", "System.MethodNotFound");
                    return false;
                }
            }
            //Extract the params
            int StartParmIndex = stack.Count - m2.AmountOfParms;
            int EndParmIndex = stack.Count - 1;
            if (m2.AmountOfParms == 0)
            {
                StartParmIndex = -1;
                EndParmIndex = -1;
            }
            if ((EndParmIndex - StartParmIndex + 1) != m2.AmountOfParms && StartParmIndex != -1 && m2.AmountOfParms != 1)
            {
                throw new InvalidOperationException("Fatal error: an attempt was made to read " + (EndParmIndex - StartParmIndex + 1) + " parameters before calling the method, but it only needs " + m2.AmountOfParms);
            }
            CustomList<MethodArgStack> newParms = new CustomList<MethodArgStack>();
            //Find the object that we are calling it on (if any)
            MethodArgStack objectToCallOn = null;
            if (!isConstructor)
            {
                if (m2.AmountOfParms > 0)
                {
                    var idx = stack.Count - m2.AmountOfParms - 1;
                    if (idx >= 0)
                    {
                        objectToCallOn = stack[idx];
                        //TODO: remove this hack
                        if (objectToCallOn.type != StackItemType.Object && idx != 0 && !IsSpecialType(objectToCallOn, m2))
                        {
                            objectToCallOn = stack[idx - 1];
                        }
                    }
                }
                else
                {
                    //find the object
                    for (int x = stack.Count - 1; x >= 0; x--)
                    {
                        var itm = stack[x];
                        if (itm.type == StackItemType.Object)
                        {
                            if (itm.ObjectType == m2.Parrent)
                            {
                                objectToCallOn = itm;
                                break;
                            }
                        }
                    }
                }
            }
            if (objectToCallOn == null && IsVirt && !isConstructor)
            {
                //Try to find it
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    var itm = stack[i];
                    if (itm.type == StackItemType.Object)
                    {
                        if (itm.ObjectType == m2.Parrent)
                        {
                            objectToCallOn = itm;
                            break;
                        }
                    }
                }
            }
            if (objectToCallOn != null)
            {
                if (objectToCallOn.type == StackItemType.Object | IsSpecialType(objectToCallOn, m2))
                {
                    if (StartParmIndex == -1)
                    {
                        newParms.Add(objectToCallOn);
                    }
                    else
                    {
                        if (objectToCallOn.ObjectType == m2.Parrent | objectToCallOn.ObjectType == null)
                        {
                            newParms.Add(objectToCallOn);
                        }
                        else
                        {
                            //if (m2.Parrent == objectToCallOn.ObjectType)
                            // newParms.Add(objectToCallOn);
                        }
                    }
                }
            }
            if (isConstructor)
            {
                newParms.Add(constructorObj);
            }
            if (m2.AmountOfParms == 0)
            {
                StartParmIndex = -1;
                EndParmIndex = -1;
            }
            if (StartParmIndex != -1)
            {
                for (int i5 = StartParmIndex; i5 < EndParmIndex + 1; i5++)
                {
                    var itm5 = stack[i5];
                    newParms.Add(itm5);
                }
                if (StartParmIndex == 1 && EndParmIndex == 1)
                {
                    newParms.Add(stack[StartParmIndex]);
                }
            }
            if (StartParmIndex == 0 && EndParmIndex == 0 && m2.AmountOfParms == 1)
            {
                newParms.Add(stack[0]);
            }

            //Call it
            var oldStack = stack.backend.GetRange(0, stack.Count);
            returnValue = RunMethod(m2, m.Parrent.File, newParms, addToCallStack);
            stack.backend = oldStack;

            //Remove the parameters once we are finished
            if (StartParmIndex != -1 && EndParmIndex != -1)
            {
                stack.RemoveRange(StartParmIndex, EndParmIndex - StartParmIndex+1);
            }
            if (returnValue != null)
            {
                stack.Add(returnValue);
            }
            return true;
        }

        private bool IsSpecialType(MethodArgStack obj, DotNetMethod m)
        {
            if (m.Parrent.FullName == "System.String" && obj.type == StackItemType.String)
            {
                return true;
            }
            if (m.Parrent.FullName == "System.IntPtr" && obj.type == StackItemType.IntPtr)
            {
                return true;
            }
            return false;
        }
        #endregion
    }

    internal static class Arrays
    {
        public static ArrayRef[] ArrayRefs = new ArrayRef[100];
        private static int CurrentIndex = 0;
        public static int GetIndexFromRef(MethodArgStack r)
        {
            return (int)r.value;
        }
        public static ArrayRef AllocArray(int arrayLen)
        {
            ArrayRefs[CurrentIndex] = new ArrayRef();
            ArrayRefs[CurrentIndex].Length = arrayLen;
            ArrayRefs[CurrentIndex].Items = new MethodArgStack[arrayLen];
            ArrayRefs[CurrentIndex].Index = CurrentIndex;
            CurrentIndex++;
            return ArrayRefs[CurrentIndex-1];
        }
    }
    internal class ArrayRef
    {
        public MethodArgStack[] Items;
        public int Length;
        public int Index { get; internal set; }
    }
}
