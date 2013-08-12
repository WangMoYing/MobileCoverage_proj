using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class PackageItem : Item
    {
        private string m_name;       
        private string m_VMname;

        public override string Name
        {
            get { return m_name; }
        }
        public string VMname
        {
            get { return m_VMname; }
        }

        public PackageItem(Item parent,string name,string VMname)
            : base(parent)
        {
            m_name = name;
            m_VMname = VMname;
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
            XmlElement el = ownerDocument.CreateElement("package");
            el.SetAttribute("name", m_name);
            foreach (Item child in Children)
            {
                el.AppendChild(child.ToXml(ownerDocument));
            }
            return el;
        }

        public override void AddUp()
        {
            coveredLinesCount = 0;
            totalLinesCount = 0;
            coveredBlocksCount = 0;
            totalBlocksCount = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].AddUp();
                coveredLinesCount += Children[i].CoveredLinesCount;
                totalLinesCount += Children[i].TotalLinesCount;
                coveredBlocksCount += Children[i].CoveredBlocksCount;
                totalBlocksCount += Children[i].TotlaBlocksCount;
            }

        }
    }
}
