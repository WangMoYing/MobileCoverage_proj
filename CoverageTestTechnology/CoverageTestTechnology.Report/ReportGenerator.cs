using System;
using System.Collections.Generic;
using CoverageTestTechnology;
using System.Xml;

namespace CoverageTestTechnology.Report
{
    public class ReportGenerator
    {
        //RootItem m_view = null;

        public static void Process(RootItem view,string outPath) 
        {
            //m_view = view;
            //m_view.Accept(this, null);

            XmlDocument xml = new XmlDocument();
            XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            xml.AppendChild(dec);
            string PItext = @"type='text/xsl' href='ReportStyle.xsl'";
            XmlProcessingInstruction newPI = xml.CreateProcessingInstruction("xml-stylesheet", PItext);
            xml.AppendChild(newPI);
            xml.AppendChild(view.ToXml(xml));
            xml.Save(outPath);
        }

        //public object visit(RootItem item, object ctx)
        //{
        //    XmlDocument xml=new XmlDocument();
        //    XmlDeclaration dec = xml.CreateXmlDeclaration("1.0","UTF-8",null);
        //    xml.AppendChild(dec);
        //    string PItext = @"type='text/xsl' href='ReportStyle.xsl'";
        //    XmlProcessingInstruction newPI=xml.CreateProcessingInstruction("xml-stylesheet",PItext);
        //    xml.AppendChild(newPI);
        //    XmlElement element= xml.CreateElement("ApplicationCoverage");
        //    element.SetAttribute("name", item.Name);
        //}

        //public object visit(PackageItem item, object ctx)
        //{
        //    return ctx;
        //}

        //public object visit(SrcFileItem item, object ctx)
        //{
        //    return ctx;
        //}

        //public object visit(ClassItem item, object ctx)
        //{
        //    return ctx;
        //}

        //public object visit(MethodItem item, object ctx)
        //{
        //    return ctx;
        //}
    }
}
