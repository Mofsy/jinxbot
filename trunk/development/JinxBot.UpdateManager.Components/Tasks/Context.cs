using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal static class Context
    {
        public static void Reset()
        {
            _variables = InitializeDefaultVariables();
            _statuses.Clear();
        }

        private static Dictionary<string, string> _variables = InitializeDefaultVariables();
        private static Dictionary<string, TaskStatus> _statuses = new Dictionary<string, TaskStatus>();

        private static Dictionary<string, string> InitializeDefaultVariables()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("AppLocal", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            result.Add("AppRoaming", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            result.Add("ProgramFiles", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            result.Add("BaseInstallPath", Path.Combine(result["ProgramFiles"], "JinxBot"));
            result.Add("Windows", Environment.GetFolderPath(Environment.SpecialFolder.Windows));

            return result;
        }

        public static void SetVariable(string key, string value)
        {
            _variables[key] = value;
        }

        public static string ResolvePath(string testPath)
        {
            Regex rgx = new Regex(@"{(\w+?)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string result = rgx.Replace(testPath, m =>
            {
                string test = m.Groups[1].Value;
                string replacement;
                if (!_variables.TryGetValue(test, out replacement))
                    replacement = string.Empty;

                return replacement;
            });

            return result;
        }

        public static string GetVariable(string key)
        {
            return _variables[key];
        }

        public static void ReportStatus(string taskName, TaskStatus status)
        {
            if (_statuses.ContainsKey(taskName))
                throw new ArgumentOutOfRangeException("taskName", taskName, "Already reported status for this task.");

            _statuses.Add(taskName, status);
        }

        public static TaskStatus? CheckStatus(string taskName)
        {
            TaskStatus? result = null;
            TaskStatus temp;
            if (_statuses.TryGetValue(taskName, out temp))
            {
                result = temp;
            }
            return result;
        }
    }
}
