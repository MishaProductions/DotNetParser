using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibDotNetParser.PE
{
    /// <summary>
    /// The DOS Header.
    /// </summary>
    public class DOSHeader
    {
        public ushort Magic { get; set; }
        public ushort BytesOnLastPage { get; set; }
        public ushort PagesInFile { get; set; }
        public ushort Relocations { get; set; }
        public ushort SizeOfHeader { get; set; }
        public ushort MinExtraParagraphs { get; set; }
        public ushort MaxExtraParagraphs { get; set; }
        public ushort InitialSS { get; set; }
        public ushort InitialSP { get; set; }
        public ushort Checksum { get; set; }
        public ushort InitialIP { get; set; }
        public ushort InitialCS { get; set; }
        public ushort RelocTableAddress { get; set; }
        public ushort OverlayNumber { get; set; }
        public ushort Unknown01 { get; set; }
        public ushort Unknown02 { get; set; }
        public ushort Unknown03 { get; set; }
        public ushort Unknown04 { get; set; }
        public ushort OEMIdentifier { get; set; }
        public ushort OEMInfo { get; set; }
        public ushort Unknown05 { get; set; }
        public ushort Unknown06 { get; set; }
        public ushort Unknown07 { get; set; }
        public ushort Unknown08 { get; set; }
        public ushort Unknown09 { get; set; }
        public ushort Unknown10 { get; set; }
        public ushort Unknown11 { get; set; }
        public ushort Unknown12 { get; set; }
        public ushort Unknown13 { get; set; }
        public ushort Unknown14 { get; set; }
        public ushort COFFHeaderAddress { get; set; }
    }
    /// <summary>
    /// The PE Header.
    /// </summary>
    public class PEHeader
    {
        public uint Signature { get; set; }
        public ushort Machine { get; set; }
        public ushort NumberOfSections { get; set; }
        public uint DateTimeStamp { get; set; }
        public uint PtrToSymbolTable { get; set; }
        public uint NumberOfSymbols { get; set; }
        public ushort SizeOfOptionalHeaders { get; set; }
        public ushort Characteristics { get; set; }
        public ushort OptionalMagic { get; set; }
        public byte MajorLinkerVersion { get; set; }
        public byte MinorLinkerVersion { get; set; }
        public uint SizeOfCode { get; set; }
        public uint SizeOfInitData { get; set; }
        public uint SizeOfUninitData { get; set; }
        public uint AddressOfEntryPoint { get; set; }
        public uint BaseOfCode { get; set; }
        public uint BaseOfData { get; set; }
        public uint ImageBase { get; set; }
        public uint SectionAlignment { get; set; }
        public uint FileAlignment { get; set; }
        public ushort MajorOSVersion { get; set; }
        public ushort MinorOSVersion { get; set; }
        public ushort MajorImageVersion { get; set; }
        public ushort MinorImageVersion { get; set; }
        public ushort MajorSubsystemVersion { get; set; }
        public ushort MinorSubsystemVersion { get; set; }
        public uint Reserved1 { get; set; }
        public uint SizeOfImage { get; set; }
        public uint SizeOfHeaders { get; set; }
        public uint PEChecksum { get; set; }
        public ushort Subsystem { get; set; }
        public ushort DLLCharacteristics { get; set; }
        public uint SizeOfStackReserve { get; set; }
        public uint SizeOfStackCommit { get; set; }
        public uint SizeOfHeapReserve { get; set; }
        public uint SizeOfHeapCommit { get; set; }
        public uint LoaderFlags { get; set; }
        public uint DirectoryLength { get; set; }
        public IList<DataDirectory> Directories { get; set; }
        public IList<Section> Sections { get; set; }
    }
    /// <summary>
    /// CLR Header
    /// </summary>
    public class CLRHeader
    {

        public uint HeaderSize { get; set; }
        public ushort MajorRuntimeVersion { get; set; }
        public ushort MinorRuntimeVersion { get; set; }
        public uint MetaDataDirectoryAddress { get; set; }
        public uint MetaDataDirectorySize { get; set; }
        public uint Flags { get; set; }
        public uint EntryPointToken { get; set; }
        public uint ResourcesDirectoryAddress { get; set; }
        public uint ResourcesDirectorySize { get; set; }
        public uint StrongNameSignatureAddress { get; set; }
        public uint StrongNameSignatureSize { get; set; }
        public uint CodeManagerTableAddress { get; set; }
        public uint CodeManagerTableSize { get; set; }
        public uint VTableFixupsAddress { get; set; }
        public uint VTableFixupsSize { get; set; }
        public uint ExportAddressTableJumpsAddress { get; set; }
        public uint ExportAddressTableJumpsSize { get; set; }
        public uint ManagedNativeHeaderAddress { get; set; }
        public uint ManagedNativeHeaderSize { get; set; }
    }
    public enum DataDirectoryName
    {
        Export,
        Import,
        Resource,
        Exception,
        Security,
        Relocation,
        Debug,
        Copyright,
        GlobalPtr,
        ThreadLocalStorage,
        LoadConfig,
        BoundImport,
        ImportAddressTable,
        DelayLoadImportAddressTable,
        CLRHeader,
        Reserved
    }
    /// <summary>
    /// .NET Meta data header
    /// </summary>
    public class MetadataHeader
    {
        public ulong Signature { get; set; } // always 0x424A5342 [42 53 4A 42]
        public uint MajorVersion { get; set; } // always 0x0001 [01 00]
        public uint MinorVersion { get; set; } // always 0x0001 [01 00]
        public ulong Reserved1 { get; set; } // always 0x00000000 [00 00 00 00]
        public ulong VersionStringLength { get; set; }
        public string VersionString { get; set; } // null terminated in file. VersionStringLength includes the null(s) in the length, and also is always rounded up to a multiple of 4.
        public ushort Flags { get; set; } // always 0x0000 [00 00]
        public ushort NumberOfStreams { get; set; }
    }
    /// <summary>
    /// .NET Stream Header
    /// </summary>
    public class StreamHeader
    {
        public uint Offset { get; set; } // relative to start of MetadataHeader (Same as CLRHeader.MetaDataDirectoryAddress, resolved to file offset, then add this stream Offset.)
        public uint Size { get; set; }
        public string Name { get; set; } // null terminated in file, length always rounded up to divisible by 4
    }
    /// <summary>
    /// Header for the #~ stream.
    /// </summary>
    public class MetadataStreamHeader
    {
        public uint Reserved1 { get; set; }
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public StreamOffsetSizeFlags OffsetSizeFlags { get; set; } // indicates offset sizes to be used within the other streams.
        public byte Reserved2 { get; set; } // Always set to 0x01 [01]
        public MetadataTableFlags TablesFlags { get; set; } // indicated which tables are present. 8 bytes.
        public MetadataTableFlags SortedTablesFlags { get; set; } // indicated which tables are sorted. 8 bytes.
        public uint[] TableSizes { get; set; } // Size of each table. Array count will be same as # of '1's in TableFlags.
    }
    /// <summary>
    /// If the flag is not set, the offsets into the respective heap are stored as 2-bytes,
    /// If the flag is set, then the offsets are stored as 4-bytes.
    /// </summary>
    [Flags]
    public enum StreamOffsetSizeFlags : byte
    {
        String = 0x01,
        GUID = 0x02,
        Blob = 0x04,
        TypeDefOrRef
    }
    [Flags]
    public enum MetadataTableFlags : ulong
    {
        Module = 1,
        TypeRef = 2,
        TypeDef = 4,
        Reserved1 = 8,
        Field = 16,
        Reserved2 = 32,
        Method = 64,
        Reserved3 = 128,
        Param = 256,
        InterfaceImpl = 512,
        MemberRef = 1024,
        Constant = 2048,
        CustomAttribute = 4096,
        FieldMarshal = 8192,
        DeclSecurity = 16384,
        ClassLayout = 32768,
        FieldLayout = 65536,
        StandAloneSig = 131072,
        EventMap = 262144,
        Reserved4 = 524288,
        Event = 1048576,
        PropertyMap = 2097152,
        Reserved5 = 4194304,
        Property = 8388608,
        MethodSemantics = 16777216,
        MethodImpl = 33554432,
        ModuleRef = 67108864,
        TypeSpec = 134217728,
        ImplMap = 268435456,
        FieldRVA = 536870912,
        Reserved6 = 1073741824,
        Reserved7 = 2147483648,
        Assembly = 4294967296,
        AssemblyProcessor = 8589934592,
        AssemblyOS = 17179869184,
        AssemblyRef = 34359738368,
        AssemblyRefProcessor = 68719476736,
        AssemblyRefOS = 137438953472,
        File = 274877906944,
        ExportedType = 549755813888,
        ManifestResource = 1099511627776,
        NestedClass = 2199023255552,
        GenericParam = 4398046511104,
        MethodSpec = 8796093022208,
        GenericParamConstraint = 17592186044416,
    }
}
