using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal static class TaskFactory
    {
        private static Dictionary<string, Type> _taskTypes = InitializeTasksTypes();

        private static Dictionary<string, Type> InitializeTasksTypes()
        {
            Dictionary<string, Type> result = new Dictionary<string, Type>();
            Assembly asm = typeof(TaskFactory).Assembly;

            Type taskType = typeof(Task);
            foreach (Type t in asm.GetTypes())
            {
                if (taskType.IsAssignableFrom(t) && !t.IsAbstract)
                {
                    result.Add(t.Name.Replace("Task", ""), t);
                }
            }
            return result;
        }

        public static Task Create(XElement element, Task parent)
        {
            Type result;
            if (!_taskTypes.TryGetValue(element.Name.ToString(), out result))
                throw new ArgumentOutOfRangeException("element", element.Name, "Unsupported manifest task type.");

            return Activator.CreateInstance(result, element, parent) as Task;
        }
    }
}
