﻿using Contracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoggerManager : ILoggerManager
{
    private static ILogger logger = LogManager.GetCurrentClassLogger();

    public LoggerManager()
    {
    }

    public void LogInfo(string message) => logger.Info(message);
    public void LogDebug(string message) => logger.Debug(message);
    public void LogWarn(string message) => logger.Warn(message);
    public void LogError(string message) => logger.Error(message);
}
