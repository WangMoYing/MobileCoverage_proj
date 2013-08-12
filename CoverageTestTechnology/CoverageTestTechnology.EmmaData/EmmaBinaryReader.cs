using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    
    public class EmmaBinaryReader
    {
        private FileStream m_fileStream = null;
        private BinaryReader m_binaryReader = null;

        public long Length 
        {
            get 
            {
                return m_fileStream.Length;
            }
        }

        /// <summary>
        /// 使用指定的字符编码初始化EmmaBinaryReader对象
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">字符编码</param>
        public EmmaBinaryReader(string path, Encoding encoding) 
        {
            m_fileStream = new FileStream(path, FileMode.Open);
            m_binaryReader = new BinaryReader(m_fileStream, encoding);
        }

        /// <summary>
        /// 使用默认UTF8编码初始化EmmaBinaryReader对象
        /// </summary>
        /// <param name="path">文件路径</param>
        public EmmaBinaryReader(string path)
            : this(path, Encoding.UTF8)
        {

        }

        public long Seek(long offset,SeekOrigin origin) 
        {
            return m_fileStream.Seek(offset, origin);
        }

        public void Close() 
        {
            m_binaryReader.Close();
            m_fileStream.Close();
            m_binaryReader.Dispose();
            m_fileStream.Dispose();
        }

        public int ReadInt16() 
        {
            return JavaBinaryToCS.ReadInt16FromSbytes(m_binaryReader);
        }

        public uint ReadUint16() 
        {
            return JavaBinaryToCS.ReadUInt16FromSbytes(m_binaryReader);
        }

        public int ReadInt32() 
        {
            return JavaBinaryToCS.ReadInt32FromSbytes(m_binaryReader);
        }

        public long ReadLong() 
        {
            return JavaBinaryToCS.ReadInt64FromSbytes(m_binaryReader);
        }

        public bool ReadBoolean() 
        {
            return m_binaryReader.ReadBoolean();
        }

        public SByte ReadSbyte() 
        {
            return m_binaryReader.ReadSByte();
        }

        public string ReadUTF() 
        {
            return JavaBinaryToCS.ReadUTFFromSbytes(m_binaryReader);
        }

        public string ReadString(Encoding encoding) 
        {
            return JavaBinaryToCS.ReadStringFromSbytes(m_binaryReader, encoding);
        }
    }
}
