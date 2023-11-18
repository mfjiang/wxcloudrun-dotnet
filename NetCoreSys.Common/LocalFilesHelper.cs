using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreSys.Common
{
    /// <summary>
    /// 查找当前程序下的指定类型的文件
    /// </summary>
    public static class LocalFilesHelper
    {
        /// <summary>
        /// 找出xml文件
        /// </summary>
        /// <param name="xmlFilePaths"></param>
        /// <returns></returns>
        public static List<string> FindXmlFiles(List<string> xmlFilePaths)
        {
            var filePaths = new List<string>();

            //应用程序文档
            filePaths.Add($"{Assembly.GetEntryAssembly().GetName().Name}.xml");
            if (xmlFilePaths?.Any() == true)
            {
                filePaths.AddRange(xmlFilePaths);
            }

            //目录下文件
            filePaths.AddRange(Directory.GetFiles(AppContext.BaseDirectory, "*.xml"));

            filePaths = filePaths.Distinct().ToList();

            return filePaths;
        }

        /// <summary>
        /// 找出json文件
        /// </summary>
        /// <param name="jsonFilePaths"></param>
        /// <returns></returns>
        public static List<string> FindJsonFiles(List<string> jsonFilePaths)
        {
            var filePaths = new List<string>();

            //应用程序文档
            filePaths.Add($"{Assembly.GetEntryAssembly().GetName().Name}.json");
            if (jsonFilePaths?.Any() == true)
            {
                filePaths.AddRange(jsonFilePaths);
            }

            //目录下文件
            filePaths.AddRange(Directory.GetFiles(AppContext.BaseDirectory, "*.json"));

            filePaths = filePaths.Distinct().ToList();

            return filePaths;
        }

        /// <summary>
        /// 找出dll文件
        /// </summary>
        /// <param name="dllFilePaths"></param>
        /// <returns></returns>
        public static List<string> FindDllFiles(List<string> dllFilePaths)
        {
            var filePaths = new List<string>();

            //应用程序文档
            filePaths.Add($"{Assembly.GetEntryAssembly().GetName().Name}.dll");
            if (dllFilePaths?.Any() == true)
            {
                filePaths.AddRange(dllFilePaths);
            }

            //目录下文件
            filePaths.AddRange(Directory.GetFiles(AppContext.BaseDirectory, "*.dll"));

            filePaths = filePaths.Distinct().ToList();

            return filePaths;
        }
    }
}
