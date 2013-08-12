using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    public class DataConstants
    {
        public const int METHOD_NO_LINE_NUMBER_TABLE = 0x01;
        public const int METHOD_ABSTRACT_OR_NATIVE = 0x02;
        public const int METHOD_EXCLUDED = 0x04;
        public const int METHOD_ADDED = 0x08;

        public const int METHOD_NO_BLOCK_DATA = (METHOD_ABSTRACT_OR_NATIVE | METHOD_EXCLUDED | METHOD_ADDED);
        public const int METHOD_NO_LINE_DATA = (METHOD_NO_LINE_NUMBER_TABLE | METHOD_NO_BLOCK_DATA);


        public const string INIT_NAME = "<init>";
        public const string CLINIT_NAME = "<clinit>";
        public const string CLINIT_DESCRIPTOR = "()V";

    }
}
