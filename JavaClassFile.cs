using Java_Bytecode_Toolkit.ExtensionsNS;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;

namespace Java_Bytecode_Toolkit
{
    public class JavaClassFile
    {
        private static readonly XmlWriterSettings DEFAULT_XML_WRITER_SETTINGS = new XmlWriterSettings()
        {
            Indent = true,
            NewLineOnAttributes = true
        };

        // Note: Everything in Java binary files is stored in big-endian order.
        public const bool IS_JAVA_BINARY_FILE_LITTLE_ENDIAN = false;

        public string filePath = "";

        public UInt32 magic = 0;

        public UInt16 minorVersion = 0;
        
        public UInt16 majorVersion = 0;
        
        public UInt16 constantPoolCount = 0;
        
        // Array size = constantPoolCount - 1
        public ConstantPoolInfo[] constantPool = null;
        
        // Original type: UInt16
        public AccessFlags accessFlags = 0;
        
        public UInt16 thisClass = 0;
        
        public UInt16 superClass = 0;
        
        public UInt16 interfacesCount = 0;
        
        // Array size = interfacesCount
        public UInt16[] interfaces = null;
        
        public UInt16 fieldsCount = 0;

        // Array size = fieldsCount
        public FieldInfo[] fields = null;
        
        public UInt16 methodsCount = 0;

        // Array size = methodsCount
        public MethodInfo[] methods = null;

        public UInt16 attributesCount = 0;

        // Array size = attributesCount
        public AttributeInfo[] attributes = null;

        public String Name
        {
            get
            {
                string[] filePathParts = this.filePath.Split(
                    Path.DirectorySeparatorChar
                );

                return filePathParts[filePathParts.Length - 1];
            }
        }

        public JavaClassFile()
        {

        }

        public JavaClassFile(string javaClassFilePath)
        {
            this.filePath = javaClassFilePath;

            bool readSuccessful = this.Read();
        }

        public bool Read()
        {
            FileStream javaClassFileStream = null;

            try
            {
                javaClassFileStream = File.OpenRead(this.filePath);
            }
            catch
            {
                return false;
            }

            this.magic = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.minorVersion = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.majorVersion = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.constantPoolCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.constantPool = new ConstantPoolInfo[this.constantPoolCount - 1];

            this.ReadConstantPool(javaClassFileStream);

            this.accessFlags = (AccessFlags)javaClassFileStream.ReadBytesFromStreamAs<UInt16>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.thisClass = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.superClass = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.interfacesCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.interfaces = new ushort[this.interfacesCount];

            this.ReadInterfaces(javaClassFileStream);

            this.fieldsCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.fields = new FieldInfo[this.fieldsCount];

            this.ReadFields(javaClassFileStream);

            this.methodsCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.methods = new MethodInfo[this.methodsCount];

            this.ReadMethods(javaClassFileStream);

            this.attributesCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
            );

            this.attributes = new AttributeInfo[this.attributesCount];

            this.ReadAttributes(javaClassFileStream);

            return true;
        }

        // TODO: Handle different types of attributes.
        public void ReadAttributes(FileStream javaClassFileStream)
        {
            for (int currentIndex = 0; currentIndex < this.attributesCount; currentIndex++)
            {
                AttributeInfo currentAttributeInfo = new AttributeInfo();

                currentAttributeInfo.attributeNameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentAttributeInfo.attributeLength = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentAttributeInfo.info = new byte[currentAttributeInfo.attributeLength];

                for (int currentInfoByteIndex = 0; currentInfoByteIndex < currentAttributeInfo.attributeLength; currentInfoByteIndex++)
                {
                    currentAttributeInfo.info[currentInfoByteIndex] = javaClassFileStream.ReadBytesFromStreamAs<byte>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    );
                }

                this.attributes[currentIndex] = currentAttributeInfo;
            }
        }

        // TODO: Handle different types of attributes.
        public void ReadAttributesOfMethod(FileStream javaClassFileStream, MethodInfo methodInfo)
        {
            for (int currentIndex = 0; currentIndex < methodInfo.attributesCount; currentIndex++)
            {
                AttributeInfo currentAttributeInfo = new AttributeInfo();

                currentAttributeInfo.attributeNameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentAttributeInfo.attributeLength = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentAttributeInfo.info = new byte[currentAttributeInfo.attributeLength];

                for (int currentInfoByteIndex = 0; currentInfoByteIndex < currentAttributeInfo.attributeLength; currentInfoByteIndex++)
                {
                    currentAttributeInfo.info[currentInfoByteIndex] = javaClassFileStream.ReadBytesFromStreamAs<byte>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    );
                }

                methodInfo.attributes[currentIndex] = currentAttributeInfo;
            }
        }

        public void ReadMethods(FileStream javaClassFileStream)
        {
            for (int currentMethodIndex = 0; currentMethodIndex < this.methodsCount; currentMethodIndex++)
            {
                MethodInfo currentMethodInfo = new MethodInfo();

                currentMethodInfo.accessFlags = (MethodAccessFlags)javaClassFileStream.ReadBytesFromStreamAs<UInt16>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentMethodInfo.nameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentMethodInfo.descriptorIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentMethodInfo.attributesCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                currentMethodInfo.attributes = new AttributeInfo[currentMethodInfo.attributesCount];

                this.ReadAttributesOfMethod(
                    javaClassFileStream,
                    currentMethodInfo
                );

                this.methods[currentMethodIndex] = currentMethodInfo;
            }
        }

        // TODO: Handle different types of attributes.
        public void ReadAttributesOfField(FileStream javaClassFileStream, FieldInfo fieldInfo)
        {
            for (int currentIndex = 0; currentIndex < fieldInfo.AttributesCount; currentIndex++)
            {
                fieldInfo.attributes[currentIndex] = new AttributeInfo()
                {
                    attributeNameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    ),
                    attributeLength = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    ),
                    info = new byte[fieldInfo.attributes[currentIndex].attributeLength]
                };

                for (int currentInfoByteIndex = 0; currentInfoByteIndex < fieldInfo.attributes[currentIndex].attributeLength; currentInfoByteIndex++)
                {
                    fieldInfo.attributes[currentIndex].info[currentInfoByteIndex] = javaClassFileStream.ReadBytesFromStreamAs<byte>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    );
                }
            }
        }

        public void ReadFields(FileStream javaClassFileStream)
        {
            for (int currentIndex = 0; currentIndex < this.fieldsCount; currentIndex++)
            {
                this.fields[currentIndex] = new FieldInfo()
                {
                    fieldAccessFlags = (FieldAccessFlags)javaClassFileStream.ReadBytesFromStreamAs<UInt16>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    ),
                    nameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    ),
                    descriptorIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    ),
                    AttributesCount = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                        IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                    )
                };

                this.ReadAttributesOfField(
                    javaClassFileStream,
                    this.fields[currentIndex]
                );
            }
        }

        public void ReadInterfaces(FileStream javaClassFileStream)
        {
            for (int currentIndex = 0; currentIndex < this.interfacesCount; currentIndex++)
            {
                this.interfaces[currentIndex] = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );
            }
        }

        public void ReadConstantPool(FileStream javaClassFileStream)
        {
            for (int currentEntryIndex = 0; currentEntryIndex < this.constantPool.Length; currentEntryIndex++)
            {
                ConstantPoolTags currentConstantPoolEntryTag = (ConstantPoolTags)javaClassFileStream.ReadBytesFromStreamAs<byte>(
                    IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                );

                switch (currentConstantPoolEntryTag)
                {
                    case ConstantPoolTags.CONSTANT_Class:
                        ConstantClassInfo constantClassInfo = new ConstantClassInfo();

                        constantClassInfo.nameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantClassInfo;

                        break;
                    case ConstantPoolTags.CONSTANT_Fieldref:
                        ConstantFieldRefInfo constantFieldRefInfo = new ConstantFieldRefInfo();

                        constantFieldRefInfo.classIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantFieldRefInfo.nameAndTypeIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantFieldRefInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_Methodref:
                        ConstantMethodRefInfo constantMethodRefInfo = new ConstantMethodRefInfo();

                        constantMethodRefInfo.classIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantMethodRefInfo.nameAndTypeIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantMethodRefInfo;

                        break;
                    case ConstantPoolTags.CONSTANT_InterfaceMethodref:
                        ConstantInterfaceMethodRefInfo constantInterfaceMethodRefInfo = new ConstantInterfaceMethodRefInfo();

                        constantInterfaceMethodRefInfo.classIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantInterfaceMethodRefInfo.nameAndTypeIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantInterfaceMethodRefInfo;

                        break;
                    case ConstantPoolTags.CONSTANT_String:
                        ConstantStringInfo constantStringInfo = new ConstantStringInfo();

                        constantStringInfo.stringIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantStringInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_Integer:
                        ConstantIntegerInfo constantIntegerInfo = new ConstantIntegerInfo();

                        constantIntegerInfo.bytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantIntegerInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_Float:
                        ConstantFloatInfo constantFloatInfo = new ConstantFloatInfo();

                        constantFloatInfo.bytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantFloatInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_Long:
                        ConstantLongInfo constantLongInfo = new ConstantLongInfo();

                        constantLongInfo.highBytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantLongInfo.lowBytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantLongInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_Double:
                        ConstantDoubleInfo constantDoubleInfo = new ConstantDoubleInfo();

                        constantDoubleInfo.highBytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantDoubleInfo.lowBytes = javaClassFileStream.ReadBytesFromStreamAs<uint>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantDoubleInfo;

                        break;
                    case ConstantPoolTags.CONSTANT_NameAndType:
                        ConstantNameAndTypeInfo constantNameAndTypeInfo = new ConstantNameAndTypeInfo();

                        constantNameAndTypeInfo.nameIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantNameAndTypeInfo.descriptorIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantNameAndTypeInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_UTF8:
                        ConstantUTF8Info constantUTF8Info = new ConstantUTF8Info();

                        constantUTF8Info.Length = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        for (int currentByteIndex = 0; currentByteIndex < constantUTF8Info.Length; currentByteIndex++)
                        {
                            constantUTF8Info.bytes[currentByteIndex] = javaClassFileStream.ReadBytesFromStreamAs<byte>(
                                IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                            );
                        }

                        this.constantPool[currentEntryIndex] = constantUTF8Info;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_MethodHandle:
                        ConstantMethodHandleInfo constantMethodHandleInfo = new ConstantMethodHandleInfo();

                        constantMethodHandleInfo.referenceKind = javaClassFileStream.ReadBytesFromStreamAs<byte>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantMethodHandleInfo.referenceIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantMethodHandleInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_MethodType:
                        ConstantMethodTypeInfo constantMethodTypeInfo = new ConstantMethodTypeInfo();

                        constantMethodTypeInfo.descriptorIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantMethodTypeInfo;
                        
                        break;
                    case ConstantPoolTags.CONSTANT_InvokeDynamic:
                        ConstantInvokeDynamicInfo constantInvokeDynamicInfo = new ConstantInvokeDynamicInfo();

                        constantInvokeDynamicInfo.bootstrapMethodAttrIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        constantInvokeDynamicInfo.nameAndTypeIndex = javaClassFileStream.ReadBytesFromStreamAs<ushort>(
                            IS_JAVA_BINARY_FILE_LITTLE_ENDIAN
                        );

                        this.constantPool[currentEntryIndex] = constantInvokeDynamicInfo;
                        
                        break;
                    default:
                        break;
                }
            }
        }

        public void Save()
        {

        }

        public void ExportAsXMLFile(string exportedXMLFilePath)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(exportedXMLFilePath, DEFAULT_XML_WRITER_SETTINGS))
            {
                this.WriteToXMLWriter(xmlWriter, this);

                xmlWriter.Flush();
            }
        }

        public class ConstantPoolInfo
        {
            public ConstantPoolTags tag = ConstantPoolTags.Unknown;

            public ConstantPoolInfo()
            {

            }
        }

        public class ConstantClassInfo : ConstantPoolInfo
        {
            public UInt16 nameIndex = 0;

            public ConstantClassInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Class;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }
        }

        public class ConstantFieldRefInfo : ConstantPoolInfo
        {
            public UInt16 classIndex = 0;
            
            public UInt16 nameAndTypeIndex = 0;

            public ConstantFieldRefInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Fieldref;
            }

            public string GetClassName(JavaClassFile javaClassFile)
            {
                string className = "";

                try
                {
                    className = (javaClassFile.constantPool[this.classIndex] as ConstantClassInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return className;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetType(JavaClassFile javaClassFile)
            {
                string type = "";

                try
                {
                    type = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetDescriptor(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return type;
            }
        }

        public class ConstantMethodRefInfo : ConstantPoolInfo
        {
            public UInt16 classIndex = 0;

            public UInt16 nameAndTypeIndex = 0;

            public ConstantMethodRefInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Methodref;
            }

            public string GetClassName(JavaClassFile javaClassFile)
            {
                string className = "";

                try
                {
                    className = (javaClassFile.constantPool[this.classIndex] as ConstantClassInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return className;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetType(JavaClassFile javaClassFile)
            {
                string type = "";

                try
                {
                    type = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetDescriptor(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return type;
            }
        }

        public class ConstantInterfaceMethodRefInfo : ConstantPoolInfo
        {
            public UInt16 classIndex = 0;

            public UInt16 nameAndTypeIndex = 0;

            public ConstantInterfaceMethodRefInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_InterfaceMethodref;
            }

            public string GetClassName(JavaClassFile javaClassFile)
            {
                string className = "";

                try
                {
                    className = (javaClassFile.constantPool[this.classIndex] as ConstantClassInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return className;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetName(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetType(JavaClassFile javaClassFile)
            {
                string type = "";

                try
                {
                    type = (javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo).GetDescriptor(
                        javaClassFile
                    );
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return type;
            }
        }

        public class ConstantStringInfo : ConstantPoolInfo
        {
            public UInt16 stringIndex = 0;

            public ConstantStringInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_String;
            }

            public string GetStringValue(JavaClassFile javaClassFile)
            {
                string stringValue = "";

                try
                {
                    stringValue = (javaClassFile.constantPool[this.stringIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return stringValue;
            }
        }

        public class ConstantIntegerInfo : ConstantPoolInfo
        {
            public UInt32 bytes = 0;

            public ConstantIntegerInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Integer;
            }
        }

        public class ConstantFloatInfo : ConstantPoolInfo
        {
            public UInt32 bytes = 0;

            public ConstantFloatInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Float;
            }
        }

        public class ConstantLongInfo : ConstantPoolInfo
        {
            public UInt32 highBytes = 0;
            
            public UInt32 lowBytes = 0;

            public ConstantLongInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Long;
            }
        }

        public class ConstantDoubleInfo : ConstantPoolInfo
        {
            public UInt32 highBytes = 0;

            public UInt32 lowBytes = 0;

            public ConstantDoubleInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_Double;
            }
        }

        public class ConstantNameAndTypeInfo : ConstantPoolInfo
        {
            public UInt16 nameIndex = 0;
            
            public UInt16 descriptorIndex = 0;

            public ConstantNameAndTypeInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_NameAndType;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetDescriptor(JavaClassFile javaClassFile)
            {
                string descriptor = "";

                try
                {
                    descriptor = (javaClassFile.constantPool[this.descriptorIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return descriptor;
            }
        }

        public class ConstantUTF8Info : ConstantPoolInfo
        {   
            public byte[] bytes = new byte[0];

            public UInt16 Length
            {
                get
                {
                    return (UInt16)bytes.Length;
                }
                set
                {
                    bytes = new byte[value];
                }
            }

            public string BytesAsASCIIString
            {
                get
                {
                    return Encoding.ASCII.GetString(this.bytes);
                }
            }

            public ConstantUTF8Info()
            {
                this.tag = ConstantPoolTags.CONSTANT_UTF8;
            }
        }

        public class ConstantMethodHandleInfo : ConstantPoolInfo
        {
            public byte referenceKind = 0;
            
            public UInt16 referenceIndex = 0;

            public ConstantMethodHandleInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_MethodHandle;
            }
        }

        public class ConstantMethodTypeInfo : ConstantPoolInfo
        {
            public UInt16 descriptorIndex = 0;

            public ConstantMethodTypeInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_MethodType;
            }

            public string GetDescriptor(JavaClassFile javaClassFile)
            {
                string descriptor = "";

                try
                {
                    descriptor = (javaClassFile.constantPool[this.descriptorIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return descriptor;
            }
        }

        public class ConstantInvokeDynamicInfo : ConstantPoolInfo
        {
            public UInt16 bootstrapMethodAttrIndex = 0;
            
            public UInt16 nameAndTypeIndex = 0;

            public ConstantInvokeDynamicInfo()
            {
                this.tag = ConstantPoolTags.CONSTANT_InvokeDynamic;
            }

            public string GetName(JavaClassFile javaClassFile)
            {
                ConstantNameAndTypeInfo constantNameAndTypeInfo = javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo;

                return constantNameAndTypeInfo.GetName(javaClassFile);
            }

            public string GetType(JavaClassFile javaClassFile)
            {
                ConstantNameAndTypeInfo constantNameAndTypeInfo = javaClassFile.constantPool[this.nameAndTypeIndex] as ConstantNameAndTypeInfo;

                return constantNameAndTypeInfo.GetDescriptor(javaClassFile);
            }
        }

        public class FieldInfo
        {
            /// <summary>
            /// Original type: UInt16
            /// </summary>
            public FieldAccessFlags fieldAccessFlags = 0;

            public UInt16 nameIndex = 0;

            public UInt16 descriptorIndex = 0;

            /// <summary>
            /// Array size = attributesCount
            /// </summary>
            public AttributeInfo[] attributes = new AttributeInfo[0];

            public UInt16 AttributesCount
            {
                get
                {
                    return (UInt16)this.attributes.Length;
                }
                set
                {
                    this.attributes = new AttributeInfo[value];
                }
            }

            public FieldInfo()
            {

            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetDescriptor(JavaClassFile javaClassFile)
            {
                string descriptor = "";

                try
                {
                    descriptor = (javaClassFile.constantPool[this.descriptorIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return descriptor;
            }
        }

        public class MethodInfo
        {
            /// <summary>
            /// Original type: UInt16
            /// </summary>
            public MethodAccessFlags accessFlags = 0;

            public UInt16 nameIndex = 0;

            public UInt16 descriptorIndex = 0;

            public UInt16 attributesCount = 0;

            public AttributeInfo[] attributes = new AttributeInfo[0];

            public MethodInfo()
            {

            }

            public string GetName(JavaClassFile javaClassFile)
            {
                string name = "";

                try
                {
                    name = (javaClassFile.constantPool[this.nameIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return name;
            }

            public string GetDescriptor(JavaClassFile javaClassFile)
            {
                string descriptor = "";

                try
                {
                    descriptor = (javaClassFile.constantPool[this.descriptorIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return descriptor;
            }
        }

        public class AttributeInfo
        {
            public UInt16 attributeNameIndex = 0;

            public UInt32 attributeLength = 0;

            /// <summary>
            /// Array size = attributeLength
            /// </summary>
            public byte[] info = new byte[0];

            public AttributeInfo()
            {

            }

            public string GetAttributeName(JavaClassFile javaClassFile)
            {
                string attribName = "";

                try
                {
                    attribName = (javaClassFile.constantPool[attributeNameIndex] as ConstantUTF8Info).BytesAsASCIIString;
                }
                catch (Exception e)
                {
                    App.Current.logger.WriteLine(
                        e.ToString()
                    );
                }

                return attribName;
            }

            public void WriteToXMLWriter(XmlWriter xmlWriter, JavaClassFile javaClassFile)
            {

            }
        }

        [Flags]
        public enum AccessFlags
        {
            /// <summary>
            /// Declared as public. May be accessed from outside its package.
            /// </summary>
            ACC_PUBLIC = 0x0001,
            /// <summary>
            /// Declared as final. No subclasses allowed.
            /// </summary>
            ACC_FINAL = 0x0010,
            /// <summary>
            /// Treat superclass methods specially when invoked by the
            /// invokespecial instruction.
            /// </summary>
            ACC_SUPER = 0x0020,
            /// <summary>
            /// Is an interface, not a class.
            /// </summary>
            ACC_INTERFACE = 0x0200,
            /// <summary>
            /// Declared as abstract. Must not be instantiated.
            /// </summary>
            ACC_ABSTRACT = 0x0400,
            /// <summary>
            /// Declared as synthetic. Not present in the source code, generated
            /// by a compiler.
            /// </summary>
            ACC_SYNTHETIC = 0x1000,
            /// <summary>
            /// Declared as an annotation type.
            /// </summary>
            ACC_ANNOTATION = 0x2000,
            /// <summary>
            /// Declared as an enum type.
            /// </summary>
            ACC_ENUM = 0x4000
        }

        public enum ConstantPoolTags
        {
            CONSTANT_Class = 7,
            CONSTANT_Fieldref = 9,
            CONSTANT_Methodref = 10,
            CONSTANT_InterfaceMethodref = 11,
            CONSTANT_String = 8,
            CONSTANT_Integer = 3,
            CONSTANT_Float = 4,
            CONSTANT_Long = 5,
            CONSTANT_Double = 6,
            CONSTANT_NameAndType = 12,
            CONSTANT_UTF8 = 1,
            CONSTANT_MethodHandle = 15,
            CONSTANT_MethodType = 16,
            CONSTANT_InvokeDynamic = 18,
            Unknown = 0
        }

        [Flags]
        public enum FieldAccessFlags
        {
            /// <summary>
            /// Declared public. May be accessed from outside its package.
            /// </summary>
            ACC_PUBLIC = 0x0001,
            /// <summary>
            /// Declared private. Usable only within the defining class.
            /// </summary>
            ACC_PRIVATE = 0x0002,
            /// <summary>
            /// Declared protected. May be accessed within subclasses.
            /// </summary>
            ACC_PROTECTED = 0x0004,
            /// <summary>
            /// Declared static.
            /// </summary>
            ACC_STATIC = 0x0008,
            /// <summary>
            /// Declared final. Never directly assigned to after object
            /// construction.
            /// </summary>
            ACC_FINAL = 0x0010,
            /// <summary>
            /// Declared volatile. Cannot be cached.
            /// </summary>
            ACC_VOLATILE = 0x0040,
            /// <summary>
            /// Declared transient. Not written or read by a persistent object
            /// manager.
            /// </summary>
            ACC_TRANSIENT = 0x0080,
            /// <summary>
            /// Declared synthetic. Not present in the source code.
            /// </summary>
            ACC_SYNTHETIC = 0x1000,
            /// <summary>
            /// Declared as an element of an enum.
            /// </summary>
            ACC_ENUM = 0x4000
        }

        [Flags]
        public enum MethodAccessFlags
        {
            /// <summary>
            /// Declared public; may be accessed from outside its package.
            /// </summary>
            ACC_PUBLIC = 0x0001,
            /// <summary>
            /// Declared private; accessible only within the defining class.
            /// </summary>
            ACC_PRIVATE = 0x0002,
            /// <summary>
            /// Declared protected; may be accessed within subclasses.
            /// </summary>
            ACC_PROTECTED = 0x0004,
            /// <summary>
            /// Declared static.
            /// </summary>
            ACC_STATIC = 0x0008,
            /// <summary>
            /// Declared final; must not be overridden.
            /// </summary>
            ACC_FINAL = 0x0010,
            /// <summary>
            /// Declared synchronized; invocation is wrapped by a monitor use.
            /// </summary>
            ACC_SYNCHRONIZED = 0x0020,
            /// <summary>
            /// A bridge method, generated by the compiler.
            /// </summary>
            ACC_BRIDGE = 0x0040,
            /// <summary>
            /// Declared with variable number of arguments.
            /// </summary>
            ACC_VARARGS = 0x0080,
            /// <summary>
            /// Declared native; implemented in a language other than Java.
            /// </summary>
            ACC_NATIVE = 0x0100,
            /// <summary>
            /// Declared abstract; no implementation is provided.
            /// </summary>
            ACC_ABSTRACT = 0x0400,
            /// <summary>
            /// Declared strictfp; floating-point mode is FP-strict.
            /// </summary>
            ACC_STRICT = 0x0800,
            /// <summary>
            /// Declared synthetic; not present in the source code.
            /// </summary>
            ACC_SYNTHETIC = 0x1000
        }
    }
}
