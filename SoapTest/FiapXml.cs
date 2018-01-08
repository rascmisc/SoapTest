using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace SoapTest
{
    public class FiapXml
    {
        public string xml;
        public FiapXml()
        {
        }
        public void MakeFiapMessage(string keyid,string LBtime,string UBtime)
        { 
            StringWriterUTF8 writer = new StringWriterUTF8();
            XmlDocument document = new XmlDocument();
            try
            {
                XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", null, null);  // XML宣言
                document.AppendChild(declaration);

                XmlElement root;
                XmlNode x1, x2, x3, x4, x5, x6;
                root = document.CreateElement("soapenv","Envelope","xmlns");
                root.SetAttribute("xmlns:soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                root.SetAttribute("xmlns:soap", "http://soap.fiap.org/");
                root.SetAttribute("xmlns:ns", "http://gutp.jp/fiap/2009/11/");


                x1 = document.CreateNode(XmlNodeType.Element, "soapenv:Body", "http://schemas.xmlsoap.org/soap/envelope/");
                x2 = document.CreateNode(XmlNodeType.Element, "soap:queryRQ", "http://soap.fiap.org/");
                x3 = document.CreateNode(XmlNodeType.Element, "ns:transport", "http://gutp.jp/fiap/2009/11/");
                x4 = document.CreateNode(XmlNodeType.Element, "ns:header", "http://gutp.jp/fiap/2009/11/");
                x5 = document.CreateNode(XmlNodeType.Element, "ns:query", "http://gutp.jp/fiap/2009/11/");
                XmlAttribute attr;

                attr = document.CreateAttribute("id");
                attr.Value = "0a90f1fa-bdb4-48ff-87d3-661d2af6ff4c";
                x5.Attributes.SetNamedItem(attr);
                attr = document.CreateAttribute("type");
                attr.Value = "storage";
                x5.Attributes.SetNamedItem(attr);
                root.AppendChild(x1);
                x1.AppendChild(x2);
                x2.AppendChild(x3);
                x3.AppendChild(x4);
                x4.AppendChild(x5);

                string[] lines = keyid.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if (LBtime == "")
                {
                    foreach (string line in lines)
                    {
                        x6 = document.CreateNode(XmlNodeType.Element, "ns:key", "http://gutp.jp/fiap/2009/11/");

                        attr = document.CreateAttribute("id");
                        attr.Value = line;
                        x6.Attributes.SetNamedItem(attr);
                        attr = document.CreateAttribute("attrName");
                        attr.Value = "time";
                        x6.Attributes.SetNamedItem(attr);
                        attr = document.CreateAttribute("select");
                        attr.Value = "maximum";
                        x6.Attributes.SetNamedItem(attr);
                        x5.AppendChild(x6);
                    }
                }
                else
                {
                    foreach (string line in lines)
                    {
                        x6 = document.CreateNode(XmlNodeType.Element, "ns:key", "http://gutp.jp/fiap/2009/11/");

                        attr = document.CreateAttribute("id");
                        attr.Value = line;
                        x6.Attributes.SetNamedItem(attr);
                        attr = document.CreateAttribute("attrName");
                        attr.Value = "time";
                        x6.Attributes.SetNamedItem(attr);
                        attr = document.CreateAttribute("lteq");
                        attr.Value = LBtime;
                        x6.Attributes.SetNamedItem(attr);
                        attr = document.CreateAttribute("gteq");
                        attr.Value = UBtime;
                        x6.Attributes.SetNamedItem(attr);
                        x5.AppendChild(x6);
                    }

                }
                document.AppendChild(root);//xmlDocumentオブジェクトにルートノード追加

                document.Save(writer);
                xml = writer.ToString();
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
    class StringWriterUTF8 : System.IO.StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
