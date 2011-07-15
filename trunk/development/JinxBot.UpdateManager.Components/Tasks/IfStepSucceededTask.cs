using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal class IfStepSucceededTask : ConditionalTask
    {
        public IfStepSucceededTask(XElement element, Task parent)
            : base(element, parent)
        {

        }

        public override bool ShouldExecute
        {
            get { return false; }
        }
    }
}
