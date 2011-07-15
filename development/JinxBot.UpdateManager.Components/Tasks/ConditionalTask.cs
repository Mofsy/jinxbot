using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal abstract class ConditionalTask
        : ContainerTask
    {
        protected ConditionalTask(XElement element, Task parent)
            : base(element, parent, false)
        {

        }

        public override TaskStatus Execute()
        {
            if (ShouldExecute)
            {
                return base.Execute();
            }

            return TaskStatus.Success;
        }

        public abstract bool ShouldExecute { get; }
    }
}
