using System;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class ClassDescriptor
    {
        private string m_packageVMName; // in JVM format, no trailing '/' [never null]
        private string m_name; // relative to (m_packageName + '/') [never null]
        private long m_stamp;
        private string m_srcFileName; // relative to (m_packageName + '/') [can be null]
        private MethodDescriptor[] m_methods; // [never null, could be empty]

        private bool m_hasCompleteLineNumberInfo;
        private int m_hash;

        public string PackageVMName
        {
            get { return m_packageVMName; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public long Stamp
        {
            get { return m_stamp; }
        }

        public string ClassVMName
        {
            get
            {
                if (m_packageVMName.Length == 0)
                    return m_name;
                else
                    return new StringBuilder(m_packageVMName).Append("/").Append(m_name).ToString();
            }
        }

        public string SrcFileName
        {
            get { return m_srcFileName; }
        }

        public MethodDescriptor[] Methods
        {
            get { return m_methods; }
        }

        public bool HasSrcFileInfo
        {
            get { return m_srcFileName != null; }
        }

        public bool HasCompleteLineNumberInfo
        {
            get { return m_hasCompleteLineNumberInfo; }
        }

        private ClassDescriptor(string packageVMName, string name, long stamp, string srcFileName, MethodDescriptor[] methods)
        {
            if (packageVMName == null)
                throw new ArgumentNullException("null input: packageVMName");
            if (name == null)
                throw new ArgumentNullException("null input: name");
            if (methods == null)
                throw new ArgumentNullException("null input: methods");
            m_packageVMName = packageVMName;
            m_name = name;
            m_stamp = stamp;
            m_srcFileName = srcFileName;
            m_methods = methods;
        }

        public static ClassDescriptor ReadExternal(EmmaBinaryReader ebr) 
        {
            string packageVMName = ebr.ReadUTF();
            string name = ebr.ReadUTF();

            long stamp = ebr.ReadLong();

            sbyte srcFileNameFlag = ebr.ReadSbyte();
            string srcFileName = srcFileNameFlag != 0 ? ebr.ReadUTF() : null;

            int length = ebr.ReadInt32();
            MethodDescriptor[] methods = new MethodDescriptor[length];
            for (int i = 0; i < length; ++i) 
            {
                methods[i] = MethodDescriptor.ReadExternal(ebr);
            }
            return new ClassDescriptor(packageVMName,name,stamp,srcFileName,methods);
        }
    }
}
