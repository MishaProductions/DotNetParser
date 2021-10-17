using System.Collections.Generic;

namespace LibDotNetParser.CILApi
{
    public class DotNetFile
    {
        PEFile peFile;
        List<DotNetType> t = new List<DotNetType>();
        public PEFile Backend
        {
            get { return peFile; }
        }
        public List<DotNetType> Types
        {
            get
            {
                return t;
            }
        }
        /// <summary>
        /// Entry point of EXE/DLL. Will be null if EXE/DLL does not have entry point.
        /// </summary>
        public DotNetMethod EntryPoint
        {
            get
            {
                var c = peFile.ClrHeader.EntryPointToken;
                var entryPoint = c & 0xFF;

                DotNetMethod m = null;
                foreach (var item in Types)
                {
                    foreach (var m2 in item.Methods)
                    {
                        if (m2.BackendTabel == peFile.Tabels.MethodTabel[(int)entryPoint -1])
                        {
                            m = m2;
                            break;
                        }
                    }
                }

                return m;
            }
        }

        public DotNetType EntryPointType
        {
            get
            {
                var c = peFile.ClrHeader.EntryPointToken;
                var entryPoint = c & 0xFF;

                DotNetType m = null;
                foreach (var item in Types)
                {
                    foreach (var m2 in item.Methods)
                    {
                        if (m2.BackendTabel == peFile.Tabels.MethodTabel[(int)entryPoint - 1])
                        {
                            m = m2.Parrent;
                            break;
                        }
                    }
                }

                return m;
            }
        }
        public DotNetFile(string Path)
        {
            peFile = new PEFile(Path);
            if (!peFile.ContainsMetadata)
                throw new System.Exception("EXE File has no .NET Metadata");

            FindTypes();
        }

        private void FindTypes()
        {
            int i = 0;
            foreach (var item in peFile.Tabels.TypeDefTabel)
            {
                t.Add(new DotNetType(this, item, i + 1));
                i++;
            }
        }

        public DotNetFile(byte[] file)
        {
            peFile = new PEFile(file);
            if (!peFile.ContainsMetadata)
                throw new System.Exception("EXE File has no .NET Metadata");
        }
    }
}
