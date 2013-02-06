using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbc
{
    internal class AsyncSourceTest
    {
        public event EventHandler UserJoined;
        public event EventHandler UserLeft;

        public async Task RunAsync()
        {
            await Task.Delay(2000);
            UserJoined(this, EventArgs.Empty);

            await Task.Delay(3000);
            UserLeft(this, EventArgs.Empty);
        }
    }
}
