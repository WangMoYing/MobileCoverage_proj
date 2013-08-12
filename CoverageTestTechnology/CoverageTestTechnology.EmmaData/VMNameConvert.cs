using System;
using System.Text;

namespace CoverageTestTechnology.EmmaData
{
    public class VMNameConvert
    {
        private static bool RENAME_INNER_CLASSES = false;
        public static char JAVA_NAME_SEPARATOR = '.';
        public static char VM_NAME_SEPARATOR = '/';

        public static string Combine(string packageName, string name, char separator)
        {
            if ((name == null) || (name.Length == 0))
                throw new ArgumentException("null or empty input: name");

            if ((packageName == null) || (packageName.Length == 0))
                return name;
            else
                return new StringBuilder(packageName).Append(separator).Append(name).ToString();
        }

        public static string CombineVMName(string packageName, string name)
        {
            return Combine(packageName, name, VM_NAME_SEPARATOR);
        }

        public static String VMNameToJavaName(string vmName)
        {
            if (vmName == null) return null;

            return vmName.Replace('/', '.');
        }

        public static string MethodVMNameToJavaName(string className,
                                                string methodVMName,
                                                string descriptor,
                                                bool renameInits,
                                                bool shortTypeNames,
                                                bool appendReturnType)
        {
            StringBuilder outStr = new StringBuilder();

            if (renameInits)
            {
                if (DataConstants.CLINIT_NAME.Equals(methodVMName))
                    return "<static initializer>";
                else if (DataConstants.INIT_NAME.Equals(methodVMName))
                    outStr.Append(className);
                else
                    outStr.Append(methodVMName);
            }
            else
            {
                if (DataConstants.CLINIT_NAME.Equals(methodVMName))
                    return DataConstants.CLINIT_NAME;
                else
                    outStr.Append(methodVMName);
            }

            char[] chars = descriptor.ToCharArray();
            int end;

            outStr.Append(" (");
            {
                for (end = chars.Length; chars[--end] != ')'; ) ;

                for (int start = 1; start < end; )
                {
                    if (start > 1) outStr.Append(", ");
                    start = TypeDescriptorToJavaName(chars, start, shortTypeNames, outStr);
                }
            }

            if (appendReturnType)
            {
                outStr.Append("): ");

                TypeDescriptorToJavaName(chars, end + 1, shortTypeNames, outStr);
            }
            else
            {
                outStr.Append(')');
            }

            return outStr.ToString();
        }

        private static int TypeDescriptorToJavaName(char[] descriptor, int start,
                                                 bool shortTypeNames,
                                                 StringBuilder outStr)
        {
            int dims;
            for (dims = 0; descriptor[start] == '['; ++dims, ++start) ;

            char c = descriptor[start++];
            switch (c)
            {
                case 'L':
                    {
                        if (shortTypeNames)
                        {
                            int lastSlash = -1;
                            for (int s = start; descriptor[s] != ';'; ++s)
                            {
                                if (descriptor[s] == '/') lastSlash = s;
                            }

                            for (start = lastSlash > 0 ? lastSlash + 1 : start; descriptor[start] != ';'; ++start)
                            {
                                c = descriptor[start];
                                if (RENAME_INNER_CLASSES)
                                    outStr.Append(c != '$' ? c : '.');
                                else
                                    outStr.Append(c);
                            }
                        }
                        else
                        {
                            for (; descriptor[start] != ';'; ++start)
                            {
                                c = descriptor[start];
                                outStr.Append(c != '/' ? c : '.');
                            }
                        }

                        ++start;
                    }
                    break;

                case 'B': outStr.Append("byte"); break;
                case 'C': outStr.Append("char"); break;
                case 'D': outStr.Append("double"); break;
                case 'F': outStr.Append("float"); break;
                case 'I': outStr.Append("int"); break;
                case 'J': outStr.Append("long"); break;
                case 'S': outStr.Append("short"); break;
                case 'Z': outStr.Append("boolean"); break;
                case 'V': outStr.Append("void"); break;

                default:
                    throw new ArgumentException("unknown type descriptor element: " + c);

            } // end of switch

            if (dims > 0)
            {
                outStr.Append(' ');
                for (int d = 0; d < dims; ++d) outStr.Append("[]");
            }

            return start;
        }
    }
}
