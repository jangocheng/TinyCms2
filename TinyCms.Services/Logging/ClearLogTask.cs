﻿using TinyCms.Services.Tasks;

namespace TinyCms.Services.Logging
{
    /// <summary>
    ///     Represents a task to clear [Log] table
    /// </summary>
    public class ClearLogTask : ITask
    {
        private readonly ILogger _logger;

        public ClearLogTask(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Executes a task
        /// </summary>
        public virtual void Execute()
        {
            _logger.ClearLog();
        }
    }
}