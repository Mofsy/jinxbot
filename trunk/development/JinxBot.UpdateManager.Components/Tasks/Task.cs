using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal abstract class Task
    {
        protected XElement Node
        {
            get;
            private set;
        }

        protected Task Parent
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                XAttribute attr = Node.Attribute("Name");
                if (attr != null)
                    return attr.Value;
                return null;
            }
        }


        internal List<string> MessagesList = new List<string>();

        protected Task(XElement element, Task parent)
        {
            Debug.Assert(element != null);

            Node = element;
            Parent = parent;
        }

        public abstract TaskStatus Execute();

        //public abstract TaskStatus RollBack();

        public IEnumerable<string> Messages { get { return MessagesList.AsEnumerated(); } }
    }
}
