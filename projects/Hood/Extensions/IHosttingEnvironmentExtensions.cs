﻿using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hood.Extensions
{
    public static class IHosttingEnvironmentExtensions
    {
        public static void WriteLogToFile<T>(this IHostingEnvironment env, string log) where T : class
        {
            var logPath = env.ContentRootPath + "\\Logs\\" + typeof(T).ToString() + "\\";

            if (!System.IO.Directory.Exists(logPath))
                System.IO.Directory.CreateDirectory(logPath);

            var logFile = System.IO.File.Create(logPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
            var logWriter = new System.IO.StreamWriter(logFile);
            logWriter.Write(log);
            logWriter.Dispose();
        }
    }
}