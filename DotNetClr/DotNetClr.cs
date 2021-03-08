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
        public DotNetClr(string ApplicationPath)
        {
            init(File.ReadAllBytes(ApplicationPath));
        }
        public DotNetClr(byte[] exe)
        {
            init(exe);
        }

        private void init(byte[] b)
        {
            DotNetFile p = null;
            try
            {
                p = new DotNetFile(b);
            }
            catch
            {
                throw new Exception("Invaild .NET executable");
            }
            file = p;
            if (file.EntryPoint == null)
            {
                clrError("The entry point was not found.", "System.EntryPointNotFoundException");
                file = null;
                return;
            }

            //TODO: load all of the assemblies in the assemblieRef Tabel.
        }

        private void clrError(string message, string errorType, string stackStace="")
        {
            Console.WriteLine($"A {errorType} has occured in {file.Backend.ClrStringsStream.GetByOffset(file.Backend.Tabels.ModuleTabel[0].Name)}. The error is: {message}");
            Console.WriteLine(stackStace);
        }
    }
}
