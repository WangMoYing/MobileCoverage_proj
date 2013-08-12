using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class BlockItem:Item
    {
        private int m_ID;
        private int m_size;
        private int m_firstLine = -1;
        private int m_lastLine = -1;
        private string m_name;

        public int ID
        {
            get { return m_ID; }
        }

        public int HashCode { set; get; }

        /// <summary>
        /// 代码块是跳跃的，所以第一行未必就在所属的方法体内
        /// </summary>
        public int FirstLine 
        {
            get
            {
                return m_firstLine;
            }
        }

        /// <summary>
        /// 代码块是跳跃的，所以最后一行未必就在所属的方法体内
        /// </summary>
        public int LastLine 
        {
            get 
            {
                return m_lastLine;
            }
        }

        public BlockItem(Item parent, int ID,int[] lines,int size)
            : base(parent) 
        {
            m_ID = ID;
            m_size = size;
            this.lines = lines;
            Array.Sort(lines);
            m_firstLine = lines[0];
            m_lastLine = lines[lines.Length - 1];
        }

        public override string Name
        {
            get 
            {
                if (string.IsNullOrEmpty(m_name)) 
                {
                    m_name = Parent.Name + "@" + m_ID;
                }
                return m_name;
            }
        }

        public override HashSet<string> CoveredCases
        {
            get
            {
                if (m_coveredCases == null)
                {
                    MethodItem parentMethod = Parent as MethodItem;
                    if (parentMethod.CoveredCasesMap == null)
                    {
                        m_coveredCases = new HashSet<string>();
                    }
                    else
                    {
                        m_coveredCases = parentMethod.CoveredCasesMap[m_ID];
                    }
                }
                return m_coveredCases;
            }
        }

        public override XmlElement ToXml(XmlDocument ownerDocument)
        {
            XmlElement el = ownerDocument.CreateElement("block");
            el.SetAttribute("name", HashCode.ToString());
            el.SetAttribute("id", m_ID.ToString());
            
            StringBuilder sb = new StringBuilder();
            foreach (string cassId in CoveredCases)
            {
                sb.Append(cassId).Append(";");
            }
            el.SetAttribute("coveredCases", sb.ToString().Trim(';'));
            return el;
        }

        public override void AddUp()
        {
            
        }
    }
}
