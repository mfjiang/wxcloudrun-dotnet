using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace NetCoreSys.Common
{
    public static class TypeHelper
    {
        #region IsSimpleType

        /// <summary>
        /// IsSimpleType �Ƿ�Ϊ�����ͣ���ֵ���ַ����ַ��������ڡ�������ö�١�Type
        /// </summary>      
        public static bool IsSimpleType(Type t)
        {
            if (TypeHelper.IsNumbericType(t))
            {
                return true;
            }

            if (t == typeof(char))
            {
                return true;
            }

            if (t == typeof(string))
            {
                return true;
            }


            if (t == typeof(bool))
            {
                return true;
            }


            if (t == typeof(DateTime))
            {
                return true;
            }

            if (t == typeof(Type))
            {
                return true;
            }

            if (t.IsEnum)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region IsNumbericType �Ƿ�Ϊ��ֵ����

        /// <summary>
        /// �Ƿ�Ϊ��ֵ����
        /// </summary>
        /// <param name="destDataType">����</param>
        /// <returns>
        /// ������Ϊ 
        /// <see cref="Int32"/>��<see cref="UInt32"/>
        /// <see cref="Int16"/>��<see cref="UInt16"/>��
        /// <see cref="Int64"/>��<see cref="UInt64"/>��
        /// <see cref="Byte"/>��<see cref="SByte"/>��
        /// <see cref="Double"/>��<see cref="Single"/>��<see cref="Decimal"/>
        ///  ʱ���� true������ false��
        /// </returns>
        public static bool IsNumbericType(Type destDataType)
        {
            return destDataType == typeof(int) || destDataType == typeof(uint) ||
                   destDataType == typeof(short) || destDataType == typeof(ushort) ||
                   destDataType == typeof(long) || destDataType == typeof(ulong) ||
                   destDataType == typeof(byte) || destDataType == typeof(sbyte) ||
                   destDataType == typeof(double) || destDataType == typeof(decimal) || destDataType == typeof(float);

        }

        /// <summary>
        /// �Ƿ�Ϊ��ֵ���͡�
        /// </summary>
        /// <returns>������ֵΪ 
        /// <see cref="TypeCode.Int32"/>��<see cref="TypeCode.UInt32"/>
        /// <see cref="TypeCode.Int16"/>��<see cref="TypeCode.UInt16"/>��
        /// <see cref="TypeCode.Int64"/>��<see cref="TypeCode.UInt64"/>��
        /// <see cref="TypeCode.Byte"/>��<see cref="TypeCode.SByte"/>��
        /// <see cref="TypeCode.Double"/>��<see cref="TypeCode.Single"/>��<see cref="TypeCode.Decimal"/>
        /// ���� true�����򷵻� false��
        /// </returns>
        public static bool IsNumbericType(TypeCode typeCode)
        {
            return typeCode == TypeCode.Int32 || typeCode == TypeCode.UInt32 ||
                   typeCode == TypeCode.Int16 || typeCode == TypeCode.UInt16 ||
                   typeCode == TypeCode.Int64 || typeCode == TypeCode.UInt64 ||
                   typeCode == TypeCode.Byte || typeCode == TypeCode.SByte ||
                   typeCode == TypeCode.Double || typeCode == TypeCode.Decimal || typeCode == TypeCode.Single;
        }

        #endregion

        #region IsIntegerCompatibleType �Ƿ�Ϊ������������

        /// <summary>
        /// �Ƿ�Ϊ������������
        /// </summary>
        /// <param name="destDataType"></param>
        /// <returns>
        /// ������Ϊ 
        /// <see cref="Int32"/>��<see cref="UInt32"/>
        /// <see cref="Int16"/>��<see cref="UInt16"/>��
        /// <see cref="Int64"/>��<see cref="UInt64"/>��
        /// <see cref="Byte"/>��<see cref="SByte"/>��
        ///  ʱ���� true������ false��
        /// </returns>
        public static bool IsIntegerCompatibleType(Type destDataType)
        {
            return destDataType == typeof(int) ||
                   destDataType == typeof(short) ||
                   destDataType == typeof(ushort) ||
                   destDataType == typeof(byte) ||
                   destDataType == typeof(sbyte) ||
                   destDataType == typeof(uint) ||
                   destDataType == typeof(long) ||
                   destDataType == typeof(ulong);
        }

        /// <summary>
        /// ��Ϊ������������
        /// </summary>
        /// <returns>������ֵΪ 
        /// <see cref="TypeCode.Int32"/>��<see cref="TypeCode.UInt32"/>
        /// <see cref="TypeCode.Int16"/>��<see cref="TypeCode.UInt16"/>��
        /// <see cref="TypeCode.Int64"/>��<see cref="TypeCode.UInt64"/>��
        /// <see cref="TypeCode.Byte"/>��<see cref="TypeCode.SByte"/>
        /// ���� true�����򷵻� false��
        /// </returns>
        public static bool IsIntegerCompatibleType(TypeCode typeCode)
        {
            return typeCode == TypeCode.Int32 || typeCode == TypeCode.UInt32 ||
              typeCode == TypeCode.Int16 || typeCode == TypeCode.UInt16 ||
              typeCode == TypeCode.Int64 || typeCode == TypeCode.UInt64 ||
              typeCode == TypeCode.Byte || typeCode == TypeCode.SByte;
        }


        #endregion

        #region GetClassSimpleName

        /// <summary>
        /// GetClassSimpleName ��ȡclass���������ƣ��� Person
        /// </summary>      
        public static string GetClassSimpleName(Type t)
        {
            string[] parts = t.ToString().Split('.');
            return parts[parts.Length - 1].ToString();
        }

        #endregion

        #region IsFixLength
        public static bool IsFixLength(Type destDataType)
        {
            if (TypeHelper.IsNumbericType(destDataType))
            {
                return true;
            }

            if (destDataType == typeof(byte[]))
            {
                return true;
            }

            if ((destDataType == typeof(DateTime)) || (destDataType == typeof(bool)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region ChangeType
        /// <summary>
        /// ChangeType ��System.Convert.ChangeType��������ǿ��֧��(0,1)��bool��ת�����ַ���->ö�١�int->ö�١��ַ���->Type
        /// </summary>       
        public static object ChangeType(Type targetType, object val)
        {
            #region null
            if (val == null)
            {
                return null;
            }
            #endregion

            #region Same Type
            if (targetType == val.GetType())
            {
                return val;
            }
            #endregion

            #region bool 1,0
            if (targetType == typeof(bool))
            {
                if (val.ToString() == "0")
                {
                    return false;
                }

                if (val.ToString() == "1")
                {
                    return true;
                }
            }
            #endregion

            #region Enum
            if (targetType.IsEnum)
            {
                int intVal = 0;
                bool suc = int.TryParse(val.ToString(), out intVal);
                if (!suc)
                {
                    return Enum.Parse(targetType, val.ToString());
                }
                else
                {
                    return val;
                }
            }
            #endregion

            //��double��ֵ����ֵ�͵�DataRow���ֶ��ǿ��Եģ�����ͨ�����丳ֵ��object�ķ�double��������ֵ���͵����ԣ�ȴ����        
            return System.Convert.ChangeType(val, targetType);

        }
        #endregion

        #region GetDefaultValue

        /// <summary>
        /// �򵥵Ļ�ȡĳ�������͵�Ĭ��ֵ
        /// </summary>
        /// <param name="targetType">��������</param>
        /// <returns></returns>
        public static object GetDefaultValue(Type targetType)
        {
            if (TypeHelper.IsNumbericType(targetType))
            {
                return 0;
            }

            if (targetType == typeof(string))
            {
                return "";
            }

            if (targetType == typeof(bool))
            {
                return false;
            }

            if (targetType == typeof(DateTime))
            {
                return DateTime.Now;
            }

            if (targetType == typeof(Guid))
            {
                //return System.Guid.NewGuid();
                return new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            if (targetType == typeof(TimeSpan))
            {
                return System.TimeSpan.Zero;
            }

            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// �򵥵Ļ�ȡ���͵�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        public static T GetDefaultValue<T>()
        {
            return (T)GetDefaultValue(typeof(T));
        }

        #endregion

        #region GetDefaultValueString
        public static string GetDefaultValueString(Type destType)
        {
            if (TypeHelper.IsNumbericType(destType))
            {
                return "0";
            }

            if (destType == typeof(string))
            {
                return "\"\"";
            }

            if (destType == typeof(bool))
            {
                return "false";
            }

            if (destType == typeof(DateTime))
            {
                return "DateTime.Now";
            }

            if (destType == typeof(Guid))
            {
                return "System.Guid.NewGuid()";
            }

            if (destType == typeof(TimeSpan))
            {
                return "System.TimeSpan.Zero";
            }


            return "null";
        }
        #endregion

        #region GetTypeRegularName
        /// <summary>
        /// GetTypeRegularName ��ȡ���͵���ȫ���ƣ���"ESBasic.Filters.SourceFilter,ESBasic"
        /// </summary>      
        public static string GetTypeRegularName(Type destType)
        {
            string assName = destType.Assembly.FullName.Split(',')[0];

            return string.Format("{0},{1}", destType.ToString(), assName);

        }

        public static string GetTypeRegularNameOf(object obj)
        {
            Type destType = obj.GetType();
            return TypeHelper.GetTypeRegularName(destType);
        }
        #endregion

        #region GetObjectProperty

        /// <summary>
        /// ��ȡ���������ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property">�������Ի򹫹��ֶε����ƣ����ִ�Сд</param>
        /// <returns></returns>
        public static object GetObjectProperty(object obj, string property)
        {
            return obj.GetType().InvokeMember(property,
                                              BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.GetField,
                                              null, obj, null);
        }

        #endregion

        #region GetType

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        public static Type GetType<T>()
        {
            return GetType(typeof(T));
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="type">����</param>
        public static Type GetType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        #endregion

        #region Attribute ���


        /// <summary>
        /// �Ƿ���ָ���� <see cref="Attribute"/> ��
        /// </summary>
        /// <param name="typeToExamine">ʵ��</param>
        /// <param name="attributeType">������</param>
        /// <returns></returns>
        public static bool IsAttributePresent(Type typeToExamine, Type attributeType)
#if NET40
        {
            //foreach (var item in typeToExamine.GetType().GetCustomAttributes(true))
            //{
            //    if (item.GetType() == attributeType)
            //    {
            //        return true;
            //    }
            //}
            //return false;

            //return typeToExamine.GetType().GetCustomAttributes(attributeType, true).Any();

            //todo ���Ϸ������޷���ȡ�̳���� Attribute
            throw new NotImplementedException("NET 4.0 δʵ�� IsAttributePresent ������");
        }
#else
        {
            return typeToExamine.GetTypeInfo().GetCustomAttributes(attributeType, true).Any();
        }
#endif
        #endregion
    }

}
