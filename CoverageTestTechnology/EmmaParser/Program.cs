using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using EmmaData;
using System.Runtime.Serialization.Formatters.Binary;

namespace EmmaParser
{
    class Program
    {
        static int Magic = 0x454D4D41;
        static int FILE_HEADER_LENGTH = 4 + 8 + 3 * 4;
        static int ENTRY_HEADER_LENGTH = 8 + 1;
        static long DATA_FORMAT_VERSION = 0x20L;

        static sbyte TYPE_METADATA = 0x0; // must start with 0
        static sbyte TYPE_COVERAGEDATA = 0x1; 

        static void Main(string[] args)
        {
          
            CreateViewForDiff(@"F:\Files\moying\emmaFiles\TableDemo\coverage.em", @"F:\Files\moying\emmaFiles\SwingSet2\src", @"F:\Files\moying\emmaFiles\current.cttm");

            Dictionary<string,string> paths=new Dictionary<string,string>();
            paths.Add("tc_00001",@"F:\Files\moying\emmaFiles\coverage1.ec");
            paths.Add("tc_00002",@"F:\Files\moying\emmaFiles\coverage2.ec");
            paths.Add("tc_00003",@"F:\Files\moying\emmaFiles\coverage3.ec");

            CreateViewForCaseCoverage(@"F:\Files\moying\emmaFiles\coverage.em", paths, @"F:\Files\moying\SwingSet2\src", @"F:\Files\moying\emmaFiles\result.cttr");

            
            //EmmaBinaryReader cebre = new EmmaBinaryReader(@"F:\Files\moying\emmaFiles\TableDemo\coverage.em");
            //MetaDataDescriptor cedata = (MetaDataDescriptor)Load(cebre);
            //cebre.Close();
            //Report.ReportDataModel current = new Report.ReportDataModel();
            //Report.RootItem croot = current.CreateViewForDiff(cedata, @"F:\Files\moying\emmaFiles\SwingSet2\src");

            
            //EmmaBinaryReader ebre = new EmmaBinaryReader(@"F:\Files\moying\emmaFiles\coverage.em");
            //MetaDataDescriptor edata = (MetaDataDescriptor)Load(ebre);
            //ebre.Close();
            //CaseCoverageDescriptor ccdata = new CaseCoverageDescriptor();
            //EmmaBinaryReader ebrc = new EmmaBinaryReader(@"F:\Files\moying\emmaFiles\coverage1.ec");
            //CoverageDataDescriptor cdata = (CoverageDataDescriptor)Load(ebrc);
            //ebrc.Close();

            //ccdata.MergeCaseCoverageData(cdata,"tc_00001");

            //ebrc = new EmmaBinaryReader(@"F:\Files\moying\emmaFiles\coverage2.ec");
            //cdata = (CoverageDataDescriptor)Load(ebrc);
            //ebrc.Close();

            //ccdata.MergeCaseCoverageData(cdata, "tc_00002");

            //ebrc = new EmmaBinaryReader(@"F:\Files\moying\emmaFiles\coverage3.ec");
            //cdata = (CoverageDataDescriptor)Load(ebrc);
            //ebrc.Close();

            //ccdata.MergeCaseCoverageData(cdata, "tc_00003");

            //Report.ReportDataModel previous = new Report.ReportDataModel();
            //Report.RootItem root = previous.CreateViewForCaseCoverage(edata, ccdata, @"F:\Files\moying\SwingSet2\src");

            //current.Compare(previous);

            //Report.ReportGenerator.Process(root,@"F:\Files\moying\emmaFiles\result.xml");
            
        }




        public static object Load(EmmaBinaryReader ebr)
        {
            long length = ebr.Length;
            int m = ebr.ReadInt32();
            bool t = m == Magic;
            long a = ebr.ReadLong();
            bool t2 = a == DATA_FORMAT_VERSION;
            int major = 0, minor = 0, build = 0;
            bool gotAppVersion = false;

            major = ebr.ReadInt32();
            minor = ebr.ReadInt32();
            build = ebr.ReadInt32();

            gotAppVersion = true;
            ebr.Seek(FILE_HEADER_LENGTH, SeekOrigin.Begin);
            long position = FILE_HEADER_LENGTH;
            long entryLength;
            long entrystart = 0;

            object data=null;

            while (true)
            {
                if (position >= length)
                    break;
                entryLength = ebr.ReadLong();
                if ((entryLength <= 0) || (position + entryLength + ENTRY_HEADER_LENGTH > length))
                    break;
                else
                {
                    sbyte type = ebr.ReadSbyte();
                    if ((type < 0) || (type >= 2))
                        break;

                    

                    switch (type)
                    {
                        case 0x0: data = MetaDataDescriptor.ReadExternal(ebr);
                            break;

                        default /* TYPE_COVERAGEDATA */: data = CoverageDataDescriptor.ReadExternal(ebr);
                            break;

                    } // end of switch

                    //MetaDataDescriptor data=MetaDataDescriptor.ReadExternal(ebr);
                    position += entryLength + ENTRY_HEADER_LENGTH;
                    ebr.Seek(position, SeekOrigin.Begin);
                }
            }
            return data;
        }

        public static void CreateViewForDiff(string emFilePath, string srcPath, string outputPath) 
        {
            EmmaBinaryReader cebre = new EmmaBinaryReader(emFilePath);
            MetaDataDescriptor cedata = (MetaDataDescriptor)Load(cebre);
            cebre.Close();
            Report.ReportDataModel current = new Report.ReportDataModel();
            Report.RootItem croot = current.CreateViewForDiff(cedata, srcPath);

            FileStream fs = new FileStream(outputPath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, current);
            fs.Close();
            fs.Dispose();
        }

        public static void CreateViewForCaseCoverage(string emFilePath, Dictionary<string, string> ecFilesPaths,string srcPath,string outputPath) 
        {
            EmmaBinaryReader ebre = new EmmaBinaryReader(emFilePath);
            MetaDataDescriptor edata = (MetaDataDescriptor)Load(ebre);
            ebre.Close();
            CaseCoverageDescriptor ccdata = new CaseCoverageDescriptor();

            foreach (string caseId in ecFilesPaths.Keys)
            {
                EmmaBinaryReader ebrc = new EmmaBinaryReader(ecFilesPaths[caseId]);
                CoverageDataDescriptor cdata = (CoverageDataDescriptor)Load(ebrc);
                ebrc.Close();

                ccdata.MergeCaseCoverageData(cdata, caseId);
            }

            Report.ReportDataModel previous = new Report.ReportDataModel();
            Report.RootItem root = previous.CreateViewForCaseCoverage(edata, ccdata, srcPath);

            FileStream fs = new FileStream(outputPath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, previous);
            fs.Close();
            fs.Dispose();
        }
    }
}
