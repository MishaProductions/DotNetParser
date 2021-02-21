using LibDotNetParser.DotNet.Tabels.Defs;
using LibDotNetParser.PE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LibDotNetParser.DotNet.Tabels
{
    public class Tabels
    {
        private PEParaser p;
        private MetadataReader r;
        public List<ModuleTabelRow> ModuleTabel { get; }
        public List<TypeRefTabelRow> TypeRefTabel { get; }
        public List<TypeDefTabelRow> TypeDefTabel { get; }
        public List<FieldTabelRow> FieldTabel { get; }
        public List<MethodTabelRow> MethodTabel { get; }
        public List<ParamTabelRow> ParmTabel { get; }
        public List<InterfaceImplTabelRow> InterfaceImplTable { get; }
        public List<MemberRefTabelRow> MemberRefTabelRow { get; }
        public Tabels(PEParaser p)
        {
            //Init
            this.p = p;
            this.r = p.MetadataReader;

            var x = r.BaseStream.Position;


            //Read all of the tabels
            ModuleTabel = new List<ModuleTabelRow>();
            TypeRefTabel = new List<TypeRefTabelRow>();
            TypeDefTabel = new List<TypeDefTabelRow>();
            FieldTabel = new List<FieldTabelRow>();
            MethodTabel = new List<MethodTabelRow>();
            ParmTabel = new List<ParamTabelRow>();
            InterfaceImplTable = new List<InterfaceImplTabelRow>();
            MemberRefTabelRow = new List<MemberRefTabelRow>();

            int a = 0;
            //Read module Tabel (if any)
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.Module) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new ModuleTabelRow();
                    m.Read(r);
                    ModuleTabel.Add(m);
                }
                a++;
            }

            //Read TypeRef Tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.TypeRef) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new TypeRefTabelRow();
                    m.Read(r);
                    TypeRefTabel.Add(m);
                }
                a++;
            }
            //Read TypeDef Tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.TypeDef) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new TypeDefTabelRow();
                    m.Read(r);
                    TypeDefTabel.Add(m);
                }
                a++;
            }

            //Read Field Tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.Field) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new FieldTabelRow();
                    m.Read(r);
                    FieldTabel.Add(m);
                }
                a++;
            }
            //Read Method tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.Method) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new MethodTabelRow();
                    m.Read(r);
                    MethodTabel.Add(m);
                }
                a++;
            }
            //Read Parm Tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.Param) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new ParamTabelRow();
                    m.Read(r);
                    ParmTabel.Add(m);
                }
                a++;
            }
            //Read interfaceimpl Tabel (NOT TESTED)
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.InterfaceImpl) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new InterfaceImplTabelRow();
                    m.Read(r);
                    InterfaceImplTable.Add(m);
                }
                a++;
            }
            //Read MemberRef tabel
            if ((p.ClrMetaDataStreamHeader.TablesFlags & MetadataTableFlags.MemberRef) != 0)
            {
                for (int i = 0; i < p.ClrMetaDataStreamHeader.TableSizes[a]; i++)
                {
                    var m = new MemberRefTabelRow();
                    m.Read(r);
                    MemberRefTabelRow.Add(m);
                }
                a++;
            }
        }
    }
}
