using LibDotNetParser;
using LibDotNetParser.CILApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace libDotNetClr
{
    public partial class DotNetClr
    {
        private void RegisterAllInternalMethods()
        {
            //Register internal methods
            RegisterCustomInternalMethod("WriteLine", InternalMethod_Console_Writeline);
            RegisterCustomInternalMethod("Write", InternalMethod_Console_Write);
            RegisterCustomInternalMethod("Clear", InternalMethod_Console_Clear);
            RegisterCustomInternalMethod("Concat", InternalMethod_String_Concat);
            RegisterCustomInternalMethod("Internal__System_Byte_ToString", InternalMethod_Byte_ToString);
            RegisterCustomInternalMethod("Internal__System_SByte_ToString", Internal__System_SByte_ToString);
            RegisterCustomInternalMethod("Internal__System_UInt16_ToString", Internal__System_UInt16_ToString);
            RegisterCustomInternalMethod("Internal__System_Int16_ToString", Internal__System_Int16_ToString);
            RegisterCustomInternalMethod("Internal__System_Int32_ToString", Internal__System_Int32_ToString);
            RegisterCustomInternalMethod("Internal__System_UInt32_ToString", Internal__System_UInt32_ToString);
            RegisterCustomInternalMethod("Internal__System_Char_ToString", Internal__System_Char_ToString);


            RegisterCustomInternalMethod("op_Equality", InternalMethod_String_op_Equality);
            RegisterCustomInternalMethod("DebuggerBreak", DebuggerBreak);
            RegisterCustomInternalMethod("strLen", Internal__System_String_Get_Length);
            RegisterCustomInternalMethod("String_get_Chars_1", Internal__System_String_get_Chars_1);
            RegisterCustomInternalMethod("GetObjType", GetObjType);
            RegisterCustomInternalMethod("Type_FromRefernce", GetTypeFromRefrence);
            RegisterCustomInternalMethod("GetAssemblyFromType", GetAssemblyFromType);
            RegisterCustomInternalMethod("InternalAddItemToList", ListAddItem);
            //RegisterCustomInternalMethod("String_ToUpper", String_ToUpper);

            RegisterCustomInternalMethod("System_Action..ctor_impl", ActionCtorImpl);
            for (int i = 1; i < 10; i++)
            {
                RegisterCustomInternalMethod($"System_Action`{i}..ctor_impl", ActionCtorImpl);
            }

            RegisterCustomInternalMethod("System_Action.Invoke_impl", ActionInvokeImpl);
            RegisterCustomInternalMethod("System_Action`1.Invoke_impl", Action1InvokeImpl);
            RegisterCustomInternalMethod("System_Action`2.Invoke_impl", Action2InvokeImpl);
            RegisterCustomInternalMethod("System_Action`3.Invoke_impl", Action3InvokeImpl);
            RegisterCustomInternalMethod("System_Action`4.Invoke_impl", Action4InvokeImpl);
            RegisterCustomInternalMethod("System_Action`5.Invoke_impl", Action5InvokeImpl);
            RegisterCustomInternalMethod("Boolean_GetValue", Boolean_GetValue);
        }

        private void Boolean_GetValue(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var val = stack[stack.Count - 1];
            returnValue = val;
            stack.RemoveAt(stack.Count - 1);
        }
        #region Actions
        private void Action5InvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            MethodArgStack obj = Stack[0];
            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.Object) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)(((ObjectValueHolder)toCall.value).Fields["PtrToMethod"].value);
            var parms = new CustomList<MethodArgStack>();
            parms.Add(obj); //Is this needed?
            parms.Add(Stack[1]);
            parms.Add(Stack[2]);
            parms.Add(Stack[3]);
            parms.Add(Stack[4]);
            parms.Add(Stack[5]);
            RunMethod(toCallMethod, toCallMethod.File, parms);
            stack.RemoveRange(stack.Count - 5, 5);
        }
        private void Action4InvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            MethodArgStack obj = Stack[0];
            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.Object) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)(((ObjectValueHolder)toCall.value).Fields["PtrToMethod"].value);
            var parms = new CustomList<MethodArgStack>();
            parms.Add(obj); //Is this needed?
            parms.Add(Stack[1]);
            parms.Add(Stack[2]);
            parms.Add(Stack[3]);
            parms.Add(Stack[4]);
            RunMethod(toCallMethod, toCallMethod.File, parms);
            stack.RemoveRange(stack.Count - 4, 4);
        }
        private void Action3InvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            MethodArgStack obj = Stack[0];
            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.Object) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)(((ObjectValueHolder)toCall.value).Fields["PtrToMethod"].value);
            var parms = new CustomList<MethodArgStack>();
            parms.Add(obj); //Is this needed?
            parms.Add(Stack[1]);
            parms.Add(Stack[2]);
            parms.Add(Stack[3]);
            RunMethod(toCallMethod, toCallMethod.File, parms);
            stack.RemoveRange(stack.Count - 3, 3);
        }
        private void Action2InvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            MethodArgStack obj = Stack[0];
            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.Object) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)(((ObjectValueHolder)toCall.value).Fields["PtrToMethod"].value);
            var parms = new CustomList<MethodArgStack>();
            parms.Add(obj); //Is this needed?
            parms.Add(Stack[1]);
            parms.Add(Stack[2]);
            RunMethod(toCallMethod, toCallMethod.File, parms);
            stack.RemoveRange(stack.Count - 2, 2);
        }
        private void Action1InvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            MethodArgStack obj = Stack[0];
            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.Object) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)(((ObjectValueHolder)toCall.value).Fields["PtrToMethod"].value);
            var parms = new CustomList<MethodArgStack>();
            parms.Add(obj); //Is this needed?
            parms.Add(Stack[1]);
            RunMethod(toCallMethod, toCallMethod.File, parms);
            stack.RemoveAt(stack.Count - 1);
        }
        private void ActionInvokeImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var obj = Stack[Stack.Length - 1];
            if (obj.type != StackItemType.Object) throw new InvalidOperationException();

            var d = (ObjectValueHolder)obj.value;
            if (!d.Fields.ContainsKey("__internal_method")) throw new Exception("Invaild instance of Action");
            var toCall = d.Fields["__internal_method"];
            if (toCall.type != StackItemType.MethodPtr) throw new InvalidOperationException();

            var toCallMethod = (DotNetMethod)toCall.value;
            RunMethod(toCallMethod, toCallMethod.File, new CustomList<MethodArgStack>());
        }
        private void ActionCtorImpl(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            // do nothing
            var theAction = Stack[0];
            var obj = Stack[1];
            var methodPtr = Stack[2];
            if (theAction.type != StackItemType.Object) throw new InvalidOperationException();
            if (methodPtr.type != StackItemType.Object) throw new InvalidOperationException();

            //store the method in a secret field
            var d = (ObjectValueHolder)theAction.value;
            d.Fields.Add("__internal_method", methodPtr);
        }
        #endregion
        private void ListAddItem(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var list = Stack[Stack.Length - 2];
            var item = Stack[Stack.Length - 1];

            ;
        }
        #region Reflection
        private void GetAssemblyFromType(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var type = Stack[stack.Count - 1];
            if (type.type != StackItemType.Object) throw new InvalidOperationException();
            var dotNetType = type.ObjectType;

            var assembly = CreateType("System.Reflection", "Assembly");


            returnValue = assembly;
        }
        private void GetTypeFromRefrence(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var re = Stack[Stack.Length - 1];
            if (re.type != StackItemType.Object) throw new InvalidOperationException();

            var type = CreateType("System", "Type");
            var typeToRead = CreateType(ReadStringFromType(re, "_namespace"), ReadStringFromType(re, "_name"));
            WriteStringToType(type, "internal__fullname", typeToRead.ObjectType.FullName);
            returnValue = type;
        }
        private void GetObjType(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var obj = stack[stack.Count - 1];

            //TODO: Remove this hack
            if (obj.type != StackItemType.Object) obj = stack[0];
            if (obj.type != StackItemType.Object) throw new InvalidOperationException();
            //Create the type object

            MethodArgStack a = CreateType("System", "Type");
            WriteStringToType(a, "internal__fullname", obj.ObjectType.FullName);
            returnValue = a;
        }
        #endregion
        #region Making custom internal methods
        /// <summary>
        /// Represents a custom internal method.
        /// </summary>
        /// <param name="Stack">The CLR stack.</param>
        /// <returns>Return value. Return null if function returns void.</returns>
        public delegate void ClrCustomInternalMethod(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method);
        /// <summary>
        /// Registers a custom internal method.
        /// </summary>
        /// <param name="name">The name of the internal method</param>
        /// <param name="method">The method.</param>
        public void RegisterCustomInternalMethod(string name, ClrCustomInternalMethod method)
        {
            if (CustomInternalMethods.ContainsKey(name))
                throw new Exception("Internal method already registered!");

            CustomInternalMethods.Add(name, method);
        }
        #endregion
        #region Implementation for various ToString methods
        private void Internal__System_Char_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var c = stack[stack.Count - 1].value;
            returnValue = new MethodArgStack() { value = (int)c, type = StackItemType.Int32 };
        }

        private void Internal__System_String_get_Chars_1(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = (string)Stack[0].value;
            var index = (int)Stack[1].value;

            returnValue = new MethodArgStack() { type = StackItemType.String, value = str[index] };
        }

        private void Internal__System_String_Get_Length(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var stringToRead = stack[stack.Count - 1];
            var str = (string)stringToRead.value;
            returnValue = new MethodArgStack() { type = StackItemType.Int32, value = str.Length };
        }
        private void Internal__System_UInt32_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((uint)(int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        private void Internal__System_Int32_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        private void Internal__System_Int16_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        private void Internal__System_UInt16_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((ushort)(int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        private void Internal__System_SByte_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((sbyte)(int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        private void InternalMethod_Byte_ToString(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var str = new MethodArgStack();
            str.type = StackItemType.String;
            str.value = ((int)stack[stack.Count - 1].value).ToString();
            returnValue = str;
        }
        #endregion
        #region Console class
        private void InternalMethod_Console_Writeline(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            if (stack.Count == 0)
            {
                Console.WriteLine();
                return;
            }
            var s = stack[stack.Count - 1];
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
        private void InternalMethod_Console_Write(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            if (stack.Count == 0)
                throw new Exception("No items on stack for Console.Write!!");
            var s = stack[stack.Count - 1];
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
        private void InternalMethod_Console_Clear(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            Console.Clear();
        }
        #endregion
        #region String class
        private void InternalMethod_String_Concat(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            string returnVal = "";
            for (int i = Stack.Length - method.AmountOfParms; i < Stack.Length; i++)
            {
                if (Stack[i].type != StackItemType.String)
                {
                    clrError("fatal error see InternalMethod_String_Concat method", "******BIG FATAL ERROR********");
                    return;
                }
                returnVal += (string)Stack[i].value;
            }

            returnValue = MethodArgStack.String((string)returnVal);
        }
        private void InternalMethod_String_op_Equality(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            var a = stack[stack.Count - 2].value;
            var b = stack[stack.Count - 1].value;
            string first;
            string second;
            if (a is string)
            {
                first = (string)a;
            }
            else if (a is int)
            {
                first = ((char)(int)a).ToString();
            }
            else
            {
                returnValue = MethodArgStack.Int32(0);
                return;
            }

            if (b is string)
            {
                second = (string)b;
            }
            else if (b is int)
            {
                second = ((char)(int)b).ToString();
            }
            else
            {
                returnValue = MethodArgStack.Int32(0);
                return;
            }

            if (first == second)
            {
                returnValue = MethodArgStack.Int32(1);
            }
            else
            {
                returnValue = MethodArgStack.Int32(0);
            }
        }
        private void String_ToUpper(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            returnValue = MethodArgStack.String("TODOUPERCASE");
            return;
            var str = Stack[Stack.Length - 1];
            if (str.type != StackItemType.String) throw new InvalidOperationException();
            var oldVal = (string)str.value;
            returnValue = MethodArgStack.String(oldVal.ToUpper());
        }
        #endregion
        #region Misc
        private void DebuggerBreak(MethodArgStack[] Stack, ref MethodArgStack returnValue, DotNetMethod method)
        {
            Debugger.Break();
        }
        #endregion
    }
}
