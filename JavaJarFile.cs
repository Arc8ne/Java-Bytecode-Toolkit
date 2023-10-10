using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Java_Bytecode_Toolkit
{
    public class JavaJarFile : FileSystemItem
    {
        public JavaJarFile() : base()
        {

        }

        public JavaJarFile(string jarFilePath) : base(jarFilePath)
        {

        }

        // TODO: Implement this method once fields and properties (optional) for this
        // class have been finalized.
        public override void ExportAsXMLFile(string exportedXMLFilePath)
        {
            base.ExportAsXMLFile(exportedXMLFilePath);
        }
    }
}
