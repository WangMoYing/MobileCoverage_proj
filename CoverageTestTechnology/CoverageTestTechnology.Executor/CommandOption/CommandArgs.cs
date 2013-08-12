using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.Executor.CommandOption
{
    /// <summary>
    /// Contains the parsed command line arguments. This consists of two
    /// lists, one of argument pairs, and one of stand-alone arguments.
    /// </summary>
    public class CommandArgs
    {
        public string CommandOption { set; get; }

        /// <summary>
        /// Returns the list of stand-alone parameters.
        /// </summary>
        public List<string> Params
        {
            get { return mParams; }
        }
        List<string> mParams = new List<string>();
    }
}
