﻿using System.Text;

namespace BuiltinGen
{
    //This tool generates a file named Builtin.dll which has byte arrays of the contents of the corlib and test app
    internal class Program
    {
        static void Main(string[] args)
        {
            // get file path to write to

            //TODO: get proper build configuration and dont hardcode paths
            var path = "";
            foreach (var arg in args) { path += arg; }
            string folder = Path.GetDirectoryName(path);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// The file is generated by the BuiltinGen tool ran during build time");
            sb.AppendLine("internal class Builtin");
            sb.AppendLine("{");
            sb.AppendLine("public static byte[] CorLib = new byte[] { "+ CreateByteArray(folder + "/../mscorlib/bin/debug/net6.0/System.Private.CoreLib.dll") + " };");
            sb.AppendLine("public static byte[] TestApp = new byte[] { "+ CreateByteArray(folder + "/../TestApp/bin/debug/net6.0/TestApp.dll") + " };");
            sb.AppendLine("}");
            File.WriteAllText(path, sb.ToString());
        }

        private static string CreateByteArray(string dll)
        {
            var fs = File.ReadAllBytes(dll);

            //convert to string
            StringBuilder sb = new StringBuilder();
            foreach (var item in fs)
            {
                sb.Append(item + ", ");
            }
            var str = sb.ToString();
            return str.Substring(0, str.Length-2);
        }
    }
}