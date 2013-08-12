using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    public class DataFactory
    {
        private static int NULL_ARRAY_LENGTH = -1;

        private static int MAGIC = 0x454D4D41; // "EMMA"
        private static long UNKNOWN = 0L;
        private static int FILE_HEADER_LENGTH = 4 + 8 + 3 * 4; // IMPORTANT: update on writeFileHeader() changes
        private static int ENTRY_HEADER_LENGTH = 8 + 1; // IMPORTANT: update on writeEntryHeader() changes
        private static bool DO_FSYNC = true;
        private static int IO_BUF_SIZE = 32 * 1024;

        public static int[] ReadIntArray(EmmaBinaryReader ebr)
        {
            int length = ebr.ReadInt32();
            if (length == NULL_ARRAY_LENGTH)
                return null;
            else
            {
                int[] result = new int[length];
                for (int i = length; --i >= 0; )
                {
                    result[i] = ebr.ReadInt32();
                }
                return result;
            }
        }

        public static bool[] readBooleanArray(EmmaBinaryReader ebr)
        {
            int length = ebr.ReadInt32();
            if (length == NULL_ARRAY_LENGTH)
                return null;
            else
            {
                bool[] result = new bool[length];

                // read array in reverse order:
                for (int i = length; --i >= 0; )
                {
                    result[i] = ebr.ReadBoolean();
                }

                return result;
            }
        }
    }
}
