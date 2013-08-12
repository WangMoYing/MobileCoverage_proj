using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CoverageTestTechnology.Case
{
    [Serializable]
    public class CttCaseCollection
    {
        [XmlElement("CttCase")]
        public List<CttCase> CoverageFilePaths { set; get; }

        public CttCaseCollection() 
        {
            CoverageFilePaths = new List<CttCase>();
        }
    }
}
