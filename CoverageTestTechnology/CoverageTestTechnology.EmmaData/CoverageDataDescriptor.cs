using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class CoverageDataDescriptor
    {
        private Dictionary<string, DataHolder> m_coverageMap; // never null

        public Dictionary<string, DataHolder> CoverageMap
        {
            get { return m_coverageMap; }
        }

        private CoverageDataDescriptor(Dictionary<string, DataHolder> coverageMap)
        {
            if (coverageMap == null)
                throw new ArgumentNullException("coverageMap is null");
            m_coverageMap = coverageMap;
        }

        public static CoverageDataDescriptor ReadExternal(EmmaBinaryReader ebr)
        {
            int size = ebr.ReadInt32();
            Dictionary<string, DataHolder> coverageMap = new Dictionary<string, DataHolder>();

            for (int i = 0; i < size; ++i)
            {
                string classVMName = ebr.ReadUTF();
                long stamp = ebr.ReadLong();

                int length = ebr.ReadInt32();
                bool[][] coverage = new bool[length][];
                for (int c = 0; c < length; ++c)
                {
                    coverage[c] = DataFactory.readBooleanArray(ebr);
                }

                coverageMap.Add(classVMName, new DataHolder(coverage, stamp));
            }

            return new CoverageDataDescriptor(coverageMap);
        }

        public DataHolder GetCoverage(ClassDescriptor cls)
        {
            if (cls == null)
                throw new ArgumentNullException("null input: cls");
            if (!m_coverageMap.ContainsKey(cls.ClassVMName))
                return null;
            return m_coverageMap[cls.ClassVMName];
        }

    }

    public class DataHolder
    {
        public DataHolder(bool[][] coverage, long stamp)
        {
            Coverage = coverage;
            Stamp = stamp;
        }

        public bool[][] Coverage { set; get; }
        public long Stamp { set; get; }
    }
}
