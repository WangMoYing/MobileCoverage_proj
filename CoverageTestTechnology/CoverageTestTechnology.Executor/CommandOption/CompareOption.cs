using System;
using System.Collections.Generic;
using System.Text;
using CoverageTestTechnology.Executor.CommandExecutor;

namespace CoverageTestTechnology.Executor.CommandOption
{
    public class CompareOption:Option
    {
        public string CttmPath { set; get; }
        public string CttrPath { set; get; }
        public string OutCttdPath { set; get; }

        public CompareOption(CommandArgs args)
        {
            foreach (string param in args.Params)
            {
                if (param.EndsWith(OptionString.CTTM))
                {
                    CttmPath = param;
                }
                else if (param.EndsWith(OptionString.CTTR))
                {
                    CttrPath = param;
                }
                else if (param.EndsWith(OptionString.CTTD))
                {
                    OutCttdPath = param;
                }
            }
        }

        public override void Execute()
        {
            CTTExecutor.Compare(CttmPath, CttrPath, OutCttdPath);
        }
    }
}
