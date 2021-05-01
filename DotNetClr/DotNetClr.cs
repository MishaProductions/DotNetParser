//#define CLR_DEBUG
using LibDotNetParser;
using LibDotNetParser.CILApi;
using LibDotNetParser.CILApi.IL;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetClr
{
    /// <summary>
    /// DotNetCLR Class
    /// </summary>
    public class DotNetClr
    {
        private DotNetFile file;
        private string EXEPath;
        private Dictionary<string, DotNetFile> dlls = new Dictionary<string, DotNetFile>();
        private List<MethodArgStack> stack = new List<MethodArgStack>();
        private MethodArgStack[] Localstack = new MethodArgStack[256];
        private bool Running = false;
        private List<CallStackItem> CallStack = new List<CallStackItem>();
        public DotNetClr(DotNetFile exe, string DllPath)
        {
            if (!Directory.Exists(DllPath))
            {
                throw new DirectoryNotFoundException(DllPath);
            }
            EXEPath = DllPath;
            Init(exe);
        }
        private void Init(DotNetFile p)
        {
            file = p;
            dlls.Clear();
            dlls.Add("main_exe", p);
        }
        /// <summary>
        /// Starts the .NET Executable
        /// </summary>
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
#if CLR_DEBUG
                Console.WriteLine("[CLR] Loading: " + Path.GetFileName(fullPath));
#endif
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
            //Call contructor on main type
            foreach (var item in file.EntryPointType.Methods)
            {
                if (item.Name == ".cctor")
                {
                    RunMethod(item, file);
                    break;
                }
            }



            //Run the entry point
            RunMethod(file.EntryPoint, file);
        }

        private MethodArgStack RunMethod(DotNetMethod m, DotNetFile file)
        {
            if (m.Name == ".ctor" && m.Parrent.FullName == "System.Object")
                return null;
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

            #region Internal methods
            //Make sure that RVA is not zero. If its zero, than its extern
            if (m.RVA == 0)
            {
                if (m.Name == "WriteLine")
                {
                    if (stack.Count == 0)
                    {
                        Console.WriteLine();
                        return null;
                    }
                    var s = stack[stack.Count-1];
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
                else if (m.Name == "op_Equality")
                {
                    if ((string)stack[0].value == (string)stack[1].value)
                    {
                        return new MethodArgStack() { type = StackItemType.Int32, value = 1 };
                    }
                    else
                    {
                        return new MethodArgStack() { type = StackItemType.Int32, value = 0 };
                    }
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
            #endregion
            //Add this method to the callstack.
            CallStack.Add(new CallStackItem() { method = m });

            //Now decompile the code and run it
            var decompiler = new IlDecompiler(m);
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
                    var oldItem = stack[stack.Count - 1];
                    Localstack[(byte)item.Operand] = oldItem;
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
                else if (item.OpCodeName == "stloc.0")
                {
                    var oldItem = stack[stack.Count - 1];
                    Localstack[0] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldloc.0")
                {
                    var oldItem = Localstack[0];
                    stack.Add(oldItem);
                    //Localstack[0] = null;
                }
                else if (item.OpCodeName == "stloc.1")
                {
                    var oldItem = stack[stack.Count - 1];

                    Localstack[1] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldloc.1")
                {
                    var oldItem = Localstack[1];
                    stack.Add(oldItem);

                    // Localstack[1] = null;
                }
                else if (item.OpCodeName == "stloc.2")
                {
                    var oldItem = stack[stack.Count - 1];

                    Localstack[2] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
                }
                else if (item.OpCodeName == "ldloc.2")
                {
                    var oldItem = Localstack[2];
                    stack.Add(oldItem);
                    //Localstack[2] = null;
                }
                else if (item.OpCodeName == "stloc.3")
                {
                    var oldItem = stack[stack.Count - 1];

                    Localstack[3] = oldItem;
                    stack.RemoveAt(stack.Count - 1);
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
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)item.Operand });
                }
                else if (item.OpCodeName == "ldc.i4.0")
                {
                    //Puts an 0 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)0 });
                }
                else if (item.OpCodeName == "ldc.i4.1")
                {
                    //Puts an int32 with value 1 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)1 });
                }
                else if (item.OpCodeName == "ldc.i4.2")
                {
                    //Puts an int32 with value 2 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)2 });
                }
                else if (item.OpCodeName == "ldc.i4.3")
                {
                    //Puts an int32 with value 3 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)3 });
                }
                else if (item.OpCodeName == "ldc.i4.4")
                {
                    //Puts an int32 with value 4 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)4 });
                }
                else if (item.OpCodeName == "ldc.i4.5")
                {
                    //Puts an int32 with value 5 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)5 });
                }
                else if (item.OpCodeName == "ldc.i4.6")
                {
                    //Puts an int32 with value 6 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)6 });
                }
                else if (item.OpCodeName == "ldc.i4.7")
                {
                    //Puts an int32 with value 7 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)7 });
                }
                else if (item.OpCodeName == "ldc.i4.8")
                {
                    //Puts an int32 with value 3 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)7 });
                }
                else if (item.OpCodeName == "ldc.i4.m1")
                {
                    //Puts an int32 with value -1 onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)-1 });
                }
                else if (item.OpCodeName == "ldc.i4.s")
                {
                    //Push an int32 onto the stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)(byte)item.Operand });
                }
                //Push int64
                else if (item.OpCodeName == "ldc.i8")
                {
                    stack.Add(new MethodArgStack() { type = StackItemType.Int64, value = (long)item.Operand });
                }
                //push float64
                else if (item.OpCodeName == "ldc.r4")
                {
                    //Puts an float32 with value onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Float32, value = (float)item.Operand });
                }
                //Push float64
                else if (item.OpCodeName == "ldc.r8")
                {
                    //Puts an float32 with value onto the arg stack
                    stack.Add(new MethodArgStack() { type = StackItemType.Float64, value = (float)item.Operand });
                }
                #endregion
                #region Math
                else if (item.OpCodeName == "add")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    var result = numb1 + numb2;
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = result });
                }
                else if (item.OpCodeName == "sub")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    var result = numb1 - numb2;
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = result });
                }
                else if (item.OpCodeName == "div")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;

                    //TODO: Check if dividing by zero
                    var result = numb1 / numb2;
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = result });
                }
                else if (item.OpCodeName == "mul")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    var result = numb1 * numb2;
                    stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = result });
                }
                else if (item.OpCodeName == "ceq")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    if (numb1 == numb2)
                    {
                        //push 1
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)1 });
                    }
                    else
                    {
                        //push 0
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)0 });
                    }
                }
                else if (item.OpCodeName == "cgt")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    if (numb1 > numb2)
                    {
                        //push 1
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)1 });
                    }
                    else
                    {
                        //push 0
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)0 });
                    }
                }
                else if (item.OpCodeName == "clt")
                {
                    var numb1 = (int)stack[stack.Count - 2].value;
                    var numb2 = (int)stack[stack.Count - 1].value;
                    if (numb1 < numb2)
                    {
                        //push 1
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)1 });
                    }
                    else
                    {
                        //push 0
                        stack.Add(new MethodArgStack() { type = StackItemType.Int32, value = (int)0 });
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


#if CLR_DEBUG
                    Console.WriteLine("branching to: IL_" + inst.Position + ": " + inst.OpCodeName);
#endif
                    i = inst.RelPosition - 1;
                }
                else if (item.OpCodeName == "brfalse.s")
                {
                    if ((int)stack[stack.Count - 1].value == 0)
                    {
                        // find the ILInstruction that is in this position
                        int i2 = item.Position + (int)item.Operand + 1;
                        ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                        if (inst == null)
                            throw new Exception("Attempt to branch to null");
                        stack.Clear();
                        i = inst.RelPosition - 1;
#if CLR_DEBUG
                    Console.WriteLine("branching to: IL_" + inst.Position + ": " + inst.OpCodeName+" because item on stack is false.");
#endif
                    }
                    else
                    {
                        stack.Clear();
                    }
                }
                else if (item.OpCodeName == "brtrue.s")
                {
                    if ((int)stack[stack.Count - 1].value == 1)
                    {
                        // find the ILInstruction that is in this position
                        int i2 = item.Position + (int)item.Operand + 1;
                        ILInstruction inst = decompiler.GetInstructionAtOffset(i2, -1);

                        if (inst == null)
                            throw new Exception("Attempt to branch to null");
                        stack.Clear();
                        i = inst.RelPosition - 1;
#if CLR_DEBUG
                    Console.WriteLine("branching to: IL_" + inst.Position + ": " + inst.OpCodeName+" because item on stack is true.");
#endif
                    }
                    else
                    {
                        stack.Clear();
                    }
                }
                #endregion
                #region Misc
                else if (item.OpCodeName == "ldstr")
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
                else if (item.OpCodeName == "ldsfld")
                {
                    //get value from feild
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.Fields)
                    {
                        if (f.IndexInTabel == (int)(byte)item.Operand)
                        {
                            f2 = f;
                            break;
                        }
                    }

                    if (f2 == null)
                        throw new Exception("Cannot find the field.");

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
                        Running = false;
                        string stackTrace = "";
                        CallStack.Reverse();
                        foreach (var itm in CallStack)
                        {
                            stackTrace += itm.method.Parrent.NameSpace + "." + itm.method.Parrent.Name + "." + itm.method.Name + "()\n";
                        }
                        clrError("Attempt to push null onto the stack.", "System.NullReferenceException", stackTrace);
                        return null;
                    }
                    stack.Add(f3.value);
                }
                else if (item.OpCodeName == "stsfld")
                {
                    //write value to field.
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.Fields)
                    {
                        if (f.IndexInTabel == (int)(byte)item.Operand)
                        {
                            f2 = f;
                            break;
                        }
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
                                    if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && meth.Parrent.FullName == call.NameSpace + "." + call.ClassName)
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

                    if (m.AmountOfParms != 0)
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
                            stackTrace += itm.method.Parrent.NameSpace + "." + itm.method.Parrent.Name + "." + itm.method.Name + "()\n";
                        }
                        clrError("Null.", "System.NullRefrenceException", stackTrace);
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
#if CLR_DEBUG
                    Console.WriteLine("[CLR] Returning from function");
#endif
                    ;
                    //Successful return
                    CallStack.RemoveAt(CallStack.Count - 1);

                    if (stack.Count != 0)
                        return stack[0];
                    else return null;
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
                                if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && meth.Parrent.FullName == call.NameSpace + "." + call.ClassName)
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

                    MethodArgStack a = new MethodArgStack() { ObjectContructor = m2, ObjectType = m2.Parrent, type = StackItemType.Object, value = new ObjectValueHolder() };
                    stack.Add(a);
                    //Call the contructor
                    RunMethod(m2, m.Parrent.File);
                }
                else if (item.OpCodeName == "stfld")
                {
                    //write value to field.
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.Fields)
                    {
                        if (f.IndexInTabel == (byte)item.Operand)
                        {
                            f2 = f;
                            break;
                        }
                    }
                    if (f2 == null)
                    {
                        //Resolve recursively
                        foreach (var type in file.Types)
                        {
                            foreach (var field in type.Fields)
                            {
                                if (field.IndexInTabel == (byte)item.Operand)
                                {
                                    f2 = field;
                                    break;
                                }
                            }
                        }
                    }
                    var obj = stack[0];
                    if (obj.type != StackItemType.Object) throw new InvalidOperationException();

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
                }
                else if (item.OpCodeName == "ldfld")
                {
                    //write value to field.
                    DotNetField f2 = null;
                    foreach (var f in m.Parrent.Fields)
                    {
                        if (f.IndexInTabel == (byte)item.Operand)
                        {
                            f2 = f;
                            break;
                        }
                    }
                    if (f2 == null)
                    {
                        //Resolve recursively
                        foreach (var type in file.Types)
                        {
                            foreach (var field in type.Fields)
                            {
                                if (field.IndexInTabel == (byte)item.Operand)
                                {
                                    f2 = field;
                                    break;
                                }
                            }
                        }
                    }
                    var obj = stack[0];
                    if (obj.type != StackItemType.Object) throw new InvalidOperationException();

                    var data = (ObjectValueHolder)obj.value;
                    if (data.Fields.ContainsKey(f2.Name))
                    {
                        stack.Add(data.Fields[f2.Name]);
                    }
                    else
                    {
                        throw new Exception("Attempt to read from a nonexistent or null field.");
                    }
                }
                else if (item.OpCodeName == "ldarg.0")
                {
                    //TODO
                }
                else if (item.OpCodeName == "callvirt")
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
                                if (meth.RVA == call.RVA && meth.Name == call.FunctionName && meth.Signature == call.Signature && meth.Parrent.FullName == call.NameSpace + "." + call.ClassName)
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
                    RunMethod(m2,m2.File);
                    ;
                }
                #endregion
                else
                {
                    Running = false;
                    PrintColor("Unsupported OpCode: " + item.OpCodeName, ConsoleColor.Red);
                    PrintColor("Application Terminated.", ConsoleColor.Red);
                    CallStack.Reverse();
                    string stackTrace = "";
                    foreach (var itm in CallStack)
                    {
                        stackTrace += "At " + itm.method.Parrent.NameSpace + "." + itm.method.Parrent.Name + "." + itm.method.Name + "()\n";
                    }
                    PrintColor(stackTrace, ConsoleColor.Red);
                    return null;
                }
            }
            return null;
        }
        #region Utils
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
        #endregion
    }
}
