using System;
using System.Collections.Generic;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class CaseCoverageDescriptor
    {
        private Dictionary<string, CaseCoverageDataHolder> m_coverageMap; // never null

        public CaseCoverageDescriptor()
        {
            m_coverageMap = new Dictionary<string, CaseCoverageDataHolder>();
        }

        public HashSet<string> MergeCaseCoverageData(CoverageDataDescriptor cdata,string caseId)
        {
            HashSet<string> differentStamps = new HashSet<string>();
            foreach (string key in cdata.CoverageMap.Keys) 
            {
                if (!m_coverageMap.ContainsKey(key))
                {
                    m_coverageMap.Add(key, new CaseCoverageDataHolder(cdata.CoverageMap[key].Coverage, cdata.CoverageMap[key].Stamp, caseId));
                }
                else 
                {
                    //标识不一致表示同个类前后两次内容不一致，可能是错误操作引起得，记录类名以供分析
                    if (m_coverageMap[key].Stamp != cdata.CoverageMap[key].Stamp) 
                    {
                        differentStamps.Add(key);
                        continue;
                    }
                    for (int i = 0; i < m_coverageMap[key].CaseCoverage.Length; i++)
                    {
                        for (int j = 0; j < m_coverageMap[key].CaseCoverage[i].Length; j++)
                        {
                            if (cdata.CoverageMap[key].Coverage[i][j])
                            {
                                m_coverageMap[key].CaseCoverage[i][j].Add(caseId);
                            }
                        }
                    }
                }
            }
            return differentStamps;
        }

        public CaseCoverageDataHolder GetCoverage(ClassDescriptor cls)
        {
            if (cls == null)
                throw new ArgumentNullException("null input: cls");
            if (!m_coverageMap.ContainsKey(cls.ClassVMName))
                return null;
            return m_coverageMap[cls.ClassVMName];
        }
    }

    public class CaseCoverageDataHolder
    {
        public CaseCoverageDataHolder(bool[][] coverage, long stamp,string caseId)
        {
            Stamp = stamp;
            CaseCoverage = new HashSet<string>[coverage.Length][];
            for (int i = 0; i < coverage.Length;i++ )
            {
                CaseCoverage[i]=new HashSet<string>[coverage[i].Length];
                for (int j = 0; j < coverage[i].Length;j++ )
                {
                    CaseCoverage[i][j] = new HashSet<string>();
                    if (coverage[i][j])
                    {
                        CaseCoverage[i][j].Add(caseId);
                    }
                }
            }
        }
        public long Stamp { set; get; }
        public HashSet<string>[][] CaseCoverage { set; get; }
    }
}
