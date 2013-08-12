using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class MethodDescriptor
    {
        private string m_name; // internal JVM name (<init>, <clinit> for initializers, etc) [never null]
        private string m_descriptor; // [never null]
        private int m_status; // excluded, no debug data, etc
        private int[] m_blockSizes; // always of positive length if ((status & METHOD_NO_BLOCK_DATA) == 0)
        private int[][] m_blockMap; // [never null or empty if status is ...]
        private int m_firstLine; // 0 if not src line info is available
        private int m_lastLine = -1;
        private Dictionary<int, HashSet<int>> m_lineMap;

        public string Name
        {
            get { return m_name; }
        }
        public int Status
        {
            get { return m_status; }
        }
        public string Descriptor
        {
            get { return m_descriptor; }
        }
        public int[] BlockSizes
        {
            get { return m_blockSizes; }
        }
        public int[][] BlockMap
        {
            get { return m_blockMap; }
        }
        public int FirstLine
        {
            get { return m_firstLine; }
        }

        /// <summary>
        /// 这个最后行是由代码块计算的，由于代码块的跳跃式，所以此最后行未必是方法的结尾行
        /// </summary>
        public int LastLine 
        {
            get 
            {
                if (m_lastLine == -1)
                {
                    for (int i = 0; i < m_blockMap.Length; i++) 
                    {
                        for (int j = 0; j < m_blockMap[i].Length; j++) 
                        {
                            if (m_blockMap[i][j] > m_lastLine) 
                            {
                                m_lastLine = m_blockMap[i][j];
                            }
                        }
                    }
                }
                return m_lastLine;
            }
        }

        public Dictionary<int, HashSet<int>> LineMap
        {
            get
            {
                if (m_lineMap != null)
                {
                    return m_lineMap;
                }
                else if ((m_status & DataConstants.METHOD_NO_LINE_DATA) == 0)
                {
                    // construct reverse line->block ID mapping:
                    m_lineMap = new Dictionary<int, HashSet<int>>();
                    for (int bl = 0; bl < m_blockMap.Length; ++bl)
                    {
                        int[] lines = m_blockMap[bl];
                        if (lines != null)
                        {
                            int lineCount = lines.Length;

                            for (int l = 0; l < lineCount; ++l)
                            {
                                int line = lines[l];
                                HashSet<int> blockIDs;
                                if (!m_lineMap.ContainsKey(line))
                                {
                                    blockIDs = new HashSet<int>();
                                    m_lineMap.Add(line, blockIDs);
                                }
                                else 
                                {
                                    blockIDs = m_lineMap[line];
                                }
                                blockIDs.Add(bl);
                            }
                        }
                    }
                    return m_lineMap;
                }
                return null;
            }
        }

        private MethodDescriptor(string name, string descriptor, int status, int[] blockSizes, int[][] blockMap, int firstLine)
        {
            if (name == null)
                throw new ArgumentNullException("null input: name");
            if (descriptor == null)
                throw new ArgumentNullException("null input: descriptor");
            if ((status & DataConstants.METHOD_NO_BLOCK_DATA) == 0)
            {
                int blockCount = blockSizes.Length;
                if (blockCount <= 0)
                    throw new ArgumentException("blockCount must be positive.");
                m_blockSizes = blockSizes;
                if ((status & DataConstants.METHOD_NO_LINE_DATA) == 0)
                {
                    if ((blockMap == null) || (blockMap.Length == 0))
                        throw new ArgumentException("null or empty input: blockMap");
                    m_blockMap = blockMap;
                    m_firstLine = firstLine;
                }
                else
                {
                    m_blockMap = null;
                    m_firstLine = 0;
                }
            }
            else
            {
                m_blockSizes = null;
                m_blockMap = null;
                m_firstLine = 0;
            }

            m_name = name;
            m_descriptor = descriptor;
            m_status = status;
        }

        public static MethodDescriptor ReadExternal(EmmaBinaryReader ebr)
        {
            string name = ebr.ReadUTF();
            string descriptor = ebr.ReadUTF();

            int status = ebr.ReadInt32();

            int[] blockSizes = null;
            int[][] blockMap = null;
            int firstLine = 0;

            if ((status & DataConstants.METHOD_NO_BLOCK_DATA) == 0)
            {
                blockSizes = DataFactory.ReadIntArray(ebr);
                if ((status & DataConstants.METHOD_NO_LINE_DATA) == 0)
                {
                    int length = ebr.ReadInt32();
                    blockMap = new int[length][];
                    for (int i = 0; i < length; ++i)
                    {
                        blockMap[i] = DataFactory.ReadIntArray(ebr);
                    }
                    firstLine = ebr.ReadInt32();
                }
            }

            return new MethodDescriptor(name,descriptor,status,blockSizes,blockMap,firstLine);
        }
    }
}
