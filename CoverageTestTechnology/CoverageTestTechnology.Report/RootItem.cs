using System;
using System.Xml;
using System.Collections.Generic;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class RootItem:Item
    {
        public override string Name
        {
            get
            {
                return "All classes";
            }
        }

        public RootItem() : base(null) 
        { }


        public override XmlElement ToXml(XmlDocument ownerDocument)
        {
            XmlElement el = ownerDocument.CreateElement("applicationCoverage");
            el.SetAttribute("name", Name);
            foreach (Item child in Children)
            {
                el.AppendChild(child.ToXml(ownerDocument));
            }
            return el;
        }

        public override HashSet<string> CoveredCases
        {
            get { throw new NotImplementedException(); }
        }

        public override void AddUp()
        {
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
