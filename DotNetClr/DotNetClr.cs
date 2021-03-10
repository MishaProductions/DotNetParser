using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibDotNetParser;
using LibDotNetParser.CILApi;

namespace DotNetClr
{
    public class DotNetClr
    {
        private DotNetFile file;
        private string EXEPath;
        private Dictionary<string, DotNetFile> dlls = new Dictionary<string, DotNetFile>();
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
            catch(Exception x)
            {
                clrError("The entry point was not found. Internal error: "+x.Message, "System.EntryPointNotFoundException");
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
            var code = new IlDecompiler(m).Decompile();
            foreach (var item in code)
            {
                throw new NotImplementedException();
            }
        }

        private void clrError(string message, string errorType, string stackStace = "")
        {
            Console.WriteLine($"A {errorType} has occured in {file.Backend.ClrStringsStream.GetByOffset(file.Backend.Tabels.ModuleTabel[0].Name)}. The error is: {message}");
            Console.WriteLine(stackStace);
        }
    }
}
