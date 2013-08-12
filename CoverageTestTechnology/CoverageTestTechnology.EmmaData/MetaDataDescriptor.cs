using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class MetaDataDescriptor
    {
        private CoverageOptionsDescriptor m_options; // [never null]
        private bool m_hasSrcFileInfo;
        private bool m_hasLineNumberInfo;
        private Dictionary<string, ClassDescriptor> m_classMap; // [never null]

        public bool HasSrcFileData
        {
            get{return m_hasSrcFileInfo;}
        }

        public bool HasLineNumberData
        {
            get { return m_hasLineNumberInfo; }
        }
        public Dictionary<string, ClassDescriptor> ClassMap 
        {
            get { return m_classMap; }
        }

        private MetaDataDescriptor(CoverageOptionsDescriptor options, Dictionary<string, ClassDescriptor> classMap,
                      bool hasSrcFileInfo, bool hasLineNumberInfo)
        {
            if (options == null)
                throw new ArgumentNullException("options is null");
            m_options = options;

            m_hasSrcFileInfo = hasSrcFileInfo;
            m_hasLineNumberInfo = hasLineNumberInfo;

            m_classMap = classMap;
        }

        public static MetaDataDescriptor ReadExternal(EmmaBinaryReader ebr)
        {
            CoverageOptionsDescriptor options = CoverageOptionsDescriptor.ReadExternal(ebr);
            bool hasSrcFileInfo = ebr.ReadBoolean();
            bool hasLineNumberInfo = ebr.ReadBoolean();
            int size = ebr.ReadInt32();
            Dictionary<string, ClassDescriptor> classMap = new Dictionary<string, ClassDescriptor>();
            for (int i = 0; i < size; ++i)
            {
                string className = ebr.ReadUTF();
                ClassDescriptor cls = ClassDescriptor.ReadExternal(ebr);
                classMap.Add(className, cls);
            }
            return new MetaDataDescriptor(options, classMap, hasSrcFileInfo, hasLineNumberInfo);
        }
    }
}
