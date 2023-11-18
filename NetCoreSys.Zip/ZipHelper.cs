using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCoreSys.Zip
{
    /// <summary>
    /// ZIP方法类
    /// </summary>
    public class ZipHelper
    {
        ///// <summary>
        ///// 压缩ZIP文件
        ///// 支持多文件和多目录，或是多文件和多目录一起压缩
        ///// </summary>
        ///// <param name="list">待压缩的文件或目录集合</param>
        ///// <param name="strZipName">压缩后的文件名</param>
        ///// <param name="isDirStruct">是否按目录结构压缩</param>
        ///// <returns>成功：true/失败：false</returns>
        //public static bool CompressMulti(List<string> list, string strZipName, bool isDirStruct)
        //{
        //    if (list == null)
        //    {
        //        throw new ArgumentNullException("list");
        //    }
        //    if (string.IsNullOrEmpty(strZipName))
        //    {
        //        throw new ArgumentNullException(strZipName);
        //    }
        //    try
        //    {
        //        //设置编码，解决压缩文件时中文乱码
        //        using (var zip = new ZipFile(Encoding.UTF8))
        //        {
        //            foreach (var path in list)
        //            {
        //                //取目录名称
        //                var fileName = Path.GetFileName(path);
        //                //如果是目录
        //                if (Directory.Exists(path))
        //                {
        //                    //按目录结构压缩
        //                    if (isDirStruct)
        //                    {
        //                        zip.AddDirectory(path, fileName);
        //                    }
        //                    else
        //                    {
        //                        //目录下的文件都压缩到Zip的根目录
        //                        zip.AddDirectory(path);
        //                    }
        //                }
        //                if (File.Exists(path))
        //                {
        //                    zip.AddFile(path);
        //                }
        //            }
        //            //压缩
        //            zip.Save(strZipName);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// 压缩单个文件
        ///// </summary>
        ///// <param name="filePath">原文件路径</param>
        ///// <param name="strZipName">压缩后文件路径</param>
        ///// <returns></returns>
        //public static bool CompressFile(string filePath, string strZipName)
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //    {
        //        throw new ArgumentNullException("filePath");
        //    }
        //    if (string.IsNullOrEmpty(strZipName))
        //    {
        //        throw new ArgumentNullException(strZipName);
        //    }
        //    try
        //    {
        //        //设置编码，解决压缩文件时中文乱码
        //        using (var zip = new ZipFile(Encoding.UTF8))
        //        {
        //            zip.CompressionLevel = CompressionLevel.BestCompression;
        //            //取目录名称
        //            var fileName = Path.GetFileName(filePath);

        //            if (File.Exists(filePath))
        //            {
        //                zip.AddFile(filePath,"");
        //            }
        //            //压缩
        //            zip.Save(strZipName);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}


        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件全名</param>
        /// <param name="zipedFile">压缩后的文件名</param>
        /// <param name="password">密码</param>
        /// <returns>压缩结果</returns>
        public static bool CompressFile(string fileToZip, string zipedFile, string password = "")
        {
            bool result = true;
            ZipOutputStream zipStream = null;
            FileStream fs = null;
            ZipEntry ent = null;

            if (!File.Exists(fileToZip))
                return false;

            try
            {
                fs = File.OpenRead(fileToZip);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                fs = File.Create(zipedFile);
                zipStream = new ZipOutputStream(fs);
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                ent = new ZipEntry(Path.GetFileName(fileToZip));
                ent.IsUnicodeText = true;  //中文文件名乱码
                zipStream.PutNextEntry(ent);
                zipStream.SetLevel(9);

                zipStream.Write(buffer, 0, buffer.Length);

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (ent != null)
                {
                    ent = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);

            return result;
        }

        #region 解压

        /// <summary>
        /// 解压功能(解压压缩文件到指定目录)
        /// </summary>
        /// <param name="fileToUnZip">待解压的文件</param>
        /// <param name="zipedFolder">指定解压目标目录</param>
        /// <param name="password">密码</param>
        /// <returns>解压结果</returns>
        public static bool Decompression(string fileToUnZip, string zipedFolder, string password = "")
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }


        #endregion


        ///// <summary>
        ///// 解压ZIP文件
        ///// </summary>
        ///// <param name="strZipPath">待解压的ZIP文件</param>
        ///// <param name="strUnZipPath">解压的目录</param>
        ///// <param name="overWrite">是否覆盖</param>
        ///// <returns>成功：true/失败：false</returns>
        //public static bool Decompression(string strZipPath, string strUnZipPath, bool overWrite)
        //{
        //    if (string.IsNullOrEmpty(strZipPath))
        //    {
        //        throw new ArgumentNullException(strZipPath);
        //    }
        //    if (string.IsNullOrEmpty(strUnZipPath))
        //    {
        //        throw new ArgumentNullException(strUnZipPath);
        //    }
        //    try
        //    {
        //        var options = new ReadOptions
        //        {
        //            Encoding = Encoding.UTF8
        //        };
        //        //设置编码，解决解压文件时中文乱码
        //        using (var zip = ZipFile.Read(strZipPath, options))
        //        {
        //            foreach (var entry in zip)
        //            {
        //                if (string.IsNullOrEmpty(strUnZipPath))
        //                {
        //                    strUnZipPath = strZipPath.Split('.').First();
        //                }
        //                entry.Extract(strUnZipPath, overWrite
        //                        ? ExtractExistingFileAction.OverwriteSilently
        //                        : ExtractExistingFileAction.DoNotOverwrite);
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        /// <summary>
        /// 压缩算法
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static byte[] Compress(MemoryStream ms)
        {
            byte[] pBytes = ms.ToArray();

            return Compress(pBytes);
        }
        /// <summary>
        /// 对文件流进行压缩
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static MemoryStream CompressZip(string fileName, MemoryStream fs)
        {
            MemoryStream returnSm = new MemoryStream();
            var zipStream = new MemoryStream();
            using (ZipOutputStream stream =new ZipOutputStream(zipStream))
            {
                byte[] buffer = new byte[4 * 1024];
                ZipEntry entry = new ZipEntry(Path.GetFileName(fileName));
                entry.IsUnicodeText = true;  //中文文件名乱码
                stream.PutNextEntry(entry);
                stream.SetLevel(9);
                var pBytes = fs.ToArray();
                stream.Write(pBytes, 0, pBytes.Length);
                //fs.CopyTo(stream);
                stream.Flush(); 
                stream.Finish();
                zipStream.Seek(0,SeekOrigin.Begin);
                zipStream.CopyTo(returnSm);
            }
            returnSm.Seek(0, SeekOrigin.Begin);
            return returnSm;
        }
        /// <summary>
        /// 压缩算法
        /// </summary>
        /// <param name="pBytes"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] pBytes)
        {
            MemoryStream mMemory = new MemoryStream();
            Deflater mDeflater = new Deflater(Deflater.BEST_COMPRESSION);
            using (DeflaterOutputStream mStream = new DeflaterOutputStream(mMemory, mDeflater, 131072))
            {
                mStream.Write(pBytes, 0, pBytes.Length);
            }

            return mMemory.ToArray();
        }
        /// <summary>
        /// 解压缩算法
        /// </summary>
        /// <param name="pBytes"></param>
        /// <returns></returns>
        public static byte[] DeCompress(byte[] pBytes)
        {
            MemoryStream mMemory = new MemoryStream();
            using (InflaterInputStream mStream = new InflaterInputStream(new MemoryStream(pBytes)))
            {
                Int32 mSize;
                byte[] mWriteData = new byte[4096];
                while (true)
                {
                    mSize = mStream.Read(mWriteData, 0, mWriteData.Length);
                    if (mSize > 0)
                        mMemory.Write(mWriteData, 0, mSize);
                    else
                        break;
                }
            }
            return mMemory.ToArray();
        }
    }
}