using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.Report
{
    public class DiffItem
    {
        public HashSet<BlockItem> AddedBlocks { set; get; }
        public HashSet<BlockItem> ReducedBlocks { set; get; }

        public DiffItem() 
        {
            AddedBlocks = new HashSet<BlockItem>();
            ReducedBlocks = new HashSet<BlockItem>();
        }
    }
}
