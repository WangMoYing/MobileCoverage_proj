using System;
using System.Collections.Generic;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    [Serializable]
    public class CoverageOptionsDescriptor
    {
        //private bool m_excludeEmptyClasses;
        private bool m_excludeSyntheticMethods;
        private bool m_excludeBridgeMethods;
        private bool m_doSUIDCompensation;

        public bool ExcludeSyntheticMethods
        {
            get { return m_excludeSyntheticMethods; }
        }

        public bool ExcludeBridgeMethods
        {
            get { return m_excludeBridgeMethods; }
        }

        public bool DoSUIDCompensation
        {
            get { return m_doSUIDCompensation; }
        }

        private CoverageOptionsDescriptor(bool excludeSyntheticMethods,
                     bool excludeBridgeMethods,
                     bool doSUIDCompensation)
        {
            //m_excludeEmptyClasses = excludeEmptyClasses;
            m_excludeSyntheticMethods = excludeSyntheticMethods;
            m_excludeBridgeMethods = excludeBridgeMethods;
            m_doSUIDCompensation = doSUIDCompensation;
        }

        public static  CoverageOptionsDescriptor ReadExternal(EmmaBinaryReader ebr)
        {
            //bool excludeEmptyClasses = br.ReadBoolean();
            //bool excludeSyntheticMethods = ebr.ReadBoolean();
            //bool excludeBridgeMethods = ebr.ReadBoolean();
            //bool doSUIDCompensation = ebr.ReadBoolean();
            return new CoverageOptionsDescriptor(ebr.ReadBoolean(), ebr.ReadBoolean(), ebr.ReadBoolean());
        }
    }
}
