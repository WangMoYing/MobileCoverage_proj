using System;
using System.Collections.Generic;
using System.Text;
using CoverageTestTechnology.EmmaData;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class ReportDataModel
    {
        private RootItem m_view = null;
        private MetaDataDescriptor m_mdata = null;
        private SourceCache m_sourceCache = null;
        Dictionary<string, PackageItem> m_packageMap = null;
        Dictionary<string, SrcFileItem> m_srcFileMap = null;
        Dictionary<string, ClassItem> m_classMap = null;

        public Dictionary<string, PackageItem> PackageMap 
        { 
            get { return m_packageMap; } 
        }

        public Dictionary<string, SrcFileItem> SrcFileMap
        {
            get { return m_srcFileMap; }
        }

        public Dictionary<string, ClassItem> ClassMap
        {
            get { return m_classMap; }
        }

        public SourceCache SourceCache
        {
            get
            {
                return m_sourceCache;
            }
        }

        public void SetSrcFileRootPath(string srcRoot) 
        {
            m_sourceCache = new SourceCache(srcRoot);
        }

        public RootItem View 
        {
            get { return m_view; }
        }

        public RootItem CreateView(MetaDataDescriptor mdata, CoverageDataDescriptor cdata, bool srcView)
        {
            if (mdata == null) throw new ArgumentNullException("null input: mdata");
            if (cdata == null) throw new ArgumentNullException("null input: cdata");

            m_mdata = mdata;

            if (srcView && !mdata.HasSrcFileData)
                throw new ArgumentException("source file data view requested for metadata with incomplete SourceFile debug info");

            m_view = new RootItem();
            m_packageMap = new Dictionary<string, PackageItem>();
            m_srcFileMap = new Dictionary<string, SrcFileItem>();
            m_classMap = new Dictionary<string, ClassItem>();
            PackageItem packageItem = null;

            foreach (string key in mdata.ClassMap.Keys)
            {
                ClassDescriptor cls = mdata.ClassMap[key];
                string packageVMName = cls.PackageVMName;

                if (!m_packageMap.ContainsKey(packageVMName))
                {
                    string packageName = packageVMName.Length == 0 ? "default package" : VMNameConvert.VMNameToJavaName(packageVMName);
                    packageItem = new PackageItem(m_view, packageName, packageVMName);
                    m_packageMap.Add(packageVMName, packageItem);
                    m_view.AddChild(packageItem);
                }
                else packageItem = m_packageMap[packageVMName];

                SrcFileItem srcfileItem = null;
                if (srcView)
                {
                    string srcFileName = cls.SrcFileName;
                    if (srcFileName == null)
                        throw new ArgumentNullException("src file name = null");

                    string fullSrcFileName = VMNameConvert.CombineVMName(packageVMName, srcFileName);

                    if (!m_srcFileMap.ContainsKey(fullSrcFileName))
                    {
                        srcfileItem = new SrcFileItem(packageItem, srcFileName, fullSrcFileName);
                        m_srcFileMap.Add(fullSrcFileName, srcfileItem);

                        packageItem.AddChild(srcfileItem);
                    }
                    else srcfileItem = m_srcFileMap[fullSrcFileName];
                }

                DataHolder data = cdata.GetCoverage(cls);

                if (data != null)
                {
                    if (data.Stamp != cls.Stamp)
                        throw new Exception("class stamp mismatch:" + VMNameConvert.VMNameToJavaName(cls.ClassVMName));
                }

                bool[][] coverage = (data != null ? data.Coverage : null);

                ClassItem classItem = srcView ? new ClassItem(srcfileItem, cls, coverage) : new ClassItem(packageItem, cls, coverage);
                MethodDescriptor[] methods = cls.Methods;

                for (int m = 0; m < methods.Length; ++m)
                {
                    MethodDescriptor method = methods[m];

                    if ((method.Status & DataConstants.METHOD_NO_BLOCK_DATA) != 0)
                        continue;
                    MethodItem methodItem = new MethodItem(classItem, m, method.Name, method.Descriptor, method.FirstLine);
                    classItem.AddChild(methodItem);
                }

                if (srcView)
                {
                    srcfileItem.AddChild(classItem);
                    m_classMap.Add(classItem.Name, classItem);
                }
                else
                    packageItem.AddChild(classItem);
            }
            return m_view;
        }

        public RootItem CreateViewForCaseCoverage(MetaDataDescriptor mdata, CaseCoverageDescriptor ccdata, string srcRootPath)
        {
            if (mdata == null) throw new ArgumentNullException("null input: mdata");
            if (ccdata == null) throw new ArgumentNullException("null input: ccdata");
            m_mdata = mdata;
            bool srcView = !string.IsNullOrEmpty(srcRootPath);
            if (srcView && !mdata.HasSrcFileData)
                throw new ArgumentException("source file data view requested for metadata with incomplete SourceFile debug info");

            m_view = new RootItem();
            m_packageMap = new Dictionary<string, PackageItem>();
            m_srcFileMap = new Dictionary<string, SrcFileItem>();
            m_classMap = new Dictionary<string, ClassItem>();
            PackageItem packageItem = null;

            foreach (string key in mdata.ClassMap.Keys)
            {
                ClassDescriptor cls = mdata.ClassMap[key];
                string packageVMName = cls.PackageVMName;

                if (!m_packageMap.ContainsKey(packageVMName))
                {
                    string packageName = packageVMName.Length == 0 ? "default package" : VMNameConvert.VMNameToJavaName(packageVMName);
                    packageItem = new PackageItem(m_view, packageName, packageVMName);
                    m_packageMap.Add(packageVMName, packageItem);
                    m_view.AddChild(packageItem);
                }
                else packageItem = m_packageMap[packageVMName];

                SrcFileItem srcfileItem = null;
                if (srcView)
                {
                    string srcFileName = cls.SrcFileName;
                    if (srcFileName == null)
                        throw new ArgumentNullException("src file name = null");

                    string fullSrcFileName = VMNameConvert.CombineVMName(packageVMName, srcFileName);

                    if (!m_srcFileMap.ContainsKey(fullSrcFileName))
                    {
                        srcfileItem = new SrcFileItem(packageItem, srcFileName, fullSrcFileName);
                        m_srcFileMap.Add(fullSrcFileName, srcfileItem);

                        packageItem.AddChild(srcfileItem);
                    }
                    else srcfileItem = m_srcFileMap[fullSrcFileName];
                }

                CaseCoverageDataHolder data = ccdata.GetCoverage(cls);

                if (data != null)
                {
                    if (data.Stamp != cls.Stamp)
                        throw new Exception("class stamp mismatch:" + VMNameConvert.VMNameToJavaName(cls.ClassVMName));
                }

                HashSet<string>[][] coverage = (data != null ? data.CaseCoverage : null);

                ClassItem classItem = srcView ? new ClassItem(srcfileItem, cls, coverage) : new ClassItem(packageItem, cls, coverage);
                MethodDescriptor[] methods = cls.Methods;

                for (int m = 0; m < methods.Length; ++m)
                {
                    MethodDescriptor method = methods[m];

                    if ((method.Status & DataConstants.METHOD_NO_BLOCK_DATA) != 0)
                        continue;
                    MethodItem methodItem = new MethodItem(classItem, m, method.Name, method.Descriptor, method.FirstLine);
                    classItem.AddChild(methodItem);
                    for (int b = 0; b < method.BlockMap.Length; b++) 
                    {
                        BlockItem blockItem = new BlockItem(methodItem, b, method.BlockMap[b], method.BlockSizes[b]);
                        methodItem.AddChild(blockItem);
                    }
                }

                if (srcView)
                {
                    srcfileItem.AddChild(classItem);
                    m_classMap.Add(classItem.Name, classItem);
                }
                else
                    packageItem.AddChild(classItem);
                
            }
            ComputeHashCode(srcRootPath);
            return m_view;
        }

        public RootItem CreateViewForDiff(MetaDataDescriptor mdata,string srcRootPath) 
        {
            if (mdata == null) throw new ArgumentNullException("null input: mdata");
            m_mdata = mdata;

            m_view = new RootItem();
            m_packageMap = new Dictionary<string, PackageItem>();
            m_srcFileMap = new Dictionary<string, SrcFileItem>();
            m_classMap = new Dictionary<string, ClassItem>();
            PackageItem packageItem = null;

            foreach (string key in mdata.ClassMap.Keys)
            {
                ClassDescriptor cls = mdata.ClassMap[key];
                string packageVMName = cls.PackageVMName;

                if (!m_packageMap.ContainsKey(packageVMName))
                {
                    string packageName = packageVMName.Length == 0 ? "default package" : VMNameConvert.VMNameToJavaName(packageVMName);
                    packageItem = new PackageItem(m_view, packageName, packageVMName);
                    m_packageMap.Add(packageVMName, packageItem);
                    m_view.AddChild(packageItem);
                }
                else packageItem = m_packageMap[packageVMName];

                SrcFileItem srcfileItem = null;

                string srcFileName = cls.SrcFileName;
                if (srcFileName == null)
                    throw new ArgumentNullException("src file name = null");

                string fullSrcFileName = VMNameConvert.CombineVMName(packageVMName, srcFileName);

                if (!m_srcFileMap.ContainsKey(fullSrcFileName))
                {
                    srcfileItem = new SrcFileItem(packageItem, srcFileName, fullSrcFileName);
                    m_srcFileMap.Add(fullSrcFileName, srcfileItem);

                    packageItem.AddChild(srcfileItem);
                }
                else srcfileItem = m_srcFileMap[fullSrcFileName];


                ClassItem classItem = new ClassItem(srcfileItem, cls);
                MethodDescriptor[] methods = cls.Methods;

                for (int m = 0; m < methods.Length; ++m)
                {
                    MethodDescriptor method = methods[m];

                    if ((method.Status & DataConstants.METHOD_NO_BLOCK_DATA) != 0)
                        continue;
                    MethodItem methodItem = new MethodItem(classItem, m, method.Name, method.Descriptor, method.FirstLine);
                    classItem.AddChild(methodItem);
                    for (int b = 0; b < method.BlockMap.Length; b++)
                    {
                        int blockSize = method.BlockSizes == null ? -1 : method.BlockSizes[b];
                        BlockItem blockItem = new BlockItem(methodItem, b, method.BlockMap[b], blockSize);
                        methodItem.AddChild(blockItem);
                    }
                }

                srcfileItem.AddChild(classItem);
                m_classMap.Add(classItem.Name, classItem);
            }
            ComputeHashCode(srcRootPath);
            return m_view;
        }

        public RootItem ComputeHashCode(string srcRootPath)
        {
            m_sourceCache = new SourceCache(srcRootPath);
            List<string> lines;
            StringBuilder sb;
            string result;
            foreach (ClassItem classItem in m_classMap.Values)
            {
                lines = m_sourceCache.GetSource(classItem.Descriptor.SrcFileName);
                sb = new StringBuilder();
                for (int l = classItem.FirstLine; l <=classItem.LastLine; l++)
                {
                    sb.Append(lines[l]);
                }
                classItem.HashCode = sb.ToString().GetHashCode();
                for (int i = 0; i < classItem.Descriptor.Methods.Length; i++)
                {
                    MethodDescriptor method = classItem.Descriptor.Methods[i];
                    sb = new StringBuilder();
                    for (int j = method.FirstLine; j <= method.LastLine; j++)
                    {
                        sb.Append(lines[j]);
                    }
                    result = sb.ToString();
                    ((MethodItem)classItem.Children[i]).HashCode = result.GetHashCode();
                    foreach (Item child in classItem.Children[i].Children)
                    {
                        BlockItem block = child as BlockItem;
                        sb = new StringBuilder();
                        for (int k = 0; k < block.Lines.Length; k++)
                        {
                            sb.Append(lines[block.Lines[k]]);
                        }
                        result = sb.ToString();
                        block.HashCode = result.GetHashCode();
                    }
                }
            }
            return m_view;
        }

        public DiffItem Compare(ReportDataModel oldReportData) 
        {
            HashSet<int> currentHash = new HashSet<int>();
            HashSet<int> currentHashForExpect = new HashSet<int>();
            HashSet<int> previousHash = new HashSet<int>();
            
            Dictionary<int, Dictionary<string, BlockItem>> currentBlocks = new Dictionary<int, Dictionary<string, BlockItem>>();
            Dictionary<int, Dictionary<string, BlockItem>> previousBlocks = new Dictionary<int, Dictionary<string, BlockItem>>();
            foreach (string key in oldReportData.ClassMap.Keys) 
            {
                if (m_classMap.ContainsKey(key) && oldReportData.ClassMap[key].HashCode == m_classMap[key].HashCode)
                {
                    continue;
                }
                foreach (Item methodItem in oldReportData.ClassMap[key].Children) 
                {
                    foreach (Item blockItem in methodItem.Children) 
                    {
                        BlockItem block=blockItem as BlockItem;
                        int hash=block.HashCode;
                        previousHash.Add(hash);
                        if (!previousBlocks.ContainsKey(hash))
                        {
                            Dictionary<string, BlockItem> previousBlockMap = new Dictionary<string, BlockItem>();
                            previousBlockMap.Add(block.Name, block);
                            previousBlocks.Add(hash, previousBlockMap);
                        }
                        else 
                        {
                            if (!previousBlocks[hash].ContainsKey(block.Name)) 
                            {
                                previousBlocks[hash].Add(block.Name, block);
                            }
                        }
                    }
                }
            }

            foreach (string key in m_classMap.Keys)
            {
                if (oldReportData.ClassMap.ContainsKey(key) && oldReportData.ClassMap[key].HashCode == m_classMap[key].HashCode)
                {
                    continue;
                }
                foreach (Item methodItem in m_classMap[key].Children)
                {
                    foreach (Item blockItem in methodItem.Children)
                    {
                        BlockItem block = blockItem as BlockItem;
                        int hash = block.HashCode;
                        currentHash.Add(hash);
                        currentHashForExpect.Add(hash);
                        if (!currentBlocks.ContainsKey(hash))
                        {
                            Dictionary<string, BlockItem> currentBlockMap = new Dictionary<string, BlockItem>();
                            currentBlockMap.Add(block.Name, block);
                            currentBlocks.Add(hash, currentBlockMap);
                        }
                        else
                        {
                            if (!currentBlocks[hash].ContainsKey(block.Name))
                            {
                                currentBlocks[hash].Add(block.Name, block);
                            }
                        }
                    }
                }
            }
         
            currentHash.ExceptWith(previousHash);
            previousHash.ExceptWith(currentHashForExpect);

            DiffItem diffItem = new DiffItem();

            foreach (int i in currentHash) 
            {
                foreach (BlockItem item in currentBlocks[i].Values)
                {
                    diffItem.AddedBlocks.Add(item);
                }
            }

            foreach (int i in previousHash)
            {
                foreach (BlockItem item in previousBlocks[i].Values)
                {
                    diffItem.ReducedBlocks.Add(item);
                }
            }

            return diffItem;
        }

        public DiffItem CheckCoverage(DiffItem diffItem) 
        {
            Predicate<BlockItem> predicate = new Predicate<BlockItem>(IsCovered);
            diffItem.AddedBlocks.RemoveWhere(predicate);
            return diffItem;
        }

        private bool IsCovered(BlockItem blockItem)
        {
            string key = blockItem.Parent.Parent.Name;
            bool isCovered = false;
            if (m_classMap.ContainsKey(key)) 
            {
                foreach (Item method in m_classMap[key].Children) 
                {
                    if (method.Name.Equals(blockItem.Parent.Name)) 
                    {
                        foreach (Item block in method.Children) 
                        {
                            if (((BlockItem)block).ID == blockItem.ID) 
                            {
                                isCovered = true;
                                break;
                            }
                        }
                    }
                }
            }
            return isCovered;
        }
    }
}
