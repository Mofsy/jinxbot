using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;

namespace JinxBot.UpdateManager.Components.Tasks
{
    internal class SetVariableTask
        : Task
    {
        public SetVariableTask(XElement element, Task parent)
            : base(element, parent)
        {
            VariableName = element.Attribute("VariableName").Value;
            OriginalValue = element.Attribute("Value").Value;
        }

        public string VariableName
        {
            get;
            private set;
        }

        public string OriginalValue
        {
            get;
            private set;
        }

        public string GetTransformedValue()
        {
            return Context.ResolvePath(OriginalValue);
        }

        public override TaskStatus Execute()
        {
            string val = GetTransformedValue();
            Context.SetVariable(VariableName, GetTransformedValue());

            Trace.WriteLine(string.Format("Setting context variable '{0}' to value '{1}'", VariableName, val));

            return TaskStatus.Success;
        }
    }
}
