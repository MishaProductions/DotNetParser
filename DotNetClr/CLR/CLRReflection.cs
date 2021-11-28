using LibDotNetParser;
using LibDotNetParser.CILApi;

namespace libDotNetClr
{
    public partial class DotNetClr
    {
        private MethodArgStack CreateType(string nameSpace, string TypeName)
        {
            DotNetType ttype = null;
            foreach (var dll in dlls)
            {
                foreach (var type in dll.Value.Types)
                {
                    if (type.NameSpace == nameSpace && type.Name == TypeName)
                    {
                        ttype = type;
                    }
                }
            }
            if (ttype == null)
            {
                clrError("Internal: Cannot resolve type: " + nameSpace + "." + TypeName + " Error at CLRInternalMethodsImpl::CreateType", "TypeNotFound");
                return null;
            }
            return CreateType(ttype);
        }
        private MethodArgStack CreateType(DotNetType type)
        {
            //TODO: Do we need to resolve the constructor?
            MethodArgStack a = new MethodArgStack() { ObjectContructor = null, ObjectType = type, type = StackItemType.Object, value = new ObjectValueHolder() };
            return a;
        }
        private void WriteStringToType(MethodArgStack objectInstance, string property, string value)
        {
            var d = (ObjectValueHolder)objectInstance.value;
            d.Fields.Add(property, new MethodArgStack() { type = StackItemType.String, value = value });
        }
        private string ReadStringFromType(MethodArgStack objectInstance, string property)
        {
            var d = (ObjectValueHolder)objectInstance.value;
            return (string)d.Fields[property].value;
        }
    }
}
