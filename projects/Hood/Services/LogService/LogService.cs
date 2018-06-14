﻿using System.Collections.Generic;
using System.Linq;
using Hood.Interfaces;
using Geocoding;
using Geocoding.Google;
using Hood.Extensions;
using System.Threading.Tasks;
using Hood.Models;
using System;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hood.Services
{
    public class LogService : ILogService
    {
        protected readonly HoodDbContext _db;
        protected readonly ISettingsRepository _settings;
        protected readonly IConfiguration _config;

        public LogService(ISettingsRepository site, IConfiguration config)
        {
            _settings = site;
            _config = config;

            var options = new DbContextOptionsBuilder<HoodDbContext>();
            options.UseSqlServer(_config["ConnectionStrings:DefaultConnection"]);
            _db = new HoodDbContext(options.Options);
        }

        public async Task AddLogAsync(string message, string detail, LogType type, LogSource source, string UserId, string entityId, string entityType, string url)
        {
            var log = new Log()
            {
                Type = type,
                Source = source,
                Detail = detail,
                Time = DateTime.Now,
                Title = message,
                UserId = UserId,
                EntityId = entityId,
                EntityType = entityType,
                SourceUrl = url
            };
            _db.Logs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task AddLogAsync(string message, Exception ex, LogType type, LogSource source, string UserId, string entityId, string entityType, string url)
        {
            var detail = string.Concat(
                "Exception  Message: ", ex.Message, Environment.NewLine,
                "Stack Trace:", Environment.NewLine, ex.StackTrace,
                "Exception JSON:", Environment.NewLine, Environment.NewLine,
                JsonConvert.SerializeObject(ex)
            );
            if (ex.InnerException != null)
                detail = string.Concat(
                    "Inner Exception: ", ex.InnerException.Message, Environment.NewLine,
                    "Stack Trace:", Environment.NewLine, ex.StackTrace,
                    "Exception JSON:", Environment.NewLine, Environment.NewLine,
                    JsonConvert.SerializeObject(ex));
            await AddLogAsync(message, detail, LogType.Error, source, UserId, entityId, entityType, url);
        }

    }
}