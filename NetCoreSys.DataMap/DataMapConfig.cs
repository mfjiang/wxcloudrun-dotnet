using System;

namespace NetCoreSys.DataMap
{
    //Author    江名峰
    //Date      2019.04.18

    /// <summary>
    /// 对DataMap类的扩展方法
    /// </summary>
    public static class DataMapConfig
    {
        /// <summary>
        /// 配置数据实体映射
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public static IDataMap Config(this IDataMap cfg, Action<IDataMap> actions)
        {
            actions.Invoke(cfg);
            return cfg;
        }
    }
}
