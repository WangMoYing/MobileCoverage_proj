using System;
using CoverageTestTechnology.Executor.CommandOption;

namespace CTTRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Option option = OptionFactory.InitializeWithArgs(args);
                option.Execute();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            //ReportDataModel current = Load(@"F:\Files\moying\emmaFiles\current.cttm");
            //ReportDataModel previous = Load(@"F:\Files\moying\emmaFiles\result.cttr");
            //current.Compare(previous);
        }
    }
}
