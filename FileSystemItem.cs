using Java_Bytecode_Toolkit.ExtensionsNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Java_Bytecode_Toolkit
{
    public class FileSystemItem
    {
        public static readonly XmlWriterSettings DEFAULT_XML_WRITER_SETTINGS = new XmlWriterSettings()
        {
            Indent = true,
            NewLineOnAttributes = true
        };

        public string filePath = "";

        public string Name
        {
            get
            {
                return Path.GetFileName(this.filePath);
            }
        }

        public bool IsFile
        {
            get
            {
                return File.Exists(this.filePath);
            }
        }

        public bool IsDirectory
        {
            get
            {
                return Directory.Exists(this.filePath);
            }
        }

        public FileSystemItem()
        {

        }

        public FileSystemItem(string filePath)
        {
            this.filePath = filePath;
        }

        public virtual void ExportAsXMLFile(string exportedXMLFilePath)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(exportedXMLFilePath, DEFAULT_XML_WRITER_SETTINGS))
            {
                xmlWriter.WriteStartElement("FileSystemItem");

                xmlWriter.WriteAttributeString("FilePath", this.filePath);

                xmlWriter.WriteAttributeString("Name", this.Name);

                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
            }
        }
    }
}
