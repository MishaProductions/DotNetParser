using LibDotNetParser.DotNet;
using LibDotNetParser.DotNet.Tabels.Defs;
using LibDotNetParser.PE;

namespace LibDotNetParser.CILApi
{
    public class DotNetField
    {
        private PEFile file;
        public DotNetType ParrentType;
        public Field BackendTabel;
        private FieldAttribs flags;
        public int IndexInTabel { get; private set; }
        public string Name { get; private set; }
        public bool IsStatic { get { return (flags & FieldAttribs.fdStatic) != 0; } }
        public bool HasDefault { get { return (flags & FieldAttribs.fdHasDefault) != 0; } }
        public bool IsPublic { get { return (flags & FieldAttribs.fdPublic) != 0; } }
        /// <summary>
        /// Value of the field.
        /// </summary>
        public MethodArgStack Value;

        public DotNetField(PEFile file, Field backend, DotNetType parrent, int indexintable)
        {
            this.file = file;
            this.ParrentType = parrent;
            this.BackendTabel = backend;
            this.IndexInTabel = indexintable;
            flags = (FieldAttribs)BackendTabel.Flags;
            Name = file.ClrStringsStream.GetByOffset(backend.Name);
        }
    }
}
