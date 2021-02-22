using LibDotNetParser.PE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LibDotNetParser.DotNet.Streams;
using LibDotNetParser.DotNet.Tabels;

namespace LibDotNetParser
{
    public class PEParaser
    {
        #region PE
        public DOSHeader DosHeader { get; private set; }
        public PEHeader PeHeader { get; private set; }
        #endregion
        #region CLR
        public CLRHeader ClrHeader { get; private set; }
        public MetadataHeader ClrMetaDataHeader;
        public MetadataStreamHeader ClrMetaDataStreamHeader { get; private set; }
        public StringsStream ClrStringsStream;
        public USStream ClrUsStream;
        public MetadataReader MetadataReader;
        public Tabels tabels { get; private set; }
        public byte[] ClrStrongNameHash { get; private set; }
        #endregion
        public BinaryReader RawFile;
        public PEParaser(string FilePath)
        {
            byte[] fs = File.ReadAllBytes(FilePath);
            Init(fs);
        }

        public PEParaser(byte[] file)
        {
            Init(file);
        }
        private void Init(byte[] data)
        {
            #region Parse PE & Strong name hash
            RawFile = new BinaryReader(new MemoryStream(data));
            BinaryReader r = new BinaryReader(new MemoryStream(data));

            DosHeader = ReadDOSHeader(r);
            PeHeader = ReadPEHeader(DosHeader.COFFHeaderAddress, r);

            //Read all of the data
            PeHeader.Directories = ReadDirectoriesList(PeHeader.DirectoryLength, r);
            PeHeader.Sections = ReadSectionsList(PeHeader.NumberOfSections, r);

            try
            {
                ClrHeader = ReadCLRHeader(r, PeHeader);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Invaild metadata: " + ex.Message);
            }

            //Read the strong name hash
            ClrStrongNameHash = ReadStrongNameHash(r, ClrHeader.StrongNameSignatureAddress, ClrHeader.StrongNameSignatureSize, PeHeader.Sections);
            #endregion
            #region Parse metadata header

            //Skip past all of the IL Code, and get tto the metadata header
            long pos = (long)RelativeVirtualAddressToFileOffset(ClrHeader.MetaDataDirectoryAddress, PeHeader.Sections);
            r.BaseStream.Position = pos;


            ClrMetaDataHeader = new MetadataHeader();

            ClrMetaDataHeader.Signature = r.ReadUInt32();
            ClrMetaDataHeader.MajorVersion = r.ReadUInt16();
            ClrMetaDataHeader.MinorVersion = r.ReadUInt16();
            ClrMetaDataHeader.Reserved1 = r.ReadUInt32();
            ClrMetaDataHeader.VersionStringLength = r.ReadUInt32();
            ClrMetaDataHeader.VersionString = r.ReadNullTermString((int)ClrMetaDataHeader.VersionStringLength);
            ClrMetaDataHeader.Flags = r.ReadUInt16(); //reserved
            ClrMetaDataHeader.NumberOfStreams = r.ReadUInt16();

            //Simple checks
            //Debug.Assert(ClrMetaDataHeader.Signature == 0x424A5342);
            //Debug.Assert(ClrMetaDataHeader.Reserved1 == 0);
            //Debug.Assert(ClrMetaDataHeader.Flags == 0);
            #endregion
            #region Parse streams

            //Read all of the tabels
            List<StreamHeader> Streams = new List<StreamHeader>();

            //Parse the StreamHeader(s)
            for (int i = 0; i < ClrMetaDataHeader.NumberOfStreams; i++)
            {
                var hdr = new StreamHeader();

                hdr.Offset = r.ReadUInt32();
                hdr.Size = r.ReadUInt32();
                hdr.Name = r.ReadNullTermString();

                //#~ Stream
                if (hdr.Name.Length == 2)
                    r.BaseStream.Position += 1; //Skip past the 4 zeros
                //#Strings stream
                else if (hdr.Name.Length == 8)
                    r.BaseStream.Position += 3;
                //#US Stream
                else if (hdr.Name.Length == 3)
                { }
                //#GUID Stream
                else if (hdr.Name.Length == 5)
                    r.BaseStream.Position += 2;

                Console.WriteLine("Stream: " + hdr.Name + " Size: " + hdr.Size + " Offset: " + hdr.Offset);
                Streams.Add(hdr);
            }

            //Parse the #String stream
            var bytes = GetStreamBytes(r, Streams[1], ClrHeader.MetaDataDirectoryAddress, PeHeader.Sections);
            ClrStringsStream = new StringsStreamReader(bytes).Read();
            
            //Parse the #US Stream
            var bytes2 = GetStreamBytes(r, Streams[2], ClrHeader.MetaDataDirectoryAddress, PeHeader.Sections);
            ClrUsStream = new USStreamReader(bytes2).Read();
            
            #endregion
            #region Parse #~ Stream
            //Parse the #~ stream
            BinaryReader TableStreamR = new BinaryReader(new MemoryStream(
                GetStreamBytes(r, Streams[0], ClrHeader.MetaDataDirectoryAddress, PeHeader.Sections)));

            ClrMetaDataStreamHeader = ReadHeader(TableStreamR);

            //Parse the tabels data
            var numberOfTables = GetTableCount(ClrMetaDataStreamHeader.TablesFlags);
            ClrMetaDataStreamHeader.TableSizes = new uint[numberOfTables];

            for (var i = 0; i < numberOfTables; i++)
            {
                ClrMetaDataStreamHeader.TableSizes[i] = TableStreamR.ReadUInt32();
            }
            
            MetadataReader = new MetadataReader(TableStreamR.BaseStream);
            //Parse the tabels
            tabels = new Tabels(this);
            #endregion
        }


        #region Read Windows Header
        public DOSHeader ReadDOSHeader(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return new DOSHeader
            {
                Magic = reader.ReadUInt16(),
                BytesOnLastPage = reader.ReadUInt16(),
                PagesInFile = reader.ReadUInt16(),
                Relocations = reader.ReadUInt16(),
                SizeOfHeader = reader.ReadUInt16(),
                MinExtraParagraphs = reader.ReadUInt16(),
                MaxExtraParagraphs = reader.ReadUInt16(),
                InitialSS = reader.ReadUInt16(),
                InitialSP = reader.ReadUInt16(),
                Checksum = reader.ReadUInt16(),
                InitialIP = reader.ReadUInt16(),
                InitialCS = reader.ReadUInt16(),
                RelocTableAddress = reader.ReadUInt16(),
                OverlayNumber = reader.ReadUInt16(),
                Unknown01 = reader.ReadUInt16(),
                Unknown02 = reader.ReadUInt16(),
                Unknown03 = reader.ReadUInt16(),
                Unknown04 = reader.ReadUInt16(),
                OEMIdentifier = reader.ReadUInt16(),
                OEMInfo = reader.ReadUInt16(),
                Unknown05 = reader.ReadUInt16(),
                Unknown06 = reader.ReadUInt16(),
                Unknown07 = reader.ReadUInt16(),
                Unknown08 = reader.ReadUInt16(),
                Unknown09 = reader.ReadUInt16(),
                Unknown10 = reader.ReadUInt16(),
                Unknown11 = reader.ReadUInt16(),
                Unknown12 = reader.ReadUInt16(),
                Unknown13 = reader.ReadUInt16(),
                Unknown14 = reader.ReadUInt16(),
                COFFHeaderAddress = reader.ReadUInt16(),
            };
        }
        public PEHeader ReadPEHeader(ushort headerAddress, BinaryReader _assemblyReader)
        {
            _assemblyReader.BaseStream.Seek(headerAddress, SeekOrigin.Begin);
            var header = new PEHeader
            {
                Signature = _assemblyReader.ReadUInt32(),
                Machine = _assemblyReader.ReadUInt16(),
                NumberOfSections = _assemblyReader.ReadUInt16(),
                DateTimeStamp = _assemblyReader.ReadUInt32(),
                PtrToSymbolTable = _assemblyReader.ReadUInt32(),
                NumberOfSymbols = _assemblyReader.ReadUInt32(),
                SizeOfOptionalHeaders = _assemblyReader.ReadUInt16(),
                Characteristics = _assemblyReader.ReadUInt16(),
                OptionalMagic = _assemblyReader.ReadUInt16(),
                MajorLinkerVersion = _assemblyReader.ReadByte(),
                MinorLinkerVersion = _assemblyReader.ReadByte(),
                SizeOfCode = _assemblyReader.ReadUInt32(),
                SizeOfInitData = _assemblyReader.ReadUInt32(),
                SizeOfUninitData = _assemblyReader.ReadUInt32(),
                AddressOfEntryPoint = _assemblyReader.ReadUInt32(),
                BaseOfCode = _assemblyReader.ReadUInt32(),
                BaseOfData = _assemblyReader.ReadUInt32(),
                ImageBase = _assemblyReader.ReadUInt32(),
                SectionAlignment = _assemblyReader.ReadUInt32(),
                FileAlignment = _assemblyReader.ReadUInt32(),
                MajorOSVersion = _assemblyReader.ReadUInt16(),
                MinorOSVersion = _assemblyReader.ReadUInt16(),
                MajorImageVersion = _assemblyReader.ReadUInt16(),
                MinorImageVersion = _assemblyReader.ReadUInt16(),
                MajorSubsystemVersion = _assemblyReader.ReadUInt16(),
                MinorSubsystemVersion = _assemblyReader.ReadUInt16(),
                Reserved1 = _assemblyReader.ReadUInt32(),
                SizeOfImage = _assemblyReader.ReadUInt32(),
                SizeOfHeaders = _assemblyReader.ReadUInt32(),
                PEChecksum = _assemblyReader.ReadUInt32(),
                Subsystem = _assemblyReader.ReadUInt16(),
                DLLCharacteristics = _assemblyReader.ReadUInt16(),
                SizeOfStackReserve = _assemblyReader.ReadUInt32(),
                SizeOfStackCommit = _assemblyReader.ReadUInt32(),
                SizeOfHeapReserve = _assemblyReader.ReadUInt32(),
                SizeOfHeapCommit = _assemblyReader.ReadUInt32(),
                LoaderFlags = _assemblyReader.ReadUInt32(),
                DirectoryLength = _assemblyReader.ReadUInt32()
            };
            return header;
        }
        #endregion
        #region Read virtual directory
        private IList<DataDirectory> ReadDirectoriesList(uint directoryCount, BinaryReader _assemblyReader)
        {
            var result = new List<DataDirectory>((int)directoryCount);
            for (var i = 0; i < directoryCount; i++)
            {
                result.Add(new DataDirectory
                {
                    Address = _assemblyReader.ReadUInt32(),
                    Size = _assemblyReader.ReadUInt32()
                });
            }
            return result;
        }
        public byte[] ReadVirtualDirectory(BinaryReader reader, DataDirectory dataDirectory, IList<Section> sections)
        {
            // find the section whose virtual address range contains the data directory's virtual address.
            var section = sections[0];

            // calculate the offset into the file.
            var fileOffset = section.PointerToRawData + (dataDirectory.Address - section.VirtualAddress);

            // read the virtual directory data.
            reader.BaseStream.Seek((long)fileOffset, SeekOrigin.Begin);
            return reader.ReadBytes((int)dataDirectory.Size);
        }
        #endregion
        #region Read the sections
        private List<Section> ReadSectionsList(int numberOfSections, BinaryReader _assemblyReader)
        {
            var result = new List<Section>();
            for (var i = 0; i < numberOfSections; i++)
            {
                var s = new Section();

                s.Name = _assemblyReader.ReadNullTermString(8);
                s.VirtualSize = _assemblyReader.ReadUInt32();
                s.VirtualAddress = _assemblyReader.ReadUInt32();
                s.SizeOfRawData = _assemblyReader.ReadUInt32();
                s.PointerToRawData = _assemblyReader.ReadUInt32();
                s.PointerToRelocations = _assemblyReader.ReadUInt32();
                s.PointerToLinenumbers = _assemblyReader.ReadUInt32();
                s.NumberOfRelocations = _assemblyReader.ReadUInt16();
                s.NumberOfLinenumbers = _assemblyReader.ReadUInt16();
                s.Characteristics = _assemblyReader.ReadUInt32();

                result.Add(s);
            }
            return result;
        }
        public byte[] ReadSection(BinaryReader reader, Section section)
        {
            reader.BaseStream.Seek((long)section.PointerToRawData, SeekOrigin.Begin);
            return reader.ReadBytes((int)section.SizeOfRawData);
        }
        #endregion
        #region Read CLR Headers
        private CLRHeader ReadCLRHeader(BinaryReader assemblyReader, PEHeader peHeader)
        {
            var clrDirectoryHeader = peHeader.Directories[(int)DataDirectoryName.CLRHeader];
            var clrDirectoryData = ReadVirtualDirectory(assemblyReader, clrDirectoryHeader, peHeader.Sections);
            using (var reader = new BinaryReader(new MemoryStream(clrDirectoryData)))
            {
                var a = new CLRHeader
                {
                    HeaderSize = reader.ReadUInt32(),
                    MajorRuntimeVersion = reader.ReadUInt16(),
                    MinorRuntimeVersion = reader.ReadUInt16(),
                    MetaDataDirectoryAddress = reader.ReadUInt32(),
                    MetaDataDirectorySize = reader.ReadUInt32(),
                    Flags = reader.ReadUInt32(),
                    EntryPointToken = reader.ReadUInt32(),
                    ResourcesDirectoryAddress = reader.ReadUInt32(),
                    ResourcesDirectorySize = reader.ReadUInt32(),
                    StrongNameSignatureAddress = reader.ReadUInt32(),
                    StrongNameSignatureSize = reader.ReadUInt32(),
                    CodeManagerTableAddress = reader.ReadUInt32(),
                    CodeManagerTableSize = reader.ReadUInt32(),
                    VTableFixupsAddress = reader.ReadUInt32(),
                    VTableFixupsSize = reader.ReadUInt32(),
                    ExportAddressTableJumpsAddress = reader.ReadUInt32(),
                    ExportAddressTableJumpsSize = reader.ReadUInt32(),
                    ManagedNativeHeaderAddress = reader.ReadUInt32(),
                    ManagedNativeHeaderSize = reader.ReadUInt32()
                };
                return a;
            }
        }
        private MetadataStreamHeader ReadHeader(BinaryReader _reader)
        {
            return new MetadataStreamHeader
            {
                Reserved1 = _reader.ReadUInt32(),
                MajorVersion = _reader.ReadByte(),
                MinorVersion = _reader.ReadByte(),
                OffsetSizeFlags = (StreamOffsetSizeFlags)_reader.ReadByte(),
                Reserved2 = _reader.ReadByte(),
                TablesFlags = (MetadataTableFlags)_reader.ReadUInt64(),
                SortedTablesFlags = (MetadataTableFlags)_reader.ReadUInt64(),
            };
        }
        private static byte[] ReadStrongNameHash(BinaryReader reader, uint rva, uint size, IEnumerable<Section> sections)
        {
            if (rva == 0)
                return new byte[0];

            var fileOffset = RelativeVirtualAddressToFileOffset(rva, sections);
            reader.BaseStream.Seek((long)fileOffset, SeekOrigin.Begin);
            return reader.ReadBytes((int)size);
        }
        private int GetTableCount(MetadataTableFlags tablesFlags)
        {
            var count = 0;
            while (tablesFlags != 0)
            {
                tablesFlags = tablesFlags & (tablesFlags - 1);
                count++;
            }
            return count;
        }
        public byte[] GetStreamBytes(BinaryReader reader, StreamHeader streamHeader, uint metadataDirectoryAddress, IEnumerable<Section> sections)
        {
            var rva = metadataDirectoryAddress + streamHeader.Offset;
            var fileOffset = RelativeVirtualAddressToFileOffset(rva, sections);
            reader.BaseStream.Seek((long)fileOffset, SeekOrigin.Begin);
            return reader.ReadBytes((int)streamHeader.Size);
        }
        #endregion
        public static ulong RelativeVirtualAddressToFileOffset(ulong rva, IEnumerable<Section> sections)
        {
            // find the section whose virtual address range contains the data directory's virtual address.
            Section section = null;
            foreach (var s in sections)
            {
                if (s.VirtualAddress <= rva && s.VirtualAddress + s.SizeOfRawData >= rva)
                {
                    section = s;
                    break;
                }
            }
            
            if (section == null)
                throw new Exception("Cannot find the section");

            // calculate the offset into the file.
            var fileOffset = section.PointerToRawData + (rva - section.VirtualAddress);
            return fileOffset;
        }
    }
}
