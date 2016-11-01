using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace PaperRecognize.Log
{
    /// <summary>
    /// 获取日志的工具类
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 获取日志访问句柄
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static log4net.ILog GetLogger( [CallerFilePath] string filename = "" )
        {
            return log4net.LogManager.GetLogger( filename );
        }
    }
}