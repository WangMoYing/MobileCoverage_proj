using System;
using System.IO;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    /// <summary>
    /// Author: Moying Wang
    /// 大坑，C/C++/C#的二进制是无符号的，而JAVA的二进制是有符号的
    /// </summary>
    public class JavaBinaryToCS
    {
        public static uint ReadUInt16FromSbytes(BinaryReader br)
        {
            sbyte[] b = new sbyte[2];
            for (int i = 0; i < 2; i++)
            {
                b[i] = br.ReadSByte();
            }
            return (ushort)(((b[0] & 0xff) << 8) | (b[1] & 0xff));
        }

        public static int ReadInt16FromSbytes(BinaryReader br)
        {
            sbyte[] b = new sbyte[2];
            for (int i = 0; i < 2; i++)
            {
                b[i] = br.ReadSByte();
            }
            return (short)((b[0] << 8) | (b[1] & 0xff));
        }

        public static int ReadInt32FromSbytes(BinaryReader br)
        {
            sbyte[] b = new sbyte[4];
            for (int i = 0; i < 4; i++)
            {
                b[i] = br.ReadSByte();
            }
            return (((b[0] & 0xff) << 24) | ((b[1] & 0xff) << 16) | ((b[2] & 0xff) << 8) | (b[3] & 0xff));
        }

        public static long ReadInt64FromSbytes(BinaryReader br)
        {
            sbyte[] b = new sbyte[8];
            for (int i = 0; i < 8; i++)
            {
                b[i] = br.ReadSByte();
            }
            return (((long)(b[0] & 0xff) << 56) |
                    ((long)(b[1] & 0xff) << 48) |
                    ((long)(b[2] & 0xff) << 40) |
                    ((long)(b[3] & 0xff) << 32) |
                    ((long)(b[4] & 0xff) << 24) |
                    ((long)(b[5] & 0xff) << 16) |
                    ((long)(b[6] & 0xff) << 8) |
                    ((long)(b[7] & 0xff)));
        }

        public static string ReadUTFFromSbytes(BinaryReader br)
        {
            return ReadStringFromSbytes(br, Encoding.UTF8);
        }

        public static string ReadStringFromSbytes(BinaryReader br, Encoding encoding)
        {
            int length = ReadInt16FromSbytes(br);
            return encoding.GetString(br.ReadBytes(length));
        }
    }
}
