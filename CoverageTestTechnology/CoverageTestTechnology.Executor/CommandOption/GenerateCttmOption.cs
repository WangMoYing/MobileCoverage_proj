using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoverageTestTechnology.Executor.CommandExecutor;

namespace CoverageTestTechnology.Executor.CommandOption
{
    public class GenerateCttmOption:Option
    {
        public string MetadataFile { set; get; }
        public string SourceFileRootPath { set; get; }
        public string CttmOutputPath { set; get; }

        public GenerateCttmOption(CommandArgs args)
        {
            foreach (string param in args.Params)
            {
                if (param.EndsWith(OptionString.EM))
                {
                    MetadataFile = param;
                }
                else if (param.EndsWith(OptionString.CTTM))
                {
                    CttmOutputPath = param;
                }
                else
                {
                    SourceFileRootPath = param;
                }
            }
        }

        public override void Execute()
        {
            CTTExecutor.CreateCttm(MetadataFile, SourceFileRootPath, CttmOutputPath);
        }
    }
}
