using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.UpdateManager.Components.Tasks
{
    /// <summary>
    /// Represents statuses of task execution.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Indicates that the task succeeded.
        /// </summary>
        Success,
        /// <summary>
        /// Indicates that the task failed but was non-essential, or that some warning was generated.
        /// </summary>
        Warning,
        /// <summary>
        /// Indicates that the task failed and previous tasks should roll back.
        /// </summary>
        Failure,
    }
}
