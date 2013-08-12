using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using CoverageTestTechnology.EmmaData;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class ClassItem : Item
    {
        private ClassDescriptor m_cls;
        private bool[][] m_coverage;
        private int m_firstLine = 0;
        private int m_lastLine = 0;
        private HashSet<string>[][] m_coveredCasesMap;

        public int HashCode { set; get; }

        public int FirstLine
        {
            get
            {
                if (m_firstLine == 0)
                {
                    MethodDescriptor[] methods = m_cls.Methods;

                    int firstLine = Int32.MaxValue;
                    for (int m = 0, mLimit = methods.Length; m < mLimit; ++m)
                    {
                        int mFirstLine = methods[m].FirstLine;
                        if ((mFirstLine > 0) && (mFirstLine < firstLine))
                            firstLine = mFirstLine;
                    }

                    m_firstLine = firstLine;
                    return m_firstLine;
                }

                return m_firstLine;
            }
        }

        public int LastLine 
        {
            get
            {
                if (m_lastLine == 0)
                {
                    MethodDescriptor[] methods = m_cls.Methods;

                    int lastLine = Int32.MinValue;
                    for (int m = 0, mLimit = methods.Length; m < mLimit; ++m)
                    {
                        int mLastLine = methods[m].LastLine;
                        if ((mLastLine > 0) && (mLastLine > lastLine))
                            lastLine = mLastLine;
                    }

                    m_lastLine = lastLine;
                    return m_lastLine;
                }

                return m_lastLine;
            }
        }

        public ClassDescriptor Descriptor 
        {
            get { return m_cls; }
        }

        public bool[][] Coverage { get { return m_coverage; } }

        public HashSet<string>[][] CoveredCasesMap
        {
            get { return m_coveredCasesMap; }
        }

        public bool IsCovered 
        {
            get 
            {
                return CoveredCasesMap != null && CoveredCasesMap.Length > 0;
            }
        }

        public ClassItem(Item parent, ClassDescriptor cls)
            : base(parent)
        {
            m_cls = cls;
        }

        public ClassItem(Item parent, ClassDescriptor cls, bool[][] coverage)
            : base(parent)
        {
            m_cls = cls;
            m_coverage = coverage;
        }

        public ClassItem(Item parent, ClassDescriptor cls, HashSet<string>[][] coverage)
            : base(parent)
        {
            m_cls = cls;
            m_coveredCasesMap = coverage;
            //m_coverage = new bool[coverage.Length][];
            //for (int i = 0; i < coverage.Length; i++) 
            //{
            //    bool[] blockCoverage = new bool[coverage[i].Length];
            //    for (int j = 0; j < coverage[i].Length; j++) 
            //    {
            //        blockCoverage[j] = coverage[i][j].Count > 0;
            //    }
            //    m_coverage[i] = blockCoverage;
            //}
        }

        public override string Name
        {
            get
            {
                return m_cls.Name;
            }
        }

        public override XmlElement ToXml(XmlDocument ownerDocument)
        {
            XmlElement el = ownerDocument.CreateElement("class");
            el.SetAttribute("name", m_cls.Name);
            foreach (Item child in Children)
            {
                el.AppendChild(child.ToXml(ownerDocument));
            }
            return el;
        }

        public override HashSet<string> CoveredCases
        {
            get 
            {
                return m_coveredCases;
            }
        }

        public override void AddUp() 
        {
            lines = new int[] { };
            coveredBlocksCount = 0;
            totalBlocksCount = 0;
            coveredLines = new Dictionary<int, bool>();
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].AddUp();
                lines=lines.Union(Children[i].Lines).ToArray<int>();
                coveredLines = UnionLineCoverage(coveredLines, Children[i].CoveredLines);
                coveredBlocksCount += Children[i].CoveredBlocksCount;
                totalBlocksCount += Children[i].TotlaBlocksCount;
            }
            totalLinesCount = lines.Length;
            coveredLinesCount = coveredLines.Count;
            AddUpCoveredCases();
        }

        private void AddUpCoveredCases()
        {
            m_coveredCases = new HashSet<string>();
            if (IsCovered)
            {
                for (int i = 0; i < m_coveredCasesMap.Length; i++)
                {
                    for (int j = 0; j < m_coveredCasesMap[i].Length; j++)
                    {
                        m_coveredCases.UnionWith(m_coveredCasesMap[i][j]);
                    }
                }
            }
        }
    }
}
