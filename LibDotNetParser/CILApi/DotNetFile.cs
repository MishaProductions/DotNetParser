using System.Collections.Generic;

namespace LibDotNetParser.CILApi
{
    public class DotNetFile
    {
        PEParaser peFile;
        public PEParaser Backend
        {
            get { return peFile; }
        }
        public List<DotNetType> Types
        {
            get
            {
                List<DotNetType> t = new List<DotNetType>();
                int i = 0;
                foreach (var item in peFile.tabels.TypeDefTabel)
                {
                    t.Add(new DotNetType(this, item, i + 1));
                    i++;
                }

                return t;
            }
        }
        public DotNetFile(string Path)
        {
            peFile = new PEParaser(Path);
        }

        public DotNetFile(byte[] file)
        {
            peFile = new PEParaser(file);
        }
    }
}
