using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Java_Bytecode_Toolkit.ExtensionsNS
{
    public static class Extensions
    {
        public static void SetFilter(this FileDialog fileDialog, params FileDialogFilter[] fileDialogFilters)
        {
            string fileDialogFilterString = "";

            for (int currentFilterIndex = 0; currentFilterIndex < fileDialogFilters.Length; currentFilterIndex++)
            {
                if (currentFilterIndex != 0)
                {
                    fileDialogFilterString += "|";
                }

                fileDialogFilterString += fileDialogFilters[currentFilterIndex].filterName + "|";

                foreach (string fileExtension in fileDialogFilters[currentFilterIndex].fileExtensions)
                {
                    fileDialogFilterString += "*." + fileExtension;
                }
            }

            fileDialog.Filter = fileDialogFilterString;
        }

        public static T To<T>(this Enum enumInstance)
        {
            return (T)(object)enumInstance;
        }

        public static byte[] ReadBytes(this Stream stream, int offset, int numBytesToRead, bool isLittleEndian)
        {
            byte[] bytes = new byte[numBytesToRead];

            stream.Read(bytes, 0, numBytesToRead);

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }

            return bytes;
        }

        public static T To<T>(this byte[] byteArray)
        {
            if (typeof(T) == typeof(byte))
            {
                return (T)(object)byteArray[0];
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)BitConverter.ToBoolean(byteArray, 0);
            }
            else if (typeof(T) == typeof(char))
            {
                return (T)(object)BitConverter.ToChar(byteArray, 0);
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)BitConverter.ToDouble(byteArray, 0);
            }
            else if (typeof(T) == typeof(Int16))
            {
                return (T)(object)BitConverter.ToInt16(byteArray, 0);
            }
            else if (typeof(T) == typeof(Int32))
            {
                return (T)(object)BitConverter.ToInt32(byteArray, 0);
            }
            else if (typeof(T) == typeof(Int64))
            {
                return (T)(object)BitConverter.ToInt64(byteArray, 0);
            }
            else if (typeof(T) == typeof(Single))
            {
                return (T)(object)BitConverter.ToSingle(byteArray, 0);
            }
            else if (typeof(T) == typeof(UInt16))
            {
                return (T)(object)BitConverter.ToUInt16(byteArray, 0);
            }
            else if (typeof(T) == typeof(UInt32))
            {
                return (T)(object)BitConverter.ToUInt32(byteArray, 0);
            }
            else if (typeof(T) == typeof(UInt64))
            {
                return (T)(object)BitConverter.ToUInt64(byteArray, 0);
            }

            throw new UnsupportedConversionException();
        }

        public unsafe static T ReadBytesAs<T>(this Stream stream, int offset, bool isLittleEndian)
        {
            return stream.ReadBytes(offset, sizeof(T), isLittleEndian).To<T>();
        }

        public unsafe static T ReadBytesFromStreamAs<T>(this Stream stream, bool isLittleEndian)
        {
            int valueSizeInBytes = sizeof(T);

            T obtainedValue = stream.ReadBytes(
                (int)stream.Position,
                valueSizeInBytes,
                isLittleEndian
            ).To<T>();

            return obtainedValue;
        }

        public static void WriteToXMLWriter(this object obj, XmlWriter xmlWriter, JavaClassFile javaClassFile)
        {
            Type objType = obj.GetType();

            xmlWriter.WriteStartElement(objType.Name);

            foreach (FieldInfo fieldInfo in objType.GetFields())
            {
                string capitalizedFieldName = char.ToUpper(
                    fieldInfo.Name[0]
                ) + fieldInfo.Name.Substring(1);

                Type fieldType = fieldInfo.FieldType;

                if (fieldType.IsArray == true)
                {
                    xmlWriter.WriteStartElement(
                        capitalizedFieldName
                    );

                    Array fieldValueAsArray = fieldInfo.GetValue(obj) as Array;

                    foreach (object element in fieldValueAsArray)
                    {
                        if (element == null)
                        {
                            continue;
                        }

                        Type elementType = element.GetType();

                        if (elementType.IsPrimitive == true)
                        {
                            xmlWriter.WriteElementString(
                                elementType.Name,
                                element.ToString()
                            );

                            continue;
                        }

                        element.WriteToXMLWriter(xmlWriter, javaClassFile);
                    }

                    xmlWriter.WriteEndElement();

                    continue;
                }

                MethodInfo writeToXMLWriterMethodInfo = fieldType.GetMethod(
                    nameof(WriteToXMLWriter)
                );

                if (writeToXMLWriterMethodInfo == null)
                {
                    xmlWriter.WriteElementString(
                        capitalizedFieldName,
                        fieldInfo.GetValue(obj).ToString()
                    );

                    continue;
                }

                writeToXMLWriterMethodInfo.Invoke(
                    fieldInfo.GetValue(obj),
                    new object[]
                    {
                        xmlWriter,
                        javaClassFile
                    }
                );
            }

            xmlWriter.WriteEndElement();
        }

        public class UnsupportedConversionException : Exception
        {
            public UnsupportedConversionException() : base("Conversion from byte array to current data type is not supported.")
            {

            }
        }
    }
}
