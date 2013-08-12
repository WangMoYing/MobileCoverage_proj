using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.Executor.CommandOption
{
    /// <summary>
    /// Implements command line parsing
    /// </summary>
    public class CommandLine
    {
        public static CommandArgs Parse(string[] args)
        {
            char[] kArgStart = new char[] { '-', '\\', '"' };
            char[] kArgEnd = new char[] { ' ', '"' };
            CommandArgs ca = new CommandArgs();
            foreach (string arg in args) 
            {
                if (IsOption(arg))
                {
                    ca.CommandOption = arg.TrimStart(kArgStart).TrimEnd(kArgEnd);
                }
                else 
                {
                    ca.Params.Add(arg.TrimStart(kArgStart).TrimEnd(kArgEnd));
                }
            }
            return ca;
        }

        static bool IsOption(string arg)
        {
            return (arg.StartsWith("-") || arg.StartsWith("\\"));
        }
    }
}
