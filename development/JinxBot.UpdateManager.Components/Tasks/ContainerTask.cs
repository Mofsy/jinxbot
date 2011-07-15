using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal class ContainerTask : Task
    {
        public ContainerTask(XElement node, Task parent)
            : base(node, parent)
        {
            List<Task> childTasks = new List<Task>();
            foreach (XElement child in node.Elements())
            {
                childTasks.Add(TaskFactory.Create(child, this));
            }
            Tasks = childTasks;
        }

        internal ContainerTask(XElement node, Task parent, bool populateChildTasks)
            : base(node, parent)
        {
            if (populateChildTasks)
            {
                List<Task> childTasks = new List<Task>();
                foreach (XElement child in node.Elements())
                {
                    childTasks.Add(TaskFactory.Create(child, this));
                }
                Tasks = childTasks;
            }
        }

        public IEnumerable<Task> Tasks
        {
            get;
            private set;
        }

        public override TaskStatus Execute()
        {
            TaskStatus status = TaskStatus.Success;
            foreach (Task task in Tasks)
            {
                try
                {
                    TaskStatus thisStatus = task.Execute();
                    if (thisStatus == TaskStatus.Failure)
                    {
                        MessagesList.AddRange(task.Messages);
                        return TaskStatus.Failure;
                    }
                    else if (thisStatus == TaskStatus.Warning)
                    {
                        MessagesList.AddRange(task.Messages);
                        status = TaskStatus.Warning;
                    }
                }
                catch (Exception ex)
                {
                    MessagesList.AddRange(task.Messages);
                    MessagesList.Add(ex.ToString());
                    return TaskStatus.Failure;
                }
            }

            return status;
        }
    }
}
