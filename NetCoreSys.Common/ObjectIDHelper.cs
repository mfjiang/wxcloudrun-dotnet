using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace JMF.CodeLibrary.Common
{
    //Author linfeng
    //Date 2012-07-17

    /// <summary>
    /// 表示ObjectID帮助类，用以生成和处理符合MongoDB的ObjectID
    ///<para>符合MongoDB的ObjectID，它由12个字节组成(4字节时间戳，3字节机器码，2字节进程标识，3字节计数器）</para>
    ///<para>4字节时间戳(int)</para>
    ///<para>3字节机器码(机器名的md5的前三个字节，即32位md5的前6位)</para>
    ///<para>2字节进程标识</para>
    ///<para>3字节计数器</para>
    /// </summary>
    public static class ObjectIDHelper
    {
        /// <summary>
        /// 生成一个24位16进制的符合MongoDB ObjectId的字符串
        /// </summary>
        /// <returns>24位小写的16进制字符串</returns>
        public static string GenericObjectID()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                byte[] objectId = GetObjectIDBytes();

                if (objectId != null && objectId.Length == 12)
                {
                    foreach (byte b in objectId)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取组合好的12字节ObjectID字节数组
        /// </summary>
        /// <returns></returns>
        private static byte[] GetObjectIDBytes()
        {
            byte[] objectId = new byte[12];

            try
            {
                //时间戳（0-3）
                int timestamp = GetTimeStamp();
                //机器码(4-6)
                byte[] machine = GetMachineHash();
                //进程id(7-8)
                int pid = GetProcessId();
                //随机数(9-11)
                int increment = GetIncrement();

                objectId[0] = (byte)(timestamp >> 24);//取前8位比特
                objectId[1] = (byte)(timestamp >> 16);//取前16位比特
                objectId[2] = (byte)(timestamp >> 8);//取前24位比特
                objectId[3] = (byte)timestamp;
                objectId[4] = machine[0];
                objectId[5] = machine[1];
                objectId[6] = machine[2];
                objectId[7] = (byte)(pid >> 8);
                objectId[8] = (byte)pid;
                objectId[9] = (byte)(increment >> 16);
                objectId[10] = (byte)(increment >> 8);
                objectId[11] = (byte)increment;

                return objectId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 生成一个从公元纪年到当前时间的时间戳
        /// </summary>
        /// <returns>从公元纪年到当前时间的时间戳(秒数)</returns>
        private static int GetTimeStamp()
        {
            DateTime now = DateTime.UtcNow;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan timespan = now - epoch;
            return Convert.ToInt32(Math.Floor(timespan.TotalSeconds));
        }

        /// <summary>
        /// 生成一个以机器名称为依据的散列码作为标识码
        /// </summary>
        /// <returns>一个以机器名称为依据的散列码作为标识码</returns>
        private static byte[] GetMachineHash()
        {
            byte[] result = null;
            string machine = Dns.GetHostName();

            using (MD5 md5 = MD5.Create())
            {
                result = md5.ComputeHash(Encoding.UTF8.GetBytes(machine));
            }

            return result;
        }

        /// <summary>
        /// 生成进程ID
        /// </summary>
        /// <returns>进程ID</returns>
        private static int GetProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }

        /// <summary>
        ///  生成随机计数器数（3个字节能存储的数值大小）
        /// </summary>
        /// <returns>随机数（范围在0~256*256*256）</returns>
        private static int GetIncrement()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int increment = rand.Next(0, 256 * 256 * 256);
            return increment;
        }

    }
}
