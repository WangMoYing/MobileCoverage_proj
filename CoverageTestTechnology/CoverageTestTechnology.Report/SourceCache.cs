using System;
using System.Collections.Generic;
using System.IO;

namespace CoverageTestTechnology.Report
{
    [Serializable]
    public class SourceCache
    {
        private FileInfo[] Files;
        private string m_RootPath;
        private StreamReader reader;
        List<string> m_source;
        public string RootPath 
        {
            get 
            {
                return m_RootPath;
            }
        }

        public List<string> Source
        {
            get
            {
                return m_source;
            }
        }

        public SourceCache(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                throw new ArgumentException("Root Path: " + rootPath + " does not exist.");
            }
            m_RootPath = rootPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            Files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
        }

        public string GetSourcePath(string className) 
        {
            string path = "";
            for (int i = 0; i < Files.Length; i++) 
            {
                if (Files[i].Name.Equals(className)) 
                {
                    path = Files[i].FullName;
                    break;
                }
            }
            return path;
        }

        public List<string> GetSource(string className) 
        {
            reader = new StreamReader(GetSourcePath(className));
            m_source = new List<string>();
            string line = reader.ReadLine();
            while (line != null) 
            {
                m_source.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();
            reader.Dispose();
            return m_source;
        }


    }
}
