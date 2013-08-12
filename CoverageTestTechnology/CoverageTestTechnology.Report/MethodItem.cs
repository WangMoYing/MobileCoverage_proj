using System;
using CoverageTestTechnology;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CoverageTestTechnology.EmmaData;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class MethodItem : Item
    {
        private int m_ID;        
        private string m_name;
        private string m_descriptor;
        private int m_firstLine;
        private string m_userName;
        private HashSet<string>[] m_coveredCasesMap;

        public override string Name
        {
            get 
            {
                if (m_userName == null)
                {
                    m_userName = VMNameConvert.MethodVMNameToJavaName(m_parent.Name, m_name, m_descriptor, true, true, true);
                }

                return m_userName;
            }
        }

        public HashSet<string>[] CoveredCasesMap 
        {
            get 
            {
                return m_coveredCasesMap;
            }
        }

        public int ID
        {
            get { return m_ID; }
        }
        public int FirstLine
        {
            get { return m_firstLine; }
        }

        public int HashCode { set;get;}

        public bool IsCovered 
        {
            get 
            {
                return m_coveredCasesMap != null && m_coveredCasesMap.Length > 0;
            }
        }

        public MethodItem(Item parent, int ID, string name, string descriptor, int firstLine)
            : base(parent)
        {
            m_ID = ID;
            m_name = name;
            m_descriptor = descriptor;
            m_firstLine = firstLine;
        }


        public override XmlElement ToXml(XmlDocument ownerDocument)
        {
            XmlElement el = ownerDocument.CreateElement("function");
            el.SetAttribute("name", m_name);
            el.SetAttribute("id", m_ID.ToString());
       
            StringBuilder sb = new StringBuilder();
            foreach (string cassId in CoveredCases)
            {
                sb.Append(cassId).Append(";");
            }
            el.SetAttribute("coveredCases", sb.ToString().Trim(';'));
            return el;
        }

        public override HashSet<string> CoveredCases
        {
            get
            {
                if (m_coveredCasesMap == null)
                {
                    if (m_coveredCases == null)
                    {
                        m_coveredCases = new HashSet<string>();
                    }
                }
                else
                {
                    if (m_coveredCases == null)
                    {
                        m_coveredCases = new HashSet<string>();
                        for (int i = 0; i < m_coveredCasesMap.Length; i++)
                        {
                            m_coveredCases.UnionWith(m_coveredCasesMap[i]);
                        }
                    }
                }
                return m_coveredCases;
            }
        }

        public override void AddUp() 
        {
            //这个方法必须优先调用
            AddUpCoveredCases();
            AddUpLines();
            AddUpCoveredLines();
            AddUpCoveredBlocks();
            AddUpBlocks();
        }

        private void AddUpCoveredCases()
        {
            ClassItem parent = m_parent as ClassItem;
            if (parent.IsCovered)
            {
                m_coveredCasesMap = parent.CoveredCasesMap[m_ID];
            }
            else m_coveredCasesMap = new HashSet<string>[0];
        }
        private void AddUpLines()
        {
            lines = new int[]{};
            for (int i = 0; i < Children.Count; i++)
            {
                lines = lines.Union(Children[i].Lines).ToArray<int>();
            }
            totalLinesCount = lines.Length;
        }
        private void AddUpCoveredLines() 
        {
            // int覆盖行号，bool完全覆盖
            coveredLines = new Dictionary<int, bool>();
            if (IsCovered)
            {
                ClassItem parentClass = Parent as ClassItem;
                Dictionary<int, HashSet<int>> lineMap = parentClass.Descriptor.Methods[m_ID].LineMap;
                foreach (int key in lineMap.Keys)
                {
                    bool fullyCovered = true;
                    bool unCovered = false;
                    foreach (int index in lineMap[key])
                    {
                        fullyCovered &= (m_coveredCasesMap[index].Count > 0);
                        unCovered |= (m_coveredCasesMap[index].Count > 0);
                    }
                    if (fullyCovered)
                    {
                        coveredLines.Add(key, true);
                    }
                    else if (!unCovered)
                    {
                        coveredLines.Add(key, false);
                    }
                }
            }
            coveredLinesCount = coveredLines.Count;
        }
        private void AddUpCoveredBlocks() 
        {
            coveredBlocksCount = 0;
            if (IsCovered)
            {
                for (int i = 0; i < CoveredCasesMap.Length; i++)
                {
                    if (CoveredCasesMap[i].Count > 0)
                    {
                        coveredBlocksCount++;
                    }
                }
            }
        }
        private void AddUpBlocks() 
        {
            totalBlocksCount = Children.Count;
        }
    }
}
