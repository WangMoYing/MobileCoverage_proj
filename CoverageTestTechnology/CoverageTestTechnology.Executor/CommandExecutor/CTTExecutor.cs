using System;
using System.Collections.Generic;
using System.Text;
using CoverageTestTechnology;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CoverageTestTechnology.Case;
using CoverageTestTechnology.EmmaData;
using CoverageTestTechnology.Report;

namespace CoverageTestTechnology.Executor.CommandExecutor
{
    public class CTTExecutor
    {
        static int Magic = 0x454D4D41;
        static int FILE_HEADER_LENGTH = 4 + 8 + 3 * 4;
        static int ENTRY_HEADER_LENGTH = 8 + 1;
        static long DATA_FORMAT_VERSION = 0x20L;

        const sbyte TYPE_METADATA = 0x0; // must start with 0
        const sbyte TYPE_COVERAGEDATA = 0x1; 

        private static object LoadEmmaFile(EmmaBinaryReader ebr)
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

            object data = null;

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
                        case TYPE_METADATA: data = MetaDataDescriptor.ReadExternal(ebr);
                            break;

                        case TYPE_COVERAGEDATA: data = CoverageDataDescriptor.ReadExternal(ebr);
                            break;

                    } // end of switch

                    //MetaDataDescriptor data=MetaDataDescriptor.ReadExternal(ebr);
                    position += entryLength + ENTRY_HEADER_LENGTH;
                    ebr.Seek(position, SeekOrigin.Begin);
                }
            }
            return data;
        }

        public static ReportDataModel LoadCttFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            ReportDataModel item = bf.Deserialize(fs) as ReportDataModel;
            fs.Close();
            fs.Dispose();
            return item;
        }

        private static DiffItem LoadDiff(string path) 
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            DiffItem item = bf.Deserialize(fs) as DiffItem;
            fs.Close();
            fs.Dispose();
            return item;
        }

        public static void CreateCttm(string emFilePath, string srcPath, string outputPath)
        {
            EmmaBinaryReader cebre = new EmmaBinaryReader(emFilePath);
            MetaDataDescriptor cedata = (MetaDataDescriptor)LoadEmmaFile(cebre);
            cebre.Close();
            Report.ReportDataModel current = new Report.ReportDataModel();
            Report.RootItem croot = current.CreateViewForDiff(cedata, srcPath);

            FileStream fs = new FileStream(outputPath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, current);
            fs.Close();
            fs.Dispose();
        }

        private static ReportDataModel CreateModuleForCttr(string emFilePath, CttCaseCollection ecFilesPaths, string srcPath) 
        {
            EmmaBinaryReader ebre = new EmmaBinaryReader(emFilePath);
            MetaDataDescriptor edata = (MetaDataDescriptor)LoadEmmaFile(ebre);
            ebre.Close();
            CaseCoverageDescriptor ccdata = new CaseCoverageDescriptor();

            foreach (CttCase cttCase in ecFilesPaths.CoverageFilePaths)
            {
                EmmaBinaryReader ebrc = new EmmaBinaryReader(cttCase.ResultPath);
                CoverageDataDescriptor cdata = (CoverageDataDescriptor)LoadEmmaFile(ebrc);
                ebrc.Close();

                ccdata.MergeCaseCoverageData(cdata, cttCase.CaseId);
            }

            ReportDataModel previous = new Report.ReportDataModel();
            RootItem root = previous.CreateViewForCaseCoverage(edata, ccdata, srcPath);
            return previous;
        }

        public static void CreateCttr(string emFilePath, CttCaseCollection ecFilesPaths, string srcPath, string outputPath)
        {
            ReportDataModel previous = CreateModuleForCttr(emFilePath, ecFilesPaths, srcPath);

            FileStream fs = new FileStream(outputPath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, previous);
            fs.Close();
            fs.Dispose();
        }

        public static void Compare(string cttmPath, string cttrPath, string outputPath)
        {
            ReportDataModel current = LoadCttFile(cttmPath);
            ReportDataModel previous = LoadCttFile(cttrPath);
            DiffItem diffItem = current.Compare(previous);
            FileStream fs = new FileStream(outputPath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, diffItem);
            fs.Close();
            fs.Dispose();
        }

        public static void CheckCoverage(string emFilePath, CttCaseCollection ecFilesPaths, string srcPath, string cttdPath) 
        {
            DiffItem diffItem = LoadDiff(cttdPath);
            ReportDataModel data = CreateModuleForCttr(emFilePath, ecFilesPaths, srcPath);
            diffItem = data.CheckCoverage(diffItem);
        }
    }
}
