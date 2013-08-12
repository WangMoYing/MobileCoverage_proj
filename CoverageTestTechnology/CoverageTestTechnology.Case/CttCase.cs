using System;
using System.Xml.Serialization;

namespace CoverageTestTechnology.Case
{
    [Serializable]
    public class CttCase
    {
        public string CaseId { set; get; }

        public string ResultPath { set; get; }
    }
}
