namespace LibDotNetParser.PE
{
    public class Section
    {
        public string Name { get; set; }
        public ulong VirtualSize { get; set; }
        public ulong VirtualAddress { get; set; }
        public ulong SizeOfRawData { get; set; }
        public ulong PointerToRawData { get; set; }
        public ulong PointerToRelocations { get; set; }
        public ulong PointerToLinenumbers { get; set; }
        public ushort NumberOfRelocations { get; set; }
        public ushort NumberOfLinenumbers { get; set; }
        public ulong Characteristics { get; set; }
    }
}