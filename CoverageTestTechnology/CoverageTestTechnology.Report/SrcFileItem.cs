using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class SrcFileItem : Item
    {
        private string m_name;
        private string m_fullVMName;
        private int m_firstLine;
        

        public override string Name
        {
            get { return m_name; }
        }

        public string FullName 
        {
            get { return m_fullVMName; }
        }

        public SrcFileItem(Item parent, string name, string fullVMName)
            : base(parent)
        {
            m_name = name;
            m_fullVMName = fullVMName;
        }

        public override HashSet<string> CoveredCases
        {
            get 
            {
                if (m_coveredCases == null) 
                {
                    m_coveredCases = new HashSet<string>();
                    foreach (Item child in Children)
                    {
                        m_coveredCases.UnionWith(child.CoveredCases);
                    }
                }
                return m_coveredCases;
            }
        }

        public override XmlElement ToXml(XmlDocument ownerDocument)
        {
            XmlElement el = ownerDocument.CreateElement("srcFile");
            el.SetAttribute("name", m_name);
            foreach (Item child in Children)
            {
                el.AppendChild(child.ToXml(ownerDocument));
            }
            return el;
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
        }
    }
}
