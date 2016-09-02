using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PaperRecognize.Import
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 返回web.config文件中appSettings配置节的value项 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetConfiguration<T>(Func<string, T> parseFunc, string key)
        {
            try
            {
                T val = default(T);
                string rawConfigValue = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(rawConfigValue))
                {
                    return parseFunc(rawConfigValue);
                }
                return val;
            }
            catch (ConfigurationException ce)
            {
                // error handling logic
                return default(T);
            }
        }
    }
}
