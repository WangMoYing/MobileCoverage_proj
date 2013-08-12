using System;
using System.Collections.Generic;
using System.Xml;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public abstract class Item
    {
        private List<Item> m_children;
        protected Item m_parent;
        protected int[] m_aggregates;
        protected HashSet<string> m_coveredCases;
        protected int totalBlocksCount = 0;
        protected int coveredBlocksCount = 0;
        protected int totalLinesCount = 0;
        protected int coveredLinesCount = 0;
        protected int[] lines;
        protected Dictionary<int, bool> coveredLines;

        public List<Item> Children
        {
            get { return m_children; }
        }
        public Item Parent
        {
            get { return m_parent; }
        }

        public abstract string Name { get; }

        public int[] Lines
        {
            get
            {
                return lines;
            }
        }
        public int TotalLinesCount 
        { 
            get 
            { 
                return totalLinesCount;
            } 
        }
        public Dictionary<int, bool> CoveredLines
        {
            get
            {
                return coveredLines;
            }
        }
        public int CoveredLinesCount 
        {
            get
            {
                return coveredLinesCount;
            }
        }
        public int CoveredBlocksCount
        {
            get
            {
                return coveredBlocksCount;
            }
        }
        public int TotlaBlocksCount
        {
            get
            {
                return totalBlocksCount;
            }
        }

        public Item(Item parent)
        {
            m_parent = parent;
            m_children = new List<Item>();
        }

        public void AddChild(Item item)
        {
            if (item == null) throw new ArgumentNullException("null input: item");
            m_children.Add(item);
        }

        protected Dictionary<int, bool> UnionLineCoverage(Dictionary<int, bool> a,Dictionary<int, bool> b) 
        {
            foreach (KeyValuePair<int, bool> kv in b) 
            {
                if (a.ContainsKey(kv.Key))
                {
                    if (kv.Value)
                    {
                        a[kv.Key] = kv.Value;
                    }
                }
                else 
                {
                    a.Add(kv.Key, kv.Value);
                }
            }
            return a;
        }

        public abstract HashSet<string> CoveredCases { get; }

        public abstract XmlElement ToXml(XmlDocument ownerDocument);

        public abstract void AddUp();

    }
}
