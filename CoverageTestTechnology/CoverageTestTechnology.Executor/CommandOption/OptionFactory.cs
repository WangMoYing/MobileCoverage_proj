using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.Executor.CommandOption
{
    public class OptionFactory
    {
        public static Option InitializeWithArgs(string[] args)
        {
            CommandArgs commandArg = CommandLine.Parse(args);
            switch (commandArg.CommandOption)
            { 
                case OptionString.GENERATECTTROPTION:
                    return new GenerateCttrOption(commandArg);
                case OptionString.COMPAREOPTION:
                    return new CompareOption(commandArg);
                case OptionString.CHECKCOVERAGE:
                    return new CheckCoverageOption(commandArg);
                case OptionString.GENERATECTTMOPTION:
                    return new GenerateCttmOption(commandArg);
                default:
                    return null;
            }
        }
    }
}
