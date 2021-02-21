using LibDotNetParser;
using LibDotNetParser.DotNet.Tabels.Defs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDotNetParser.CILApi
{
    public class DotNetType
    {
        private PEParaser file;
        private TypeDefTabelRow type;
        private TypeFlags flags;
        private int NextTypeIndex;

        public string Name { get; private set; }
        public string NameSpace { get; private set; }
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(NameSpace))
                    return Name;
                else
                    return NameSpace + "." + Name;
            }
        }
        public bool IsPublic
        {
            get
            {
                return flags.HasFlag(TypeFlags.tdPublic);
            }
        }
        public bool IsInterface
        {
            get
            {
                return flags.HasFlag(TypeFlags.tdInterface);
            }
        }

        public List<DotNetMethod> Methods
        {
            get
            {
                List<DotNetMethod> m = new List<DotNetMethod>();
                uint startIndex = type.MethodList;

                int max;

                if (file.tabels.TypeDefTabel.Count <= NextTypeIndex)
                {
                    max = file.tabels.TypeDefTabel.Count;
                }
                else
                {
                    max = (int)file.tabels.TypeDefTabel[(int)NextTypeIndex].MethodList - 1;
                }
                for (uint i = startIndex - 1; i < max; i++)
                {
                    var item = file.tabels.MethodTabel[(int)i];
                    m.Add(new DotNetMethod(file, item, this, i + 1));
                }

                return m;
            }
        }
        /// <summary>
        /// Should be used internaly
        /// </summary>
        /// <param name="file"></param>
        /// <param name="item"></param>
        /// <param name="NextTypeIndex"></param>
        public DotNetType(PEParaser file, TypeDefTabelRow item, int NextTypeIndex)
        {
            this.file = file;
            this.type = item;
            this.NextTypeIndex = NextTypeIndex;
            this.flags = (TypeFlags)item.Flags;

            Name = file.ClrStringsStream.GetByOffset(item.Name);
            NameSpace = file.ClrStringsStream.GetByOffset(item.Namespace);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
