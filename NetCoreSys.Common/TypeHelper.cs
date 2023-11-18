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
        /// IsSimpleType 是否为简单类型：数值、字符、字符串、日期、布尔、枚举、Type
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

        #region IsNumbericType 是否为数值类型

        /// <summary>
        /// 是否为数值类型
        /// </summary>
        /// <param name="destDataType">类型</param>
        /// <returns>
        /// 当类型为 
        /// <see cref="Int32"/>、<see cref="UInt32"/>
        /// <see cref="Int16"/>、<see cref="UInt16"/>、
        /// <see cref="Int64"/>、<see cref="UInt64"/>、
        /// <see cref="Byte"/>、<see cref="SByte"/>、
        /// <see cref="Double"/>、<see cref="Single"/>、<see cref="Decimal"/>
        ///  时返回 true，否则 false。
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
        /// 是否为数值类型。
        /// </summary>
        /// <returns>当参数值为 
        /// <see cref="TypeCode.Int32"/>、<see cref="TypeCode.UInt32"/>
        /// <see cref="TypeCode.Int16"/>、<see cref="TypeCode.UInt16"/>、
        /// <see cref="TypeCode.Int64"/>、<see cref="TypeCode.UInt64"/>、
        /// <see cref="TypeCode.Byte"/>、<see cref="TypeCode.SByte"/>、
        /// <see cref="TypeCode.Double"/>、<see cref="TypeCode.Single"/>、<see cref="TypeCode.Decimal"/>
        /// 返回 true，否则返回 false。
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

        #region IsIntegerCompatibleType 是否为整数兼容类型

        /// <summary>
        /// 是否为整数兼容类型
        /// </summary>
        /// <param name="destDataType"></param>
        /// <returns>
        /// 当类型为 
        /// <see cref="Int32"/>、<see cref="UInt32"/>
        /// <see cref="Int16"/>、<see cref="UInt16"/>、
        /// <see cref="Int64"/>、<see cref="UInt64"/>、
        /// <see cref="Byte"/>、<see cref="SByte"/>、
        ///  时返回 true，否则 false。
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
        /// 否为整数兼容类型
        /// </summary>
        /// <returns>当参数值为 
        /// <see cref="TypeCode.Int32"/>、<see cref="TypeCode.UInt32"/>
        /// <see cref="TypeCode.Int16"/>、<see cref="TypeCode.UInt16"/>、
        /// <see cref="TypeCode.Int64"/>、<see cref="TypeCode.UInt64"/>、
        /// <see cref="TypeCode.Byte"/>、<see cref="TypeCode.SByte"/>
        /// 返回 true，否则返回 false。
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
        /// GetClassSimpleName 获取class的声明名称，如 Person
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
        /// ChangeType 对System.Convert.ChangeType进行了增强，支持(0,1)到bool的转换，字符串->枚举、int->枚举、字符串->Type
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

            //将double赋值给数值型的DataRow的字段是可以的，但是通过反射赋值给object的非double的其它数值类型的属性，却不行        
            return System.Convert.ChangeType(val, targetType);

        }
        #endregion

        #region GetDefaultValue

        /// <summary>
        /// 简单的获取某变量类型的默认值
        /// </summary>
        /// <param name="targetType">变量类型</param>
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
        /// 简单的获取类型的默认值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
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
        /// GetTypeRegularName 获取类型的完全名称，如"ESBasic.Filters.SourceFilter,ESBasic"
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
        /// 获取对象的属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property">公共属性或公共字段的名称，不分大小写</param>
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
        /// 获取类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public static Type GetType<T>()
        {
            return GetType(typeof(T));
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <param name="type">类型</param>
        public static Type GetType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        #endregion

        #region Attribute 相关


        /// <summary>
        /// 是否有指定的 <see cref="Attribute"/> 。
        /// </summary>
        /// <param name="typeToExamine">实例</param>
        /// <param name="attributeType">特性类</param>
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

            //todo 以上方法都无法获取继承类的 Attribute
            throw new NotImplementedException("NET 4.0 未实现 IsAttributePresent 方法。");
        }
#else
        {
            return typeToExamine.GetTypeInfo().GetCustomAttributes(attributeType, true).Any();
        }
#endif
        #endregion
    }

}
