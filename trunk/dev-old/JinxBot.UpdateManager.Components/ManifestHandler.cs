using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using JinxBot.UpdateManager.Components.Tasks;

namespace JinxBot.UpdateManager.Components
{
    internal class ManifestHandler
    {
        private XElement _root;

        public ManifestHandler(string manifestXml)
            : this(new StringReader(manifestXml))
        {

        }

        public ManifestHandler(Stream manifestStream)
            : this(new StreamReader(manifestStream))
        {

        }

        // actual functional constructor
        public ManifestHandler(TextReader manifestHandle)
        {
            XDocument doc = XDocument.Load(manifestHandle);
            XElement root = doc.Element("JinxBotUpdateManifest");
            if (root == null)
                throw new InvalidDataException("Invalid JinxBot Update manifest.  Expected node: <JinxBotUpdateManifest />");

            _root = root;
        }

        public TaskStatus ExecuteManifest()
        {
            TaskStatus result = TaskStatus.Success;

            IEnumerable<Task> tasks = _root.Elements().Select(e => TaskFactory.Create(e, null));
            foreach (Task task in tasks)
            {
                TaskStatus temp = task.Execute();
                if (temp == TaskStatus.Failure)
                    return TaskStatus.Failure;
                else if (temp == TaskStatus.Warning)
                    result = TaskStatus.Warning;
            }

            return result;
        }
    }
}
