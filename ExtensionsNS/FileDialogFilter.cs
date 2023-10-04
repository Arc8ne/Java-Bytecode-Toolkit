using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Java_Bytecode_Toolkit.ExtensionsNS
{
    public class FileDialogFilter
    {
        public static readonly FileDialogFilter JAVA_CLASS_FILE_FILTER = new FileDialogFilter(
            "Java Class Files",
            "class"
        );

        public static readonly FileDialogFilter JAR_FILE_FILTER = new FileDialogFilter(
            "Executable JAR Files",
            "jar"
        );

        public static readonly FileDialogFilter XML_FILE_FILTER = new FileDialogFilter(
            "XML Files",
            "xml"
        );

        public string filterName = "";

        public string[] fileExtensions = new string[] { };

        public FileDialogFilter()
        {

        }

        public FileDialogFilter(string filterName, params string[] fileExtensions)
        {
            this.filterName = filterName;

            this.fileExtensions = fileExtensions;
        }
    }
}
