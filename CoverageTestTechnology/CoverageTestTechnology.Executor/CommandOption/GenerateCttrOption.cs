using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CoverageTestTechnology.Executor;
using CoverageTestTechnology.Executor.CommandExecutor;
using CoverageTestTechnology.Case;

namespace CoverageTestTechnology.Executor.CommandOption
{
    public class GenerateCttrOption: Option
    {
        public string MetadataFile { set; get; }
        public CttCaseCollection CoverageFilePaths { set; get; }
        public string SourceFileRootPath { set; get; }
        public string CttrOutputPath { set; get; }

        public GenerateCttrOption(CommandArgs args)
        {
            foreach (string param in args.Params)
            {
                if (param.EndsWith(OptionString.EM))
                {
                    MetadataFile = param;
                }
                else if (param.EndsWith(OptionString.XML))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CttCaseCollection));
                    CoverageFilePaths = serializer.Deserialize(File.OpenRead(param)) as CttCaseCollection;
                }
                else if (param.EndsWith(OptionString.CTTR))
                {
                    CttrOutputPath = param;
                }
                else
                {
                    SourceFileRootPath = param;
                }
            }
        }

        public override void Execute()
        {
            CTTExecutor.CreateCttr(MetadataFile, CoverageFilePaths, SourceFileRootPath, CttrOutputPath);
        }
    }
}
