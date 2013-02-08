using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jbc
{
    internal class AsyncSourceTest
    {
        public event EventHandler UserJoined;
        public event EventHandler UserLeft;

        private SynchronizationContext _syncContext;

        public async Task RunAsync()
        {
            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
                throw new InvalidOperationException("You must create a SynchronizationContext for this thread before running.");

            await Task.Delay(2000);
            _syncContext.Post((o) =>
            {
                UserJoined(this, EventArgs.Empty);
            }, null);

            await Task.Delay(3000);
            _syncContext.Post((o) =>
            {
                UserLeft(this, EventArgs.Empty);
            }, null);
        }
    }
}
