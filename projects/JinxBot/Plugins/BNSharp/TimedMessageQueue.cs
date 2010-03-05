using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Plugins;
using BNSharp;
using System.Timers;

namespace JinxBot.Plugins.BNSharp
{
    internal sealed class TimedMessageQueue : ICommandQueue
    {
        private const long MS_PER_PACKET = 240, MS_PER_BYTE = 23;
        private const long MS_PER_TICK = 10000;

        private PriorityQueue<string> m_queue;
        private Timer m_timer;
        private long m_lastTickCount;

        public TimedMessageQueue()
        {
            m_queue = new PriorityQueue<string>();

            m_timer = new Timer();
            m_timer.AutoReset = false;
            m_timer.Elapsed += new ElapsedEventHandler(m_timer_Elapsed);
        }

        void m_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_queue.Count > 0)
            {
                string message = m_queue.Dequeue();
                m_timer.Interval = GetDelayMsForMessage(message);
                OnMessageReady(message);
                m_timer.Start();
            }
            m_lastTickCount = DateTime.Now.Ticks;
        }

        private long GetMsSinceLastSend()
        {
            long currentTicks = DateTime.Now.Ticks;
            if (currentTicks < m_lastTickCount)
            {
                // handle overflow condition.
                long last = long.MaxValue - m_lastTickCount;
                return (currentTicks - last) / MS_PER_TICK;
            }

            return (currentTicks - m_lastTickCount) / MS_PER_TICK;
        }

        private static long GetDelayMsForMessage(string message)
        {
            return MS_PER_PACKET + (Encoding.UTF8.GetByteCount(message) * MS_PER_BYTE);
        }

        #region ICommandQueue Members

        public void EnqueueMessage(string message, Priority priority)
        {
            if (m_timer.Enabled)
            {
                m_queue.Enqueue(priority, message);
            }
            else
            {
                m_timer.Interval = GetDelayMsForMessage(message);
                OnMessageReady(message);
                m_timer.Start();
                m_lastTickCount = DateTime.Now.Ticks;
            }
        }

        public void Clear()
        {
            m_queue.Clear();
        }

        private void OnMessageReady(string message)
        {
            if (MessageReady != null)
                MessageReady(message);
        }

        public event QueuedMessageReadyCallback MessageReady;

        #endregion
    }
}
