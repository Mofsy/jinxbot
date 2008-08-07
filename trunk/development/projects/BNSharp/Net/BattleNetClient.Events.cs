using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BNSharp.BattleNet;
using BNSharp.BattleNet.Clans;
using BNSharp.BattleNet.Friends;

namespace BNSharp.Net
{
    partial class BattleNetClient
    {
        partial void ReportException(Exception ex, params KeyValuePair<string, object>[] notes);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        partial void FreeArgumentResources(BaseEventArgs e);

        #region Invokehelper
        private delegate void Invokee<T>(Priority p, T args) where T : EventArgs;
        private class InvokeHelperBase
        {
            public virtual void Invoke(Priority p) { }
        }
        private class InvokeHelper<T> : InvokeHelperBase where T : EventArgs
        { 
            public Invokee<T> Target;
            public T Arguments;

            public override void Invoke(Priority p)
            {
                Target(p, Arguments);
            }
        }
        #endregion
        /* 
         * This part of the class contains the implementation and registration parts of all of the events.  It is implemented
         * rapidly by using code snippets and then using the #region directive to hide the code.
         */

        private Queue<InvokeHelperBase> e_medPriorityEvents = new Queue<InvokeHelperBase>();
        private Queue<InvokeHelperBase> e_lowPriorityEvents = new Queue<InvokeHelperBase>();

        #region Threads
        private Thread e_medPriorityTd;
        private Thread e_lowPriorityTd;
        private EventWaitHandle e_medBlocker = new EventWaitHandle(false, EventResetMode.AutoReset);
        private EventWaitHandle e_lowBlocker = new EventWaitHandle(false, EventResetMode.AutoReset);

        private void CreateEventThreads()
        {
            e_medPriorityTd = new Thread(new ThreadStart(__MedPriorityEventWatcher));
            e_medPriorityTd.Priority = ThreadPriority.BelowNormal;
            e_medPriorityTd.IsBackground = true;
            e_medPriorityTd.Start();

            e_lowPriorityTd = new Thread(new ThreadStart(__LowPriorityEventWatcher));
            e_lowPriorityTd.Priority = ThreadPriority.Lowest;
            e_lowPriorityTd.IsBackground = true;
            e_lowPriorityTd.Start();
        }

        private void __MedPriorityEventWatcher()
        {
            try
            {
                while (true)
                {
                    e_medBlocker.Reset();

                    while (e_medPriorityEvents.Count == 0)
                    {
                        e_lowBlocker.Set();
                        e_medBlocker.WaitOne();
                    }

                    if (e_medPriorityEvents.Count > 0)
                    {
                        InvokeHelperBase helper = e_medPriorityEvents.Dequeue();
                        if (helper != null) 
                            helper.Invoke(Priority.Normal);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // exit gracefully
            }
        }

        private void __LowPriorityEventWatcher()
        {
            try
            {
                while (true)
                {
                    e_lowBlocker.Reset();

                    while (e_lowPriorityEvents.Count == 0 || e_medPriorityEvents.Count > 0)
                        e_lowBlocker.WaitOne();

                    if (e_lowPriorityEvents.Count > 0)
                    {
                        InvokeHelperBase helper = e_lowPriorityEvents.Dequeue();
                        if (helper != null)
                            helper.Invoke(Priority.Low);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // exit gracefully
            }
        }

        private void CloseEventThreads()
        {
            if (e_medPriorityTd != null)
            {
                e_medPriorityTd.Abort();
                e_medPriorityTd = null;
            }

            if (e_lowPriorityTd != null)
            {
                e_lowPriorityTd.Abort();
                e_lowPriorityTd = null;
            }

            if (e_medBlocker != null)
            {
                e_medBlocker.Close();
                e_medBlocker = null;
            }

            if (e_lowBlocker != null)
            {
                e_lowBlocker.Close();
                e_lowBlocker = null;
            }
        }
        #endregion

        #region chat events (0x0f)
        #region user events
        #region UserJoined event
        [NonSerialized]
        private Dictionary<Priority, List<UserEventHandler>> __UserJoined = new Dictionary<Priority, List<UserEventHandler>>(3)
        {
            { Priority.High, new List<UserEventHandler>() },
            { Priority.Normal, new List<UserEventHandler>() },
            { Priority.Low, new List<UserEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user joined the client's current channel.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserJoinedNotification</see> and
        /// <see>UnregisterUserJoinedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event UserEventHandler UserJoined
        {
            add
            {
                lock (__UserJoined)
                {
                    if (!__UserJoined.ContainsKey(Priority.Normal))
                    {
                        __UserJoined.Add(Priority.Normal, new List<UserEventHandler>());
                    }
                }
                __UserJoined[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserJoined.ContainsKey(Priority.Normal))
                {
                    __UserJoined[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserJoined</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserJoined" />
        /// <seealso cref="UnregisterUserJoinedNotification" />
        public void RegisterUserJoinedNotification(Priority p, UserEventHandler callback)
        {
            lock (__UserJoined)
            {
                if (!__UserJoined.ContainsKey(p))
                {
                    __UserJoined.Add(p, new List<UserEventHandler>());
                }
            }
            __UserJoined[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserJoined</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserJoined" />
        /// <seealso cref="RegisterUserJoinedNotification" />
        public void UnregisterUserJoinedNotification(Priority p, UserEventHandler callback)
        {
            if (__UserJoined.ContainsKey(p))
            {
                __UserJoined[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserJoined event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserJoined</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserJoined" />
        protected virtual void OnUserJoined(UserEventArgs e)
        {
            __InvokeUserJoined(Priority.High, e);
        }

        private void __InvokeUserJoined(Priority p, UserEventArgs e)
        {
            foreach (UserEventHandler eh in __UserJoined[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserJoined"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserJoined) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserJoined) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region UserLeft event
        [NonSerialized]
        private Dictionary<Priority, List<UserEventHandler>> __UserLeft = new Dictionary<Priority, List<UserEventHandler>>(3)
        {
            { Priority.High, new List<UserEventHandler>() },
            { Priority.Normal, new List<UserEventHandler>() },
            { Priority.Low, new List<UserEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user left the client's current channel.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserLeftNotification</see> and
        /// <see>UnregisterUserLeftNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event UserEventHandler UserLeft
        {
            add
            {
                lock (__UserLeft)
                {
                    if (!__UserLeft.ContainsKey(Priority.Normal))
                    {
                        __UserLeft.Add(Priority.Normal, new List<UserEventHandler>());
                    }
                }
                __UserLeft[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserLeft.ContainsKey(Priority.Normal))
                {
                    __UserLeft[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserLeft</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserLeft" />
        /// <seealso cref="UnregisterUserLeftNotification" />
        public void RegisterUserLeftNotification(Priority p, UserEventHandler callback)
        {
            lock (__UserLeft)
            {
                if (!__UserLeft.ContainsKey(p))
                {
                    __UserLeft.Add(p, new List<UserEventHandler>());
                }
            }
            __UserLeft[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserLeft</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserLeft" />
        /// <seealso cref="RegisterUserLeftNotification" />
        public void UnregisterUserLeftNotification(Priority p, UserEventHandler callback)
        {
            if (__UserLeft.ContainsKey(p))
            {
                __UserLeft[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserLeft event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserLeft</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserLeft" />
        protected virtual void OnUserLeft(UserEventArgs e)
        {
            __InvokeUserLeft(Priority.High, e);
        }

        private void __InvokeUserLeft(Priority p, UserEventArgs e)
        {
            foreach (UserEventHandler eh in __UserLeft[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserLeft"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserLeft) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserLeft) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region UserShown event
        [NonSerialized]
        private Dictionary<Priority, List<UserEventHandler>> __UserShown = new Dictionary<Priority, List<UserEventHandler>>(3)
        {
            { Priority.High, new List<UserEventHandler>() },
            { Priority.Normal, new List<UserEventHandler>() },
            { Priority.Low, new List<UserEventHandler>() }
        };
        /// <summary>
        /// Informs listeners a user was already in the channel when the client joined it.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserShownNotification</see> and
        /// <see>UnregisterUserShownNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event UserEventHandler UserShown
        {
            add
            {
                lock (__UserShown)
                {
                    if (!__UserShown.ContainsKey(Priority.Normal))
                    {
                        __UserShown.Add(Priority.Normal, new List<UserEventHandler>());
                    }
                }
                __UserShown[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserShown.ContainsKey(Priority.Normal))
                {
                    __UserShown[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserShown</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserShown" />
        /// <seealso cref="UnregisterUserShownNotification" />
        public void RegisterUserShownNotification(Priority p, UserEventHandler callback)
        {
            lock (__UserShown)
            {
                if (!__UserShown.ContainsKey(p))
                {
                    __UserShown.Add(p, new List<UserEventHandler>());
                }
            }
            __UserShown[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserShown</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserShown" />
        /// <seealso cref="RegisterUserShownNotification" />
        public void UnregisterUserShownNotification(Priority p, UserEventHandler callback)
        {
            if (__UserShown.ContainsKey(p))
            {
                __UserShown[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserShown event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserShown</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserShown" />
        protected virtual void OnUserShown(UserEventArgs e)
        {
            __InvokeUserShown(Priority.High, e);
        }

        private void __InvokeUserShown(Priority p, UserEventArgs e)
        {
            foreach (UserEventHandler eh in __UserShown[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserShown"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserShown) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserShown) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region UserFlagsChanged event
        [NonSerialized]
        private Dictionary<Priority, List<UserEventHandler>> __UserFlagsChanged = new Dictionary<Priority, List<UserEventHandler>>(3)
        {
            { Priority.High, new List<UserEventHandler>() },
            { Priority.Normal, new List<UserEventHandler>() },
            { Priority.Low, new List<UserEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user's flags changed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserFlagsChangedNotification</see> and
        /// <see>UnregisterUserFlagsChangedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event UserEventHandler UserFlagsChanged
        {
            add
            {
                lock (__UserFlagsChanged)
                {
                    if (!__UserFlagsChanged.ContainsKey(Priority.Normal))
                    {
                        __UserFlagsChanged.Add(Priority.Normal, new List<UserEventHandler>());
                    }
                }
                __UserFlagsChanged[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserFlagsChanged.ContainsKey(Priority.Normal))
                {
                    __UserFlagsChanged[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserFlagsChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserFlagsChanged" />
        /// <seealso cref="UnregisterUserFlagsChangedNotification" />
        public void RegisterUserFlagsChangedNotification(Priority p, UserEventHandler callback)
        {
            lock (__UserFlagsChanged)
            {
                if (!__UserFlagsChanged.ContainsKey(p))
                {
                    __UserFlagsChanged.Add(p, new List<UserEventHandler>());
                }
            }
            __UserFlagsChanged[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserFlagsChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserFlagsChanged" />
        /// <seealso cref="RegisterUserFlagsChangedNotification" />
        public void UnregisterUserFlagsChangedNotification(Priority p, UserEventHandler callback)
        {
            if (__UserFlagsChanged.ContainsKey(p))
            {
                __UserFlagsChanged[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserFlagsChanged event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserFlagsChanged</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserFlagsChanged" />
        protected virtual void OnUserFlagsChanged(UserEventArgs e)
        {
            __InvokeUserFlagsChanged(Priority.High, e);
        }

        private void __InvokeUserFlagsChanged(Priority p, UserEventArgs e)
        {
            foreach (UserEventHandler eh in __UserFlagsChanged[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserFlagsChanged"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserFlagsChanged) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<UserEventArgs> { Arguments = e, Target = new Invokee<UserEventArgs>(__InvokeUserFlagsChanged) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
        #endregion

        #region server events
        #region ServerBroadcast event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __ServerBroadcast = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the server sent a broadcast message.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterServerBroadcastNotification</see> and
        /// <see>UnregisterServerBroadcastNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler ServerBroadcast
        {
            add
            {
                lock (__ServerBroadcast)
                {
                    if (!__ServerBroadcast.ContainsKey(Priority.Normal))
                    {
                        __ServerBroadcast.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __ServerBroadcast[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ServerBroadcast.ContainsKey(Priority.Normal))
                {
                    __ServerBroadcast[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ServerBroadcast</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ServerBroadcast" />
        /// <seealso cref="UnregisterServerBroadcastNotification" />
        public void RegisterServerBroadcastNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__ServerBroadcast)
            {
                if (!__ServerBroadcast.ContainsKey(p))
                {
                    __ServerBroadcast.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __ServerBroadcast[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ServerBroadcast</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ServerBroadcast" />
        /// <seealso cref="RegisterServerBroadcastNotification" />
        public void UnregisterServerBroadcastNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__ServerBroadcast.ContainsKey(p))
            {
                __ServerBroadcast[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ServerBroadcast event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ServerBroadcast</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ServerBroadcast" />
        protected virtual void OnServerBroadcast(ServerChatEventArgs e)
        {
            __InvokeServerBroadcast(Priority.High, e);
        }

        private void __InvokeServerBroadcast(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __ServerBroadcast[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ServerBroadcast"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeServerBroadcast) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeServerBroadcast) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region JoinedChannel event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __JoinedChannel = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client joined a new channel.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterJoinedChannelNotification</see> and
        /// <see>UnregisterJoinedChannelNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler JoinedChannel
        {
            add
            {
                lock (__JoinedChannel)
                {
                    if (!__JoinedChannel.ContainsKey(Priority.Normal))
                    {
                        __JoinedChannel.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __JoinedChannel[Priority.Normal].Add(value);
            }
            remove
            {
                if (__JoinedChannel.ContainsKey(Priority.Normal))
                {
                    __JoinedChannel[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>JoinedChannel</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="JoinedChannel" />
        /// <seealso cref="UnregisterJoinedChannelNotification" />
        public void RegisterJoinedChannelNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__JoinedChannel)
            {
                if (!__JoinedChannel.ContainsKey(p))
                {
                    __JoinedChannel.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __JoinedChannel[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>JoinedChannel</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="JoinedChannel" />
        /// <seealso cref="RegisterJoinedChannelNotification" />
        public void UnregisterJoinedChannelNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__JoinedChannel.ContainsKey(p))
            {
                __JoinedChannel[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the JoinedChannel event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>JoinedChannel</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="JoinedChannel" />
        protected virtual void OnJoinedChannel(ServerChatEventArgs e)
        {
            __InvokeJoinedChannel(Priority.High, e);
        }

        private void __InvokeJoinedChannel(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __JoinedChannel[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "JoinedChannel"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeJoinedChannel) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeJoinedChannel) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ChannelWasFull event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __ChannelWasFull = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a channel join failed because the channel was full.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterChannelWasFullNotification</see> and
        /// <see>UnregisterChannelWasFullNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler ChannelWasFull
        {
            add
            {
                lock (__ChannelWasFull)
                {
                    if (!__ChannelWasFull.ContainsKey(Priority.Normal))
                    {
                        __ChannelWasFull.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __ChannelWasFull[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ChannelWasFull.ContainsKey(Priority.Normal))
                {
                    __ChannelWasFull[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ChannelWasFull</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ChannelWasFull" />
        /// <seealso cref="UnregisterChannelWasFullNotification" />
        public void RegisterChannelWasFullNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__ChannelWasFull)
            {
                if (!__ChannelWasFull.ContainsKey(p))
                {
                    __ChannelWasFull.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __ChannelWasFull[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ChannelWasFull</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ChannelWasFull" />
        /// <seealso cref="RegisterChannelWasFullNotification" />
        public void UnregisterChannelWasFullNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__ChannelWasFull.ContainsKey(p))
            {
                __ChannelWasFull[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ChannelWasFull event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ChannelWasFull</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ChannelWasFull" />
        protected virtual void OnChannelWasFull(ServerChatEventArgs e)
        {
            __InvokeChannelWasFull(Priority.High, e);
        }

        private void __InvokeChannelWasFull(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __ChannelWasFull[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ChannelWasFull"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelWasFull) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelWasFull) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ChannelDidNotExist event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __ChannelDidNotExist = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners a channel view failed because the channel did not exist.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterChannelDidNotExistNotification</see> and
        /// <see>UnregisterChannelDidNotExistNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler ChannelDidNotExist
        {
            add
            {
                lock (__ChannelDidNotExist)
                {
                    if (!__ChannelDidNotExist.ContainsKey(Priority.Normal))
                    {
                        __ChannelDidNotExist.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __ChannelDidNotExist[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ChannelDidNotExist.ContainsKey(Priority.Normal))
                {
                    __ChannelDidNotExist[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ChannelDidNotExist</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ChannelDidNotExist" />
        /// <seealso cref="UnregisterChannelDidNotExistNotification" />
        public void RegisterChannelDidNotExistNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__ChannelDidNotExist)
            {
                if (!__ChannelDidNotExist.ContainsKey(p))
                {
                    __ChannelDidNotExist.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __ChannelDidNotExist[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ChannelDidNotExist</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ChannelDidNotExist" />
        /// <seealso cref="RegisterChannelDidNotExistNotification" />
        public void UnregisterChannelDidNotExistNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__ChannelDidNotExist.ContainsKey(p))
            {
                __ChannelDidNotExist[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ChannelDidNotExist event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ChannelDidNotExist</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ChannelDidNotExist" />
        protected virtual void OnChannelDidNotExist(ServerChatEventArgs e)
        {
            __InvokeChannelDidNotExist(Priority.High, e);
        }

        private void __InvokeChannelDidNotExist(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __ChannelDidNotExist[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ChannelDidNotExist"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelDidNotExist) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelDidNotExist) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ChannelWasRestricted event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __ChannelWasRestricted = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a channel join failed because the channel was restricted.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterChannelWasRestrictedNotification</see> and
        /// <see>UnregisterChannelWasRestrictedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler ChannelWasRestricted
        {
            add
            {
                lock (__ChannelWasRestricted)
                {
                    if (!__ChannelWasRestricted.ContainsKey(Priority.Normal))
                    {
                        __ChannelWasRestricted.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __ChannelWasRestricted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ChannelWasRestricted.ContainsKey(Priority.Normal))
                {
                    __ChannelWasRestricted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ChannelWasRestricted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ChannelWasRestricted" />
        /// <seealso cref="UnregisterChannelWasRestrictedNotification" />
        public void RegisterChannelWasRestrictedNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__ChannelWasRestricted)
            {
                if (!__ChannelWasRestricted.ContainsKey(p))
                {
                    __ChannelWasRestricted.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __ChannelWasRestricted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ChannelWasRestricted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ChannelWasRestricted" />
        /// <seealso cref="RegisterChannelWasRestrictedNotification" />
        public void UnregisterChannelWasRestrictedNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__ChannelWasRestricted.ContainsKey(p))
            {
                __ChannelWasRestricted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ChannelWasRestricted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ChannelWasRestricted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ChannelWasRestricted" />
        protected virtual void OnChannelWasRestricted(ServerChatEventArgs e)
        {
            __InvokeChannelWasRestricted(Priority.High, e);
        }

        private void __InvokeChannelWasRestricted(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __ChannelWasRestricted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ChannelWasRestricted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelWasRestricted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeChannelWasRestricted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region InformationReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __InformationReceived = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the server has sent an informational message to the client.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterInformationReceivedNotification</see> and
        /// <see>UnregisterInformationReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler InformationReceived
        {
            add
            {
                lock (__InformationReceived)
                {
                    if (!__InformationReceived.ContainsKey(Priority.Normal))
                    {
                        __InformationReceived.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __InformationReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__InformationReceived.ContainsKey(Priority.Normal))
                {
                    __InformationReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>InformationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="InformationReceived" />
        /// <seealso cref="UnregisterInformationReceivedNotification" />
        public void RegisterInformationReceivedNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__InformationReceived)
            {
                if (!__InformationReceived.ContainsKey(p))
                {
                    __InformationReceived.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __InformationReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>InformationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="InformationReceived" />
        /// <seealso cref="RegisterInformationReceivedNotification" />
        public void UnregisterInformationReceivedNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__InformationReceived.ContainsKey(p))
            {
                __InformationReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the InformationReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>InformationReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="InformationReceived" />
        protected virtual void OnInformationReceived(ServerChatEventArgs e)
        {
            __InvokeInformationReceived(Priority.High, e);
        }

        private void __InvokeInformationReceived(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __InformationReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "InformationReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeInformationReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeInformationReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ServerErrorReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ServerChatEventHandler>> __ServerErrorReceived = new Dictionary<Priority, List<ServerChatEventHandler>>(3)
        {
            { Priority.High, new List<ServerChatEventHandler>() },
            { Priority.Normal, new List<ServerChatEventHandler>() },
            { Priority.Low, new List<ServerChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that an error message was received from the server.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterServerErrorReceivedNotification</see> and
        /// <see>UnregisterServerErrorReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerChatEventHandler ServerErrorReceived
        {
            add
            {
                lock (__ServerErrorReceived)
                {
                    if (!__ServerErrorReceived.ContainsKey(Priority.Normal))
                    {
                        __ServerErrorReceived.Add(Priority.Normal, new List<ServerChatEventHandler>());
                    }
                }
                __ServerErrorReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ServerErrorReceived.ContainsKey(Priority.Normal))
                {
                    __ServerErrorReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ServerErrorReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ServerErrorReceived" />
        /// <seealso cref="UnregisterServerErrorReceivedNotification" />
        public void RegisterServerErrorReceivedNotification(Priority p, ServerChatEventHandler callback)
        {
            lock (__ServerErrorReceived)
            {
                if (!__ServerErrorReceived.ContainsKey(p))
                {
                    __ServerErrorReceived.Add(p, new List<ServerChatEventHandler>());
                }
            }
            __ServerErrorReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ServerErrorReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ServerErrorReceived" />
        /// <seealso cref="RegisterServerErrorReceivedNotification" />
        public void UnregisterServerErrorReceivedNotification(Priority p, ServerChatEventHandler callback)
        {
            if (__ServerErrorReceived.ContainsKey(p))
            {
                __ServerErrorReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ServerErrorReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ServerErrorReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ServerErrorReceived" />
        protected virtual void OnServerErrorReceived(ServerChatEventArgs e)
        {
            __InvokeServerErrorReceived(Priority.High, e);
        }

        private void __InvokeServerErrorReceived(Priority p, ServerChatEventArgs e)
        {
            foreach (ServerChatEventHandler eh in __ServerErrorReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ServerErrorReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeServerErrorReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerChatEventArgs> { Arguments = e, Target = new Invokee<ServerChatEventArgs>(__InvokeServerErrorReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region chat events
        #region WhisperSent event
        [NonSerialized]
        private Dictionary<Priority, List<ChatMessageEventHandler>> __WhisperSent = new Dictionary<Priority, List<ChatMessageEventHandler>>(3)
        {
            { Priority.High, new List<ChatMessageEventHandler>() },
            { Priority.Normal, new List<ChatMessageEventHandler>() },
            { Priority.Low, new List<ChatMessageEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a whisper was sent from the client to another user.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterWhisperSentNotification</see> and
        /// <see>UnregisterWhisperSentNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChatMessageEventHandler WhisperSent
        {
            add
            {
                lock (__WhisperSent)
                {
                    if (!__WhisperSent.ContainsKey(Priority.Normal))
                    {
                        __WhisperSent.Add(Priority.Normal, new List<ChatMessageEventHandler>());
                    }
                }
                __WhisperSent[Priority.Normal].Add(value);
            }
            remove
            {
                if (__WhisperSent.ContainsKey(Priority.Normal))
                {
                    __WhisperSent[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>WhisperSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="WhisperSent" />
        /// <seealso cref="UnregisterWhisperSentNotification" />
        public void RegisterWhisperSentNotification(Priority p, ChatMessageEventHandler callback)
        {
            lock (__WhisperSent)
            {
                if (!__WhisperSent.ContainsKey(p))
                {
                    __WhisperSent.Add(p, new List<ChatMessageEventHandler>());
                }
            }
            __WhisperSent[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>WhisperSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="WhisperSent" />
        /// <seealso cref="RegisterWhisperSentNotification" />
        public void UnregisterWhisperSentNotification(Priority p, ChatMessageEventHandler callback)
        {
            if (__WhisperSent.ContainsKey(p))
            {
                __WhisperSent[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the WhisperSent event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>WhisperSent</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="WhisperSent" />
        protected virtual void OnWhisperSent(ChatMessageEventArgs e)
        {
            __InvokeWhisperSent(Priority.High, e);
        }

        private void __InvokeWhisperSent(Priority p, ChatMessageEventArgs e)
        {
            foreach (ChatMessageEventHandler eh in __WhisperSent[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "WhisperSent"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeWhisperSent) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeWhisperSent) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region WhisperReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ChatMessageEventHandler>> __WhisperReceived = new Dictionary<Priority, List<ChatMessageEventHandler>>(3)
        {
            { Priority.High, new List<ChatMessageEventHandler>() },
            { Priority.Normal, new List<ChatMessageEventHandler>() },
            { Priority.Low, new List<ChatMessageEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a whisper was received from another user.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterWhisperReceivedNotification</see> and
        /// <see>UnregisterWhisperReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChatMessageEventHandler WhisperReceived
        {
            add
            {
                lock (__WhisperReceived)
                {
                    if (!__WhisperReceived.ContainsKey(Priority.Normal))
                    {
                        __WhisperReceived.Add(Priority.Normal, new List<ChatMessageEventHandler>());
                    }
                }
                __WhisperReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__WhisperReceived.ContainsKey(Priority.Normal))
                {
                    __WhisperReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>WhisperReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="WhisperReceived" />
        /// <seealso cref="UnregisterWhisperReceivedNotification" />
        public void RegisterWhisperReceivedNotification(Priority p, ChatMessageEventHandler callback)
        {
            lock (__WhisperReceived)
            {
                if (!__WhisperReceived.ContainsKey(p))
                {
                    __WhisperReceived.Add(p, new List<ChatMessageEventHandler>());
                }
            }
            __WhisperReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>WhisperReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="WhisperReceived" />
        /// <seealso cref="RegisterWhisperReceivedNotification" />
        public void UnregisterWhisperReceivedNotification(Priority p, ChatMessageEventHandler callback)
        {
            if (__WhisperReceived.ContainsKey(p))
            {
                __WhisperReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the WhisperReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>WhisperReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="WhisperReceived" />
        protected virtual void OnWhisperReceived(ChatMessageEventArgs e)
        {
            __InvokeWhisperReceived(Priority.High, e);
        }

        private void __InvokeWhisperReceived(Priority p, ChatMessageEventArgs e)
        {
            foreach (ChatMessageEventHandler eh in __WhisperReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "WhisperReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeWhisperReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeWhisperReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region UserSpoke event
        [NonSerialized]
        private Dictionary<Priority, List<ChatMessageEventHandler>> __UserSpoke = new Dictionary<Priority, List<ChatMessageEventHandler>>(3)
        {
            { Priority.High, new List<ChatMessageEventHandler>() },
            { Priority.Normal, new List<ChatMessageEventHandler>() },
            { Priority.Low, new List<ChatMessageEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user spoke in the channel.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserSpokeNotification</see> and
        /// <see>UnregisterUserSpokeNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChatMessageEventHandler UserSpoke
        {
            add
            {
                lock (__UserSpoke)
                {
                    if (!__UserSpoke.ContainsKey(Priority.Normal))
                    {
                        __UserSpoke.Add(Priority.Normal, new List<ChatMessageEventHandler>());
                    }
                }
                __UserSpoke[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserSpoke.ContainsKey(Priority.Normal))
                {
                    __UserSpoke[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserSpoke</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserSpoke" />
        /// <seealso cref="UnregisterUserSpokeNotification" />
        public void RegisterUserSpokeNotification(Priority p, ChatMessageEventHandler callback)
        {
            lock (__UserSpoke)
            {
                if (!__UserSpoke.ContainsKey(p))
                {
                    __UserSpoke.Add(p, new List<ChatMessageEventHandler>());
                }
            }
            __UserSpoke[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserSpoke</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserSpoke" />
        /// <seealso cref="RegisterUserSpokeNotification" />
        public void UnregisterUserSpokeNotification(Priority p, ChatMessageEventHandler callback)
        {
            if (__UserSpoke.ContainsKey(p))
            {
                __UserSpoke[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserSpoke event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserSpoke</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserSpoke" />
        protected virtual void OnUserSpoke(ChatMessageEventArgs e)
        {
            __InvokeUserSpoke(Priority.High, e);
        }

        private void __InvokeUserSpoke(Priority p, ChatMessageEventArgs e)
        {
            foreach (ChatMessageEventHandler eh in __UserSpoke[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserSpoke"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeUserSpoke) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeUserSpoke) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region UserEmoted event
        [NonSerialized]
        private Dictionary<Priority, List<ChatMessageEventHandler>> __UserEmoted = new Dictionary<Priority, List<ChatMessageEventHandler>>(3)
        {
            { Priority.High, new List<ChatMessageEventHandler>() },
            { Priority.Normal, new List<ChatMessageEventHandler>() },
            { Priority.Low, new List<ChatMessageEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user emoted in the current channel.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserEmotedNotification</see> and
        /// <see>UnregisterUserEmotedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChatMessageEventHandler UserEmoted
        {
            add
            {
                lock (__UserEmoted)
                {
                    if (!__UserEmoted.ContainsKey(Priority.Normal))
                    {
                        __UserEmoted.Add(Priority.Normal, new List<ChatMessageEventHandler>());
                    }
                }
                __UserEmoted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserEmoted.ContainsKey(Priority.Normal))
                {
                    __UserEmoted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserEmoted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserEmoted" />
        /// <seealso cref="UnregisterUserEmotedNotification" />
        public void RegisterUserEmotedNotification(Priority p, ChatMessageEventHandler callback)
        {
            lock (__UserEmoted)
            {
                if (!__UserEmoted.ContainsKey(p))
                {
                    __UserEmoted.Add(p, new List<ChatMessageEventHandler>());
                }
            }
            __UserEmoted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserEmoted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserEmoted" />
        /// <seealso cref="RegisterUserEmotedNotification" />
        public void UnregisterUserEmotedNotification(Priority p, ChatMessageEventHandler callback)
        {
            if (__UserEmoted.ContainsKey(p))
            {
                __UserEmoted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserEmoted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserEmoted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserEmoted" />
        protected virtual void OnUserEmoted(ChatMessageEventArgs e)
        {
            __InvokeUserEmoted(Priority.High, e);
        }

        private void __InvokeUserEmoted(Priority p, ChatMessageEventArgs e)
        {
            foreach (ChatMessageEventHandler eh in __UserEmoted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserEmoted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeUserEmoted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeUserEmoted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region MessageSent event
        [NonSerialized]
        private Dictionary<Priority, List<ChatMessageEventHandler>> __MessageSent = new Dictionary<Priority, List<ChatMessageEventHandler>>(3)
        {
            { Priority.High, new List<ChatMessageEventHandler>() },
            { Priority.Normal, new List<ChatMessageEventHandler>() },
            { Priority.Low, new List<ChatMessageEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has sent a message to Battle.net.
        /// </summary>
        /// <remarks>
        /// <para>The event handlers should check the <see cref="ChatEventArgs.EventType">EventType property</see> of the event arguments to 
        /// determine whether this event was an emote or a standard talk command and present the text appropriately.</para>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterMessageSentNotification</see> and
        /// <see>UnregisterMessageSentNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChatMessageEventHandler MessageSent
        {
            add
            {
                lock (__MessageSent)
                {
                    if (!__MessageSent.ContainsKey(Priority.Normal))
                    {
                        __MessageSent.Add(Priority.Normal, new List<ChatMessageEventHandler>());
                    }
                }
                __MessageSent[Priority.Normal].Add(value);
            }
            remove
            {
                if (__MessageSent.ContainsKey(Priority.Normal))
                {
                    __MessageSent[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>MessageSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="MessageSent" />
        /// <seealso cref="UnregisterMessageSentNotification" />
        public void RegisterMessageSentNotification(Priority p, ChatMessageEventHandler callback)
        {
            lock (__MessageSent)
            {
                if (!__MessageSent.ContainsKey(p))
                {
                    __MessageSent.Add(p, new List<ChatMessageEventHandler>());
                }
            }
            __MessageSent[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>MessageSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="MessageSent" />
        /// <seealso cref="RegisterMessageSentNotification" />
        public void UnregisterMessageSentNotification(Priority p, ChatMessageEventHandler callback)
        {
            if (__MessageSent.ContainsKey(p))
            {
                __MessageSent[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the MessageSent event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>MessageSent</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="MessageSent" />
        protected virtual void OnMessageSent(ChatMessageEventArgs e)
        {
            __InvokeMessageSent(Priority.High, e);
        }

        private void __InvokeMessageSent(Priority p, ChatMessageEventArgs e)
        {
            foreach (ChatMessageEventHandler eh in __MessageSent[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "MessageSent"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeMessageSent) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChatMessageEventArgs> { Arguments = e, Target = new Invokee<ChatMessageEventArgs>(__InvokeMessageSent) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region CommandSent event
        [NonSerialized]
        private Dictionary<Priority, List<InformationEventHandler>> __CommandSent = new Dictionary<Priority, List<InformationEventHandler>>(3)
        {
            { Priority.High, new List<InformationEventHandler>() },
            { Priority.Normal, new List<InformationEventHandler>() },
            { Priority.Low, new List<InformationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a command was sent to the server.
        /// </summary>
        /// <remarks>
        /// <para>This event is fired whenever the user sends a slash command to the server, except for /me or /emote commands.  The /me and 
        /// /emote commands are informed through the <see>MessageSent</see> event.</para>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterCommandSentNotification</see> and
        /// <see>UnregisterCommandSentNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event InformationEventHandler CommandSent
        {
            add
            {
                lock (__CommandSent)
                {
                    if (!__CommandSent.ContainsKey(Priority.Normal))
                    {
                        __CommandSent.Add(Priority.Normal, new List<InformationEventHandler>());
                    }
                }
                __CommandSent[Priority.Normal].Add(value);
            }
            remove
            {
                if (__CommandSent.ContainsKey(Priority.Normal))
                {
                    __CommandSent[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>CommandSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="CommandSent" />
        /// <seealso cref="UnregisterCommandSentNotification" />
        public void RegisterCommandSentNotification(Priority p, InformationEventHandler callback)
        {
            lock (__CommandSent)
            {
                if (!__CommandSent.ContainsKey(p))
                {
                    __CommandSent.Add(p, new List<InformationEventHandler>());
                }
            }
            __CommandSent[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>CommandSent</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="CommandSent" />
        /// <seealso cref="RegisterCommandSentNotification" />
        public void UnregisterCommandSentNotification(Priority p, InformationEventHandler callback)
        {
            if (__CommandSent.ContainsKey(p))
            {
                __CommandSent[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the CommandSent event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>CommandSent</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="CommandSent" />
        protected virtual void OnCommandSent(InformationEventArgs e)
        {
            __InvokeCommandSent(Priority.High, e);
        }

        private void __InvokeCommandSent(Priority p, InformationEventArgs e)
        {
            foreach (InformationEventHandler eh in __CommandSent[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "CommandSent"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeCommandSent) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeCommandSent) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region 0x51 events (client check passed/failed)
        #region ClientCheckPassed event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __ClientCheckPassed = new Dictionary<Priority, List<EventHandler>>(3)
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client versioning check was successful.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClientCheckPassedNotification</see> and
        /// <see>UnregisterClientCheckPassedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event EventHandler ClientCheckPassed
        {
            add
            {
                lock (__ClientCheckPassed)
                {
                    if (!__ClientCheckPassed.ContainsKey(Priority.Normal))
                    {
                        __ClientCheckPassed.Add(Priority.Normal, new List<EventHandler>());
                    }
                }
                __ClientCheckPassed[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClientCheckPassed.ContainsKey(Priority.Normal))
                {
                    __ClientCheckPassed[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClientCheckPassed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClientCheckPassed" />
        /// <seealso cref="UnregisterClientCheckPassedNotification" />
        public void RegisterClientCheckPassedNotification(Priority p, EventHandler callback)
        {
            lock (__ClientCheckPassed)
            {
                if (!__ClientCheckPassed.ContainsKey(p))
                {
                    __ClientCheckPassed.Add(p, new List<EventHandler>());
                }
            }
            __ClientCheckPassed[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClientCheckPassed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClientCheckPassed" />
        /// <seealso cref="RegisterClientCheckPassedNotification" />
        public void UnregisterClientCheckPassedNotification(Priority p, EventHandler callback)
        {
            if (__ClientCheckPassed.ContainsKey(p))
            {
                __ClientCheckPassed[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClientCheckPassed event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClientCheckPassed</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClientCheckPassed" />
        protected virtual void OnClientCheckPassed(BaseEventArgs e)
        {
            __InvokeClientCheckPassed(Priority.High, e);
        }

        private void __InvokeClientCheckPassed(Priority p, BaseEventArgs e)
        {
            foreach (EventHandler eh in __ClientCheckPassed[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClientCheckPassed"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<BaseEventArgs> { Arguments = e, Target = new Invokee<BaseEventArgs>(__InvokeClientCheckPassed) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<BaseEventArgs> { Arguments = e, Target = new Invokee<BaseEventArgs>(__InvokeClientCheckPassed) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClientCheckFailed event
        [NonSerialized]
        private Dictionary<Priority, List<ClientCheckFailedEventHandler>> __ClientCheckFailed = new Dictionary<Priority, List<ClientCheckFailedEventHandler>>(3)
        {
            { Priority.High, new List<ClientCheckFailedEventHandler>() },
            { Priority.Normal, new List<ClientCheckFailedEventHandler>() },
            { Priority.Low, new List<ClientCheckFailedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client versioning check failed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClientCheckFailedNotification</see> and
        /// <see>UnregisterClientCheckFailedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClientCheckFailedEventHandler ClientCheckFailed
        {
            add
            {
                lock (__ClientCheckFailed)
                {
                    if (!__ClientCheckFailed.ContainsKey(Priority.Normal))
                    {
                        __ClientCheckFailed.Add(Priority.Normal, new List<ClientCheckFailedEventHandler>());
                    }
                }
                __ClientCheckFailed[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClientCheckFailed.ContainsKey(Priority.Normal))
                {
                    __ClientCheckFailed[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClientCheckFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClientCheckFailed" />
        /// <seealso cref="UnregisterClientCheckFailedNotification" />
        public void RegisterClientCheckFailedNotification(Priority p, ClientCheckFailedEventHandler callback)
        {
            lock (__ClientCheckFailed)
            {
                if (!__ClientCheckFailed.ContainsKey(p))
                {
                    __ClientCheckFailed.Add(p, new List<ClientCheckFailedEventHandler>());
                }
            }
            __ClientCheckFailed[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClientCheckFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClientCheckFailed" />
        /// <seealso cref="RegisterClientCheckFailedNotification" />
        public void UnregisterClientCheckFailedNotification(Priority p, ClientCheckFailedEventHandler callback)
        {
            if (__ClientCheckFailed.ContainsKey(p))
            {
                __ClientCheckFailed[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClientCheckFailed event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClientCheckFailed</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClientCheckFailed" />
        protected virtual void OnClientCheckFailed(ClientCheckFailedEventArgs e)
        {
            __InvokeClientCheckFailed(Priority.High, e);
        }

        private void __InvokeClientCheckFailed(Priority p, ClientCheckFailedEventArgs e)
        {
            foreach (ClientCheckFailedEventHandler eh in __ClientCheckFailed[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClientCheckFailed"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClientCheckFailedEventArgs> { Arguments = e, Target = new Invokee<ClientCheckFailedEventArgs>(__InvokeClientCheckFailed) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClientCheckFailedEventArgs> { Arguments = e, Target = new Invokee<ClientCheckFailedEventArgs>(__InvokeClientCheckFailed) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
		
        #endregion

        #region login events
        #region LoginSucceeded event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __LoginSucceeded = new Dictionary<Priority, List<EventHandler>>(3)
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client login succeeded.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterLoginSucceededNotification</see> and
        /// <see>UnregisterLoginSucceededNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event EventHandler LoginSucceeded
        {
            add
            {
                lock (__LoginSucceeded)
                {
                    if (!__LoginSucceeded.ContainsKey(Priority.Normal))
                    {
                        __LoginSucceeded.Add(Priority.Normal, new List<EventHandler>());
                    }
                }
                __LoginSucceeded[Priority.Normal].Add(value);
            }
            remove
            {
                if (__LoginSucceeded.ContainsKey(Priority.Normal))
                {
                    __LoginSucceeded[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>LoginSucceeded</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="LoginSucceeded" />
        /// <seealso cref="UnregisterLoginSucceededNotification" />
        public void RegisterLoginSucceededNotification(Priority p, EventHandler callback)
        {
            lock (__LoginSucceeded)
            {
                if (!__LoginSucceeded.ContainsKey(p))
                {
                    __LoginSucceeded.Add(p, new List<EventHandler>());
                }
            }
            __LoginSucceeded[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>LoginSucceeded</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="LoginSucceeded" />
        /// <seealso cref="RegisterLoginSucceededNotification" />
        public void UnregisterLoginSucceededNotification(Priority p, EventHandler callback)
        {
            if (__LoginSucceeded.ContainsKey(p))
            {
                __LoginSucceeded[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the LoginSucceeded event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>LoginSucceeded</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="LoginSucceeded" />
        protected virtual void OnLoginSucceeded(EventArgs e)
        {
            __InvokeLoginSucceeded(Priority.High, e);
        }

        private void __InvokeLoginSucceeded(Priority p, EventArgs e)
        {
            foreach (EventHandler eh in __LoginSucceeded[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "LoginSucceeded"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeLoginSucceeded) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeLoginSucceeded) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region LoginFailed event
        [NonSerialized]
        private Dictionary<Priority, List<LoginFailedEventHandler>> __LoginFailed = new Dictionary<Priority, List<LoginFailedEventHandler>>(3)
        {
            { Priority.High, new List<LoginFailedEventHandler>() },
            { Priority.Normal, new List<LoginFailedEventHandler>() },
            { Priority.Low, new List<LoginFailedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client login failed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterLoginFailedNotification</see> and
        /// <see>UnregisterLoginFailedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event LoginFailedEventHandler LoginFailed
        {
            add
            {
                lock (__LoginFailed)
                {
                    if (!__LoginFailed.ContainsKey(Priority.Normal))
                    {
                        __LoginFailed.Add(Priority.Normal, new List<LoginFailedEventHandler>());
                    }
                }
                __LoginFailed[Priority.Normal].Add(value);
            }
            remove
            {
                if (__LoginFailed.ContainsKey(Priority.Normal))
                {
                    __LoginFailed[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>LoginFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="LoginFailed" />
        /// <seealso cref="UnregisterLoginFailedNotification" />
        public void RegisterLoginFailedNotification(Priority p, LoginFailedEventHandler callback)
        {
            lock (__LoginFailed)
            {
                if (!__LoginFailed.ContainsKey(p))
                {
                    __LoginFailed.Add(p, new List<LoginFailedEventHandler>());
                }
            }
            __LoginFailed[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>LoginFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="LoginFailed" />
        /// <seealso cref="RegisterLoginFailedNotification" />
        public void UnregisterLoginFailedNotification(Priority p, LoginFailedEventHandler callback)
        {
            if (__LoginFailed.ContainsKey(p))
            {
                __LoginFailed[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the LoginFailed event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>LoginFailed</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="LoginFailed" />
        protected virtual void OnLoginFailed(LoginFailedEventArgs e)
        {
            __InvokeLoginFailed(Priority.High, e);
        }

        private void __InvokeLoginFailed(Priority p, LoginFailedEventArgs e)
        {
            foreach (LoginFailedEventHandler eh in __LoginFailed[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "LoginFailed"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<LoginFailedEventArgs> { Arguments = e, Target = new Invokee<LoginFailedEventArgs>(__InvokeLoginFailed) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<LoginFailedEventArgs> { Arguments = e, Target = new Invokee<LoginFailedEventArgs>(__InvokeLoginFailed) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #region EnteredChat event
        [NonSerialized]
        private Dictionary<Priority, List<EnteredChatEventHandler>> __EnteredChat = new Dictionary<Priority, List<EnteredChatEventHandler>>(3)
        {
            { Priority.High, new List<EnteredChatEventHandler>() },
            { Priority.Normal, new List<EnteredChatEventHandler>() },
            { Priority.Low, new List<EnteredChatEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has entered chat.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterEnteredChatNotification</see> and
        /// <see>UnregisterEnteredChatNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event EnteredChatEventHandler EnteredChat
        {
            add
            {
                lock (__EnteredChat)
                {
                    if (!__EnteredChat.ContainsKey(Priority.Normal))
                    {
                        __EnteredChat.Add(Priority.Normal, new List<EnteredChatEventHandler>());
                    }
                }
                __EnteredChat[Priority.Normal].Add(value);
            }
            remove
            {
                if (__EnteredChat.ContainsKey(Priority.Normal))
                {
                    __EnteredChat[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>EnteredChat</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="EnteredChat" />
        /// <seealso cref="UnregisterEnteredChatNotification" />
        public void RegisterEnteredChatNotification(Priority p, EnteredChatEventHandler callback)
        {
            lock (__EnteredChat)
            {
                if (!__EnteredChat.ContainsKey(p))
                {
                    __EnteredChat.Add(p, new List<EnteredChatEventHandler>());
                }
            }
            __EnteredChat[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>EnteredChat</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="EnteredChat" />
        /// <seealso cref="RegisterEnteredChatNotification" />
        public void UnregisterEnteredChatNotification(Priority p, EnteredChatEventHandler callback)
        {
            if (__EnteredChat.ContainsKey(p))
            {
                __EnteredChat[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the EnteredChat event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>EnteredChat</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="EnteredChat" />
        protected virtual void OnEnteredChat(EnteredChatEventArgs e)
        {
            __InvokeEnteredChat(Priority.High, e);
        }

        private void __InvokeEnteredChat(Priority p, EnteredChatEventArgs e)
        {
            foreach (EnteredChatEventHandler eh in __EnteredChat[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "EnteredChat"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<EnteredChatEventArgs> { Arguments = e, Target = new Invokee<EnteredChatEventArgs>(__InvokeEnteredChat) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<EnteredChatEventArgs> { Arguments = e, Target = new Invokee<EnteredChatEventArgs>(__InvokeEnteredChat) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region AccountCreated event
        [NonSerialized]
        private Dictionary<Priority, List<AccountCreationEventHandler>> __AccountCreated = new Dictionary<Priority, List<AccountCreationEventHandler>>(3)
        {
            { Priority.High, new List<AccountCreationEventHandler>() },
            { Priority.Normal, new List<AccountCreationEventHandler>() },
            { Priority.Low, new List<AccountCreationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a new account has been created.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterAccountCreatedNotification</see> and
        /// <see>UnregisterAccountCreatedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event AccountCreationEventHandler AccountCreated
        {
            add
            {
                lock (__AccountCreated)
                {
                    if (!__AccountCreated.ContainsKey(Priority.Normal))
                    {
                        __AccountCreated.Add(Priority.Normal, new List<AccountCreationEventHandler>());
                    }
                }
                __AccountCreated[Priority.Normal].Add(value);
            }
            remove
            {
                if (__AccountCreated.ContainsKey(Priority.Normal))
                {
                    __AccountCreated[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>AccountCreated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="AccountCreated" />
        /// <seealso cref="UnregisterAccountCreatedNotification" />
        public void RegisterAccountCreatedNotification(Priority p, AccountCreationEventHandler callback)
        {
            lock (__AccountCreated)
            {
                if (!__AccountCreated.ContainsKey(p))
                {
                    __AccountCreated.Add(p, new List<AccountCreationEventHandler>());
                }
            }
            __AccountCreated[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>AccountCreated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="AccountCreated" />
        /// <seealso cref="RegisterAccountCreatedNotification" />
        public void UnregisterAccountCreatedNotification(Priority p, AccountCreationEventHandler callback)
        {
            if (__AccountCreated.ContainsKey(p))
            {
                __AccountCreated[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the AccountCreated event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>AccountCreated</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="AccountCreated" />
        protected virtual void OnAccountCreated(AccountCreationEventArgs e)
        {
            __InvokeAccountCreated(Priority.High, e);
        }

        private void __InvokeAccountCreated(Priority p, AccountCreationEventArgs e)
        {
            foreach (AccountCreationEventHandler eh in __AccountCreated[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "AccountCreated"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<AccountCreationEventArgs> { Arguments = e, Target = new Invokee<AccountCreationEventArgs>(__InvokeAccountCreated) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<AccountCreationEventArgs> { Arguments = e, Target = new Invokee<AccountCreationEventArgs>(__InvokeAccountCreated) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region AccountCreationFailed event
        [NonSerialized]
        private Dictionary<Priority, List<AccountCreationFailedEventHandler>> __AccountCreationFailed = new Dictionary<Priority, List<AccountCreationFailedEventHandler>>(3)
        {
            { Priority.High, new List<AccountCreationFailedEventHandler>() },
            { Priority.Normal, new List<AccountCreationFailedEventHandler>() },
            { Priority.Low, new List<AccountCreationFailedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that an attempt to create an account has failed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterAccountCreationFailedNotification</see> and
        /// <see>UnregisterAccountCreationFailedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event AccountCreationFailedEventHandler AccountCreationFailed
        {
            add
            {
                lock (__AccountCreationFailed)
                {
                    if (!__AccountCreationFailed.ContainsKey(Priority.Normal))
                    {
                        __AccountCreationFailed.Add(Priority.Normal, new List<AccountCreationFailedEventHandler>());
                    }
                }
                __AccountCreationFailed[Priority.Normal].Add(value);
            }
            remove
            {
                if (__AccountCreationFailed.ContainsKey(Priority.Normal))
                {
                    __AccountCreationFailed[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>AccountCreationFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="AccountCreationFailed" />
        /// <seealso cref="UnregisterAccountCreationFailedNotification" />
        public void RegisterAccountCreationFailedNotification(Priority p, AccountCreationFailedEventHandler callback)
        {
            lock (__AccountCreationFailed)
            {
                if (!__AccountCreationFailed.ContainsKey(p))
                {
                    __AccountCreationFailed.Add(p, new List<AccountCreationFailedEventHandler>());
                }
            }
            __AccountCreationFailed[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>AccountCreationFailed</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="AccountCreationFailed" />
        /// <seealso cref="RegisterAccountCreationFailedNotification" />
        public void UnregisterAccountCreationFailedNotification(Priority p, AccountCreationFailedEventHandler callback)
        {
            if (__AccountCreationFailed.ContainsKey(p))
            {
                __AccountCreationFailed[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the AccountCreationFailed event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>AccountCreationFailed</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="AccountCreationFailed" />
        protected virtual void OnAccountCreationFailed(AccountCreationFailedEventArgs e)
        {
            foreach (AccountCreationFailedEventHandler eh in __AccountCreationFailed[Priority.High])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "AccountCreationFailed"),
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (AccountCreationFailedEventHandler eh in __AccountCreationFailed[Priority.Normal])
                {
                    try
                    {
                        eh(this, e);
                    }
                    catch (Exception ex)
                    {
                        ReportException(
                            ex,
                            new KeyValuePair<string, object>("delegate", eh),
                            new KeyValuePair<string, object>("Event", "AccountCreationFailed"),
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (AccountCreationFailedEventHandler eh in __AccountCreationFailed[Priority.Low])
                    {
                        try
                        {
                            eh(this, e);
                        }
                        catch (Exception ex)
                        {
                            ReportException(
                                ex,
                                new KeyValuePair<string, object>("delegate", eh),
                                new KeyValuePair<string, object>("Event", "AccountCreationFailed"),
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e as BaseEventArgs);
                });
            });
        }
        #endregion
        #endregion

        #region informational events
        #region ChannelListReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ChannelListEventHandler>> __ChannelListReceived = new Dictionary<Priority, List<ChannelListEventHandler>>(3)
        {
            { Priority.High, new List<ChannelListEventHandler>() },
            { Priority.Normal, new List<ChannelListEventHandler>() },
            { Priority.Low, new List<ChannelListEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the channel list has been provided by the server.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterChannelListReceivedNotification</see> and
        /// <see>UnregisterChannelListReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ChannelListEventHandler ChannelListReceived
        {
            add
            {
                lock (__ChannelListReceived)
                {
                    if (!__ChannelListReceived.ContainsKey(Priority.Normal))
                    {
                        __ChannelListReceived.Add(Priority.Normal, new List<ChannelListEventHandler>());
                    }
                }
                __ChannelListReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ChannelListReceived.ContainsKey(Priority.Normal))
                {
                    __ChannelListReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ChannelListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ChannelListReceived" />
        /// <seealso cref="UnregisterChannelListReceivedNotification" />
        public void RegisterChannelListReceivedNotification(Priority p, ChannelListEventHandler callback)
        {
            lock (__ChannelListReceived)
            {
                if (!__ChannelListReceived.ContainsKey(p))
                {
                    __ChannelListReceived.Add(p, new List<ChannelListEventHandler>());
                }
            }
            __ChannelListReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ChannelListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ChannelListReceived" />
        /// <seealso cref="RegisterChannelListReceivedNotification" />
        public void UnregisterChannelListReceivedNotification(Priority p, ChannelListEventHandler callback)
        {
            if (__ChannelListReceived.ContainsKey(p))
            {
                __ChannelListReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ChannelListReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ChannelListReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ChannelListReceived" />
        protected virtual void OnChannelListReceived(ChannelListEventArgs e)
        {
            __InvokeChannelListReceived(Priority.High, e);
        }

        private void __InvokeChannelListReceived(Priority p, ChannelListEventArgs e)
        {
            foreach (ChannelListEventHandler eh in __ChannelListReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ChannelListReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ChannelListEventArgs> { Arguments = e, Target = new Invokee<ChannelListEventArgs>(__InvokeChannelListReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ChannelListEventArgs> { Arguments = e, Target = new Invokee<ChannelListEventArgs>(__InvokeChannelListReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region Error event
        [NonSerialized]
        private Dictionary<Priority, List<ErrorEventHandler>> __Error = new Dictionary<Priority, List<ErrorEventHandler>>(3)
        {
            { Priority.High, new List<ErrorEventHandler>() },
            { Priority.Normal, new List<ErrorEventHandler>() },
            { Priority.Low, new List<ErrorEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a general or connection error has occurred.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterErrorNotification</see> and
        /// <see>UnregisterErrorNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ErrorEventHandler Error
        {
            add
            {
                lock (__Error)
                {
                    if (!__Error.ContainsKey(Priority.Normal))
                    {
                        __Error.Add(Priority.Normal, new List<ErrorEventHandler>());
                    }
                }
                __Error[Priority.Normal].Add(value);
            }
            remove
            {
                if (__Error.ContainsKey(Priority.Normal))
                {
                    __Error[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>Error</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="Error" />
        /// <seealso cref="UnregisterErrorNotification" />
        public void RegisterErrorNotification(Priority p, ErrorEventHandler callback)
        {
            lock (__Error)
            {
                if (!__Error.ContainsKey(p))
                {
                    __Error.Add(p, new List<ErrorEventHandler>());
                }
            }
            __Error[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>Error</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="Error" />
        /// <seealso cref="RegisterErrorNotification" />
        public void UnregisterErrorNotification(Priority p, ErrorEventHandler callback)
        {
            if (__Error.ContainsKey(p))
            {
                __Error[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the Error event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>Error</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="Error" />
        protected virtual void OnError(ErrorEventArgs e)
        {
            __InvokeError(Priority.High, e);
        }

        private void __InvokeError(Priority p, ErrorEventArgs e)
        {
            foreach (ErrorEventHandler eh in __Error[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "Error"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ErrorEventArgs> { Arguments = e, Target = new Invokee<ErrorEventArgs>(__InvokeError) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ErrorEventArgs> { Arguments = e, Target = new Invokee<ErrorEventArgs>(__InvokeError) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region Information event
        [NonSerialized]
        private Dictionary<Priority, List<InformationEventHandler>> __Information = new Dictionary<Priority, List<InformationEventHandler>>(3)
        {
            { Priority.High, new List<InformationEventHandler>() },
            { Priority.Normal, new List<InformationEventHandler>() },
            { Priority.Low, new List<InformationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners about a general informational message.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterInformationNotification</see> and
        /// <see>UnregisterInformationNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event InformationEventHandler Information
        {
            add
            {
                lock (__Information)
                {
                    if (!__Information.ContainsKey(Priority.Normal))
                    {
                        __Information.Add(Priority.Normal, new List<InformationEventHandler>());
                    }
                }
                __Information[Priority.Normal].Add(value);
            }
            remove
            {
                if (__Information.ContainsKey(Priority.Normal))
                {
                    __Information[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>Information</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="Information" />
        /// <seealso cref="UnregisterInformationNotification" />
        public void RegisterInformationNotification(Priority p, InformationEventHandler callback)
        {
            lock (__Information)
            {
                if (!__Information.ContainsKey(p))
                {
                    __Information.Add(p, new List<InformationEventHandler>());
                }
            }
            __Information[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>Information</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="Information" />
        /// <seealso cref="RegisterInformationNotification" />
        public void UnregisterInformationNotification(Priority p, InformationEventHandler callback)
        {
            if (__Information.ContainsKey(p))
            {
                __Information[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the Information event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>Information</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="Information" />
        protected virtual void OnInformation(InformationEventArgs e)
        {
            __InvokeInformation(Priority.High, e);
        }

        private void __InvokeInformation(Priority p, InformationEventArgs e)
        {
            foreach (InformationEventHandler eh in __Information[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "Information"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeInformation) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeInformation) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region connection events
        #region Connected event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __Connected = new Dictionary<Priority, List<EventHandler>>(3) 
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the application has connected to the server.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterConnectedNotification</see> and
        /// <see>UnregisterConnectedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event EventHandler Connected
        {
            add
            {
                lock (__Connected)
                {
                    if (!__Connected.ContainsKey(Priority.Normal))
                    {
                        __Connected.Add(Priority.Normal, new List<EventHandler>());
                    }
                }
                __Connected[Priority.Normal].Add(value);
            }
            remove
            {
                if (__Connected.ContainsKey(Priority.Normal))
                {
                    __Connected[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>Connected</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="Connected" />
        /// <seealso cref="UnregisterConnectedNotification" />
        public void RegisterConnectedNotification(Priority p, EventHandler callback)
        {
            lock (__Connected)
            {
                if (!__Connected.ContainsKey(p))
                {
                    __Connected.Add(p, new List<EventHandler>());
                }
            }
            __Connected[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>Connected</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="Connected" />
        /// <seealso cref="RegisterConnectedNotification" />
        public void UnregisterConnectedNotification(Priority p, EventHandler callback)
        {
            if (__Connected.ContainsKey(p))
            {
                __Connected[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the Connected event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>Connected</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="Connected" />
        protected virtual void OnConnected(EventArgs e)
        {
            __InvokeConnected(Priority.High, e);
        }

        private void __InvokeConnected(Priority p, EventArgs e)
        {
            foreach (EventHandler eh in __Connected[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "Connected"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeConnected) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeConnected) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region Disconnected event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __Disconnected = new Dictionary<Priority, List<EventHandler>>(3)
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has disconnected.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterDisconnectedNotification</see> and
        /// <see>UnregisterDisconnectedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event EventHandler Disconnected
        {
            add
            {
                lock (__Disconnected)
                {
                    if (!__Disconnected.ContainsKey(Priority.Normal))
                    {
                        __Disconnected.Add(Priority.Normal, new List<EventHandler>());
                    }
                }
                __Disconnected[Priority.Normal].Add(value);
            }
            remove
            {
                if (__Disconnected.ContainsKey(Priority.Normal))
                {
                    __Disconnected[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>Disconnected</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="Disconnected" />
        /// <seealso cref="UnregisterDisconnectedNotification" />
        public void RegisterDisconnectedNotification(Priority p, EventHandler callback)
        {
            lock (__Disconnected)
            {
                if (!__Disconnected.ContainsKey(p))
                {
                    __Disconnected.Add(p, new List<EventHandler>());
                }
            }
            __Disconnected[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>Disconnected</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="Disconnected" />
        /// <seealso cref="RegisterDisconnectedNotification" />
        public void UnregisterDisconnectedNotification(Priority p, EventHandler callback)
        {
            if (__Disconnected.ContainsKey(p))
            {
                __Disconnected[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the Disconnected event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>Disconnected</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="Disconnected" />
        protected virtual void OnDisconnected(EventArgs e)
        {
            __InvokeDisconnected(Priority.High, e);
        }

        private void __InvokeDisconnected(Priority p, EventArgs e)
        {
            foreach (EventHandler eh in __Disconnected[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "Disconnected"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeDisconnected) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<EventArgs> { Arguments = e, Target = new Invokee<EventArgs>(__InvokeDisconnected) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region WardentUnhandled event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __WardentUnhandled = new Dictionary<Priority, List<EventHandler>>(3)
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the server has challenged the client with a warden handshake, but that the client did not have a 
        /// Warden plugin enabled.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterWardentUnhandledNotification</see> and
        /// <see>UnregisterWardentUnhandledNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        /// <seealso cref="BNSharp.Plugins.IWardenModule"/>
        public event EventHandler WardentUnhandled
        {
            add
            {
                lock (__WardentUnhandled)
                {
                    if (!__WardentUnhandled.ContainsKey(Priority.Normal))
                    {
                        __WardentUnhandled.Add(Priority.Normal, new List<EventHandler>());
                    }
                }
                __WardentUnhandled[Priority.Normal].Add(value);
            }
            remove
            {
                if (__WardentUnhandled.ContainsKey(Priority.Normal))
                {
                    __WardentUnhandled[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>WardentUnhandled</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="WardentUnhandled" />
        /// <seealso cref="UnregisterWardentUnhandledNotification" />
        public void RegisterWardentUnhandledNotification(Priority p, EventHandler callback)
        {
            lock (__WardentUnhandled)
            {
                if (!__WardentUnhandled.ContainsKey(p))
                {
                    __WardentUnhandled.Add(p, new List<EventHandler>());
                }
            }
            __WardentUnhandled[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>WardentUnhandled</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="WardentUnhandled" />
        /// <seealso cref="RegisterWardentUnhandledNotification" />
        public void UnregisterWardentUnhandledNotification(Priority p, EventHandler callback)
        {
            if (__WardentUnhandled.ContainsKey(p))
            {
                __WardentUnhandled[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the WardentUnhandled event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>WardentUnhandled</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="WardentUnhandled" />
        protected virtual void OnWardentUnhandled(EventArgs e)
        {
            foreach (EventHandler eh in __WardentUnhandled[Priority.High])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "WardentUnhandled"),
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __WardentUnhandled[Priority.Normal])
                {
                    try
                    {
                        eh(this, e);
                    }
                    catch (Exception ex)
                    {
                        ReportException(
                            ex,
                            new KeyValuePair<string, object>("delegate", eh),
                            new KeyValuePair<string, object>("Event", "WardentUnhandled"),
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __WardentUnhandled[Priority.Low])
                    {
                        try
                        {
                            eh(this, e);
                        }
                        catch (Exception ex)
                        {
                            ReportException(
                                ex,
                                new KeyValuePair<string, object>("delegate", eh),
                                new KeyValuePair<string, object>("Event", "WardentUnhandled"),
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e as BaseEventArgs);
                });
            });
        }
        #endregion
		
        #endregion

        #region ServerNews event
        [NonSerialized]
        private Dictionary<Priority, List<ServerNewsEventHandler>> __ServerNews = new Dictionary<Priority, List<ServerNewsEventHandler>>(3)
        {
            { Priority.High, new List<ServerNewsEventHandler>() },
            { Priority.Normal, new List<ServerNewsEventHandler>() },
            { Priority.Low, new List<ServerNewsEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the server has provided news items to view.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterServerNewsNotification</see> and
        /// <see>UnregisterServerNewsNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ServerNewsEventHandler ServerNews
        {
            add
            {
                lock (__ServerNews)
                {
                    if (!__ServerNews.ContainsKey(Priority.Normal))
                    {
                        __ServerNews.Add(Priority.Normal, new List<ServerNewsEventHandler>());
                    }
                }
                __ServerNews[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ServerNews.ContainsKey(Priority.Normal))
                {
                    __ServerNews[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ServerNews</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ServerNews" />
        /// <seealso cref="UnregisterServerNewsNotification" />
        public void RegisterServerNewsNotification(Priority p, ServerNewsEventHandler callback)
        {
            lock (__ServerNews)
            {
                if (!__ServerNews.ContainsKey(p))
                {
                    __ServerNews.Add(p, new List<ServerNewsEventHandler>());
                }
            }
            __ServerNews[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ServerNews</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ServerNews" />
        /// <seealso cref="RegisterServerNewsNotification" />
        public void UnregisterServerNewsNotification(Priority p, ServerNewsEventHandler callback)
        {
            if (__ServerNews.ContainsKey(p))
            {
                __ServerNews[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ServerNews event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ServerNews</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ServerNews" />
        protected virtual void OnServerNews(ServerNewsEventArgs e)
        {
            __InvokeServerNews(Priority.High, e);
        }

        private void __InvokeServerNews(Priority p, ServerNewsEventArgs e)
        {
            foreach (ServerNewsEventHandler eh in __ServerNews[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ServerNews"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ServerNewsEventArgs> { Arguments = e, Target = new Invokee<ServerNewsEventArgs>(__InvokeServerNews) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ServerNewsEventArgs> { Arguments = e, Target = new Invokee<ServerNewsEventArgs>(__InvokeServerNews) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region clan events
        #region ClanMemberListReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ClanMemberListEventHandler>> __ClanMemberListReceived = new Dictionary<Priority, List<ClanMemberListEventHandler>>(3)
        {
            { Priority.High, new List<ClanMemberListEventHandler>() },
            { Priority.Normal, new List<ClanMemberListEventHandler>() },
            { Priority.Low, new List<ClanMemberListEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the user's clan member list has been received.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMemberListReceivedNotification</see> and
        /// <see>UnregisterClanMemberListReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanMemberListEventHandler ClanMemberListReceived
        {
            add
            {
                lock (__ClanMemberListReceived)
                {
                    if (!__ClanMemberListReceived.ContainsKey(Priority.Normal))
                    {
                        __ClanMemberListReceived.Add(Priority.Normal, new List<ClanMemberListEventHandler>());
                    }
                }
                __ClanMemberListReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMemberListReceived.ContainsKey(Priority.Normal))
                {
                    __ClanMemberListReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMemberListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMemberListReceived" />
        /// <seealso cref="UnregisterClanMemberListReceivedNotification" />
        public void RegisterClanMemberListReceivedNotification(Priority p, ClanMemberListEventHandler callback)
        {
            lock (__ClanMemberListReceived)
            {
                if (!__ClanMemberListReceived.ContainsKey(p))
                {
                    __ClanMemberListReceived.Add(p, new List<ClanMemberListEventHandler>());
                }
            }
            __ClanMemberListReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMemberListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMemberListReceived" />
        /// <seealso cref="RegisterClanMemberListReceivedNotification" />
        public void UnregisterClanMemberListReceivedNotification(Priority p, ClanMemberListEventHandler callback)
        {
            if (__ClanMemberListReceived.ContainsKey(p))
            {
                __ClanMemberListReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMemberListReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMemberListReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMemberListReceived" />
        protected virtual void OnClanMemberListReceived(ClanMemberListEventArgs e)
        {
            __InvokeClanMemberListReceived(Priority.High, e);
        }

        private void __InvokeClanMemberListReceived(Priority p, ClanMemberListEventArgs e)
        {
            foreach (ClanMemberListEventHandler eh in __ClanMemberListReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMemberListReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanMemberListEventArgs> { Arguments = e, Target = new Invokee<ClanMemberListEventArgs>(__InvokeClanMemberListReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanMemberListEventArgs> { Arguments = e, Target = new Invokee<ClanMemberListEventArgs>(__InvokeClanMemberListReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanMemberStatusChanged event
        [NonSerialized]
        private Dictionary<Priority, List<ClanMemberStatusEventHandler>> __ClanMemberStatusChanged = new Dictionary<Priority, List<ClanMemberStatusEventHandler>>(3)
        {
            { Priority.High, new List<ClanMemberStatusEventHandler>() },
            { Priority.Normal, new List<ClanMemberStatusEventHandler>() },
            { Priority.Low, new List<ClanMemberStatusEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a clan member's status has changed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMemberStatusChangedNotification</see> and
        /// <see>UnregisterClanMemberStatusChangedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanMemberStatusEventHandler ClanMemberStatusChanged
        {
            add
            {
                lock (__ClanMemberStatusChanged)
                {
                    if (!__ClanMemberStatusChanged.ContainsKey(Priority.Normal))
                    {
                        __ClanMemberStatusChanged.Add(Priority.Normal, new List<ClanMemberStatusEventHandler>());
                    }
                }
                __ClanMemberStatusChanged[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMemberStatusChanged.ContainsKey(Priority.Normal))
                {
                    __ClanMemberStatusChanged[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMemberStatusChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMemberStatusChanged" />
        /// <seealso cref="UnregisterClanMemberStatusChangedNotification" />
        public void RegisterClanMemberStatusChangedNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            lock (__ClanMemberStatusChanged)
            {
                if (!__ClanMemberStatusChanged.ContainsKey(p))
                {
                    __ClanMemberStatusChanged.Add(p, new List<ClanMemberStatusEventHandler>());
                }
            }
            __ClanMemberStatusChanged[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMemberStatusChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMemberStatusChanged" />
        /// <seealso cref="RegisterClanMemberStatusChangedNotification" />
        public void UnregisterClanMemberStatusChangedNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            if (__ClanMemberStatusChanged.ContainsKey(p))
            {
                __ClanMemberStatusChanged[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMemberStatusChanged event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMemberStatusChanged</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMemberStatusChanged" />
        protected virtual void OnClanMemberStatusChanged(ClanMemberStatusEventArgs e)
        {
            __InvokeClanMemberStatusChanged(Priority.High, e);
        }

        private void __InvokeClanMemberStatusChanged(Priority p, ClanMemberStatusEventArgs e)
        {
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberStatusChanged[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMemberStatusChanged"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberStatusChanged) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberStatusChanged) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanMemberQuit event
        [NonSerialized]
        private Dictionary<Priority, List<ClanMemberStatusEventHandler>> __ClanMemberQuit = new Dictionary<Priority, List<ClanMemberStatusEventHandler>>(3)
        {
            { Priority.High, new List<ClanMemberStatusEventHandler>() },
            { Priority.Normal, new List<ClanMemberStatusEventHandler>() },
            { Priority.Low, new List<ClanMemberStatusEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a clan member has quit the clan.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMemberQuitNotification</see> and
        /// <see>UnregisterClanMemberQuitNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanMemberStatusEventHandler ClanMemberQuit
        {
            add
            {
                lock (__ClanMemberQuit)
                {
                    if (!__ClanMemberQuit.ContainsKey(Priority.Normal))
                    {
                        __ClanMemberQuit.Add(Priority.Normal, new List<ClanMemberStatusEventHandler>());
                    }
                }
                __ClanMemberQuit[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMemberQuit.ContainsKey(Priority.Normal))
                {
                    __ClanMemberQuit[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMemberQuit</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMemberQuit" />
        /// <seealso cref="UnregisterClanMemberQuitNotification" />
        public void RegisterClanMemberQuitNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            lock (__ClanMemberQuit)
            {
                if (!__ClanMemberQuit.ContainsKey(p))
                {
                    __ClanMemberQuit.Add(p, new List<ClanMemberStatusEventHandler>());
                }
            }
            __ClanMemberQuit[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMemberQuit</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMemberQuit" />
        /// <seealso cref="RegisterClanMemberQuitNotification" />
        public void UnregisterClanMemberQuitNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            if (__ClanMemberQuit.ContainsKey(p))
            {
                __ClanMemberQuit[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMemberQuit event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMemberQuit</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMemberQuit" />
        protected virtual void OnClanMemberQuit(ClanMemberStatusEventArgs e)
        {
            __InvokeClanMemberQuit(Priority.High, e);
        }

        private void __InvokeClanMemberQuit(Priority p, ClanMemberStatusEventArgs e)
        {
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberQuit[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMemberQuit"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberQuit) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberQuit) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanMemberRemoved event
        [NonSerialized]
        private Dictionary<Priority, List<ClanMemberStatusEventHandler>> __ClanMemberRemoved = new Dictionary<Priority, List<ClanMemberStatusEventHandler>>(3)
        {
            { Priority.High, new List<ClanMemberStatusEventHandler>() },
            { Priority.Normal, new List<ClanMemberStatusEventHandler>() },
            { Priority.Low, new List<ClanMemberStatusEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a clan member has been removed from the clan.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMemberRemovedNotification</see> and
        /// <see>UnregisterClanMemberRemovedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanMemberStatusEventHandler ClanMemberRemoved
        {
            add
            {
                lock (__ClanMemberRemoved)
                {
                    if (!__ClanMemberRemoved.ContainsKey(Priority.Normal))
                    {
                        __ClanMemberRemoved.Add(Priority.Normal, new List<ClanMemberStatusEventHandler>());
                    }
                }
                __ClanMemberRemoved[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMemberRemoved.ContainsKey(Priority.Normal))
                {
                    __ClanMemberRemoved[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMemberRemoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMemberRemoved" />
        /// <seealso cref="UnregisterClanMemberRemovedNotification" />
        public void RegisterClanMemberRemovedNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            lock (__ClanMemberRemoved)
            {
                if (!__ClanMemberRemoved.ContainsKey(p))
                {
                    __ClanMemberRemoved.Add(p, new List<ClanMemberStatusEventHandler>());
                }
            }
            __ClanMemberRemoved[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMemberRemoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMemberRemoved" />
        /// <seealso cref="RegisterClanMemberRemovedNotification" />
        public void UnregisterClanMemberRemovedNotification(Priority p, ClanMemberStatusEventHandler callback)
        {
            if (__ClanMemberRemoved.ContainsKey(p))
            {
                __ClanMemberRemoved[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMemberRemoved event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMemberRemoved</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMemberRemoved" />
        protected virtual void OnClanMemberRemoved(ClanMemberStatusEventArgs e)
        {
            __InvokeClanMemberRemoved(Priority.High, e);
        }

        private void __InvokeClanMemberRemoved(Priority p, ClanMemberStatusEventArgs e)
        {
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberRemoved[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMemberRemoved"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberRemoved) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanMemberStatusEventArgs> { Arguments = e, Target = new Invokee<ClanMemberStatusEventArgs>(__InvokeClanMemberRemoved) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanMessageOfTheDay event
        [NonSerialized]
        private Dictionary<Priority, List<InformationEventHandler>> __ClanMessageOfTheDay = new Dictionary<Priority, List<InformationEventHandler>>(3)
        {
            { Priority.High, new List<InformationEventHandler>() },
            { Priority.Normal, new List<InformationEventHandler>() },
            { Priority.Low, new List<InformationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners of the clan's message-of-the-day.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMessageOfTheDayNotification</see> and
        /// <see>UnregisterClanMessageOfTheDayNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event InformationEventHandler ClanMessageOfTheDay
        {
            add
            {
                lock (__ClanMessageOfTheDay)
                {
                    if (!__ClanMessageOfTheDay.ContainsKey(Priority.Normal))
                    {
                        __ClanMessageOfTheDay.Add(Priority.Normal, new List<InformationEventHandler>());
                    }
                }
                __ClanMessageOfTheDay[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMessageOfTheDay.ContainsKey(Priority.Normal))
                {
                    __ClanMessageOfTheDay[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMessageOfTheDay</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMessageOfTheDay" />
        /// <seealso cref="UnregisterClanMessageOfTheDayNotification" />
        public void RegisterClanMessageOfTheDayNotification(Priority p, InformationEventHandler callback)
        {
            lock (__ClanMessageOfTheDay)
            {
                if (!__ClanMessageOfTheDay.ContainsKey(p))
                {
                    __ClanMessageOfTheDay.Add(p, new List<InformationEventHandler>());
                }
            }
            __ClanMessageOfTheDay[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMessageOfTheDay</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMessageOfTheDay" />
        /// <seealso cref="RegisterClanMessageOfTheDayNotification" />
        public void UnregisterClanMessageOfTheDayNotification(Priority p, InformationEventHandler callback)
        {
            if (__ClanMessageOfTheDay.ContainsKey(p))
            {
                __ClanMessageOfTheDay[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMessageOfTheDay event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMessageOfTheDay</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMessageOfTheDay" />
        protected virtual void OnClanMessageOfTheDay(InformationEventArgs e)
        {
            __InvokeClanMessageOfTheDay(Priority.High, e);
        }

        private void __InvokeClanMessageOfTheDay(Priority p, InformationEventArgs e)
        {
            foreach (InformationEventHandler eh in __ClanMessageOfTheDay[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMessageOfTheDay"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeClanMessageOfTheDay) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<InformationEventArgs> { Arguments = e, Target = new Invokee<InformationEventArgs>(__InvokeClanMessageOfTheDay) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanRankChanged event
        [NonSerialized]
        private Dictionary<Priority, List<ClanRankChangeEventHandler>> __ClanRankChanged = new Dictionary<Priority, List<ClanRankChangeEventHandler>>(3)
        {
            { Priority.High, new List<ClanRankChangeEventHandler>() },
            { Priority.Normal, new List<ClanRankChangeEventHandler>() },
            { Priority.Low, new List<ClanRankChangeEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client's user's clan rank has changed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanRankChangedNotification</see> and
        /// <see>UnregisterClanRankChangedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanRankChangeEventHandler ClanRankChanged
        {
            add
            {
                lock (__ClanRankChanged)
                {
                    if (!__ClanRankChanged.ContainsKey(Priority.Normal))
                    {
                        __ClanRankChanged.Add(Priority.Normal, new List<ClanRankChangeEventHandler>());
                    }
                }
                __ClanRankChanged[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanRankChanged.ContainsKey(Priority.Normal))
                {
                    __ClanRankChanged[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanRankChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanRankChanged" />
        /// <seealso cref="UnregisterClanRankChangedNotification" />
        public void RegisterClanRankChangedNotification(Priority p, ClanRankChangeEventHandler callback)
        {
            lock (__ClanRankChanged)
            {
                if (!__ClanRankChanged.ContainsKey(p))
                {
                    __ClanRankChanged.Add(p, new List<ClanRankChangeEventHandler>());
                }
            }
            __ClanRankChanged[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanRankChanged</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanRankChanged" />
        /// <seealso cref="RegisterClanRankChangedNotification" />
        public void UnregisterClanRankChangedNotification(Priority p, ClanRankChangeEventHandler callback)
        {
            if (__ClanRankChanged.ContainsKey(p))
            {
                __ClanRankChanged[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanRankChanged event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanRankChanged</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanRankChanged" />
        protected virtual void OnClanRankChanged(ClanRankChangeEventArgs e)
        {
            __InvokeClanRankChanged(Priority.High, e);
        }

        private void __InvokeClanRankChanged(Priority p, ClanRankChangeEventArgs e)
        {
            foreach (ClanRankChangeEventHandler eh in __ClanRankChanged[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanRankChanged"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanRankChangeEventArgs> { Arguments = e, Target = new Invokee<ClanRankChangeEventArgs>(__InvokeClanRankChanged) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanRankChangeEventArgs> { Arguments = e, Target = new Invokee<ClanRankChangeEventArgs>(__InvokeClanRankChanged) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanMembershipReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ClanMembershipEventHandler>> __ClanMembershipReceived = new Dictionary<Priority, List<ClanMembershipEventHandler>>(3)
        {
            { Priority.High, new List<ClanMembershipEventHandler>() },
            { Priority.Normal, new List<ClanMembershipEventHandler>() },
            { Priority.Low, new List<ClanMembershipEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client's user belongs to a clan.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanMembershipReceivedNotification</see> and
        /// <see>UnregisterClanMembershipReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanMembershipEventHandler ClanMembershipReceived
        {
            add
            {
                lock (__ClanMembershipReceived)
                {
                    if (!__ClanMembershipReceived.ContainsKey(Priority.Normal))
                    {
                        __ClanMembershipReceived.Add(Priority.Normal, new List<ClanMembershipEventHandler>());
                    }
                }
                __ClanMembershipReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanMembershipReceived.ContainsKey(Priority.Normal))
                {
                    __ClanMembershipReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanMembershipReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanMembershipReceived" />
        /// <seealso cref="UnregisterClanMembershipReceivedNotification" />
        public void RegisterClanMembershipReceivedNotification(Priority p, ClanMembershipEventHandler callback)
        {
            lock (__ClanMembershipReceived)
            {
                if (!__ClanMembershipReceived.ContainsKey(p))
                {
                    __ClanMembershipReceived.Add(p, new List<ClanMembershipEventHandler>());
                }
            }
            __ClanMembershipReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanMembershipReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanMembershipReceived" />
        /// <seealso cref="RegisterClanMembershipReceivedNotification" />
        public void UnregisterClanMembershipReceivedNotification(Priority p, ClanMembershipEventHandler callback)
        {
            if (__ClanMembershipReceived.ContainsKey(p))
            {
                __ClanMembershipReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanMembershipReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanMembershipReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanMembershipReceived" />
        protected virtual void OnClanMembershipReceived(ClanMembershipEventArgs e)
        {
            __InvokeClanMembershipReceived(Priority.High, e);
        }

        private void __InvokeClanMembershipReceived(Priority p, ClanMembershipEventArgs e)
        {
            foreach (ClanMembershipEventHandler eh in __ClanMembershipReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanMembershipReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanMembershipEventArgs> { Arguments = e, Target = new Invokee<ClanMembershipEventArgs>(__InvokeClanMembershipReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanMembershipEventArgs> { Arguments = e, Target = new Invokee<ClanMembershipEventArgs>(__InvokeClanMembershipReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanCandidatesSearchCompleted event
        [NonSerialized]
        private Dictionary<Priority, List<ClanCandidatesSearchEventHandler>> __ClanCandidatesSearchCompleted = new Dictionary<Priority, List<ClanCandidatesSearchEventHandler>>(3)
        {
            { Priority.High, new List<ClanCandidatesSearchEventHandler>() },
            { Priority.Normal, new List<ClanCandidatesSearchEventHandler>() },
            { Priority.Low, new List<ClanCandidatesSearchEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a clan candidates search has completed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanCandidatesSearchCompletedNotification</see> and
        /// <see>UnregisterClanCandidatesSearchCompletedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanCandidatesSearchEventHandler ClanCandidatesSearchCompleted
        {
            add
            {
                lock (__ClanCandidatesSearchCompleted)
                {
                    if (!__ClanCandidatesSearchCompleted.ContainsKey(Priority.Normal))
                    {
                        __ClanCandidatesSearchCompleted.Add(Priority.Normal, new List<ClanCandidatesSearchEventHandler>());
                    }
                }
                __ClanCandidatesSearchCompleted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanCandidatesSearchCompleted.ContainsKey(Priority.Normal))
                {
                    __ClanCandidatesSearchCompleted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanCandidatesSearchCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanCandidatesSearchCompleted" />
        /// <seealso cref="UnregisterClanCandidatesSearchCompletedNotification" />
        public void RegisterClanCandidatesSearchCompletedNotification(Priority p, ClanCandidatesSearchEventHandler callback)
        {
            lock (__ClanCandidatesSearchCompleted)
            {
                if (!__ClanCandidatesSearchCompleted.ContainsKey(p))
                {
                    __ClanCandidatesSearchCompleted.Add(p, new List<ClanCandidatesSearchEventHandler>());
                }
            }
            __ClanCandidatesSearchCompleted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanCandidatesSearchCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanCandidatesSearchCompleted" />
        /// <seealso cref="RegisterClanCandidatesSearchCompletedNotification" />
        public void UnregisterClanCandidatesSearchCompletedNotification(Priority p, ClanCandidatesSearchEventHandler callback)
        {
            if (__ClanCandidatesSearchCompleted.ContainsKey(p))
            {
                __ClanCandidatesSearchCompleted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanCandidatesSearchCompleted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanCandidatesSearchCompleted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanCandidatesSearchCompleted" />
        protected virtual void OnClanCandidatesSearchCompleted(ClanCandidatesSearchEventArgs e)
        {
            __InvokeClanCandidatesSearchCompleted(Priority.High, e);
        }

        private void __InvokeClanCandidatesSearchCompleted(Priority p, ClanCandidatesSearchEventArgs e)
        {
            foreach (ClanCandidatesSearchEventHandler eh in __ClanCandidatesSearchCompleted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanCandidatesSearchCompleted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanCandidatesSearchEventArgs> { Arguments = e, Target = new Invokee<ClanCandidatesSearchEventArgs>(__InvokeClanCandidatesSearchCompleted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanCandidatesSearchEventArgs> { Arguments = e, Target = new Invokee<ClanCandidatesSearchEventArgs>(__InvokeClanCandidatesSearchCompleted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanFormationCompleted event
        [NonSerialized]
        private Dictionary<Priority, List<ClanFormationEventHandler>> __ClanFormationCompleted = new Dictionary<Priority, List<ClanFormationEventHandler>>(3)
        {
            { Priority.High, new List<ClanFormationEventHandler>() },
            { Priority.Normal, new List<ClanFormationEventHandler>() },
            { Priority.Low, new List<ClanFormationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that an attempt to form a clan has completed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanFormationCompletedNotification</see> and
        /// <see>UnregisterClanFormationCompletedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanFormationEventHandler ClanFormationCompleted
        {
            add
            {
                lock (__ClanFormationCompleted)
                {
                    if (!__ClanFormationCompleted.ContainsKey(Priority.Normal))
                    {
                        __ClanFormationCompleted.Add(Priority.Normal, new List<ClanFormationEventHandler>());
                    }
                }
                __ClanFormationCompleted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanFormationCompleted.ContainsKey(Priority.Normal))
                {
                    __ClanFormationCompleted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanFormationCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanFormationCompleted" />
        /// <seealso cref="UnregisterClanFormationCompletedNotification" />
        public void RegisterClanFormationCompletedNotification(Priority p, ClanFormationEventHandler callback)
        {
            lock (__ClanFormationCompleted)
            {
                if (!__ClanFormationCompleted.ContainsKey(p))
                {
                    __ClanFormationCompleted.Add(p, new List<ClanFormationEventHandler>());
                }
            }
            __ClanFormationCompleted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanFormationCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanFormationCompleted" />
        /// <seealso cref="RegisterClanFormationCompletedNotification" />
        public void UnregisterClanFormationCompletedNotification(Priority p, ClanFormationEventHandler callback)
        {
            if (__ClanFormationCompleted.ContainsKey(p))
            {
                __ClanFormationCompleted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanFormationCompleted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanFormationCompleted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanFormationCompleted" />
        protected virtual void OnClanFormationCompleted(ClanFormationEventArgs e)
        {
            __InvokeClanFormationCompleted(Priority.High, e);
        }

        private void __InvokeClanFormationCompleted(Priority p, ClanFormationEventArgs e)
        {
            foreach (ClanFormationEventHandler eh in __ClanFormationCompleted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanFormationCompleted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanFormationEventArgs> { Arguments = e, Target = new Invokee<ClanFormationEventArgs>(__InvokeClanFormationCompleted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanFormationEventArgs> { Arguments = e, Target = new Invokee<ClanFormationEventArgs>(__InvokeClanFormationCompleted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanFormationInvitationReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ClanFormationInvitationEventHandler>> __ClanFormationInvitationReceived = new Dictionary<Priority, List<ClanFormationInvitationEventHandler>>(3)
        {
            { Priority.High, new List<ClanFormationInvitationEventHandler>() },
            { Priority.Normal, new List<ClanFormationInvitationEventHandler>() },
            { Priority.Low, new List<ClanFormationInvitationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has been invited to join a clan as it is forming.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanFormationInvitationReceivedNotification</see> and
        /// <see>UnregisterClanFormationInvitationReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanFormationInvitationEventHandler ClanFormationInvitationReceived
        {
            add
            {
                lock (__ClanFormationInvitationReceived)
                {
                    if (!__ClanFormationInvitationReceived.ContainsKey(Priority.Normal))
                    {
                        __ClanFormationInvitationReceived.Add(Priority.Normal, new List<ClanFormationInvitationEventHandler>());
                    }
                }
                __ClanFormationInvitationReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanFormationInvitationReceived.ContainsKey(Priority.Normal))
                {
                    __ClanFormationInvitationReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanFormationInvitationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanFormationInvitationReceived" />
        /// <seealso cref="UnregisterClanFormationInvitationReceivedNotification" />
        public void RegisterClanFormationInvitationReceivedNotification(Priority p, ClanFormationInvitationEventHandler callback)
        {
            lock (__ClanFormationInvitationReceived)
            {
                if (!__ClanFormationInvitationReceived.ContainsKey(p))
                {
                    __ClanFormationInvitationReceived.Add(p, new List<ClanFormationInvitationEventHandler>());
                }
            }
            __ClanFormationInvitationReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanFormationInvitationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanFormationInvitationReceived" />
        /// <seealso cref="RegisterClanFormationInvitationReceivedNotification" />
        public void UnregisterClanFormationInvitationReceivedNotification(Priority p, ClanFormationInvitationEventHandler callback)
        {
            if (__ClanFormationInvitationReceived.ContainsKey(p))
            {
                __ClanFormationInvitationReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanFormationInvitationReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanFormationInvitationReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanFormationInvitationReceived" />
        protected virtual void OnClanFormationInvitationReceived(ClanFormationInvitationEventArgs e)
        {
            __InvokeClanFormationInvitationReceived(Priority.High, e);
        }

        private void __InvokeClanFormationInvitationReceived(Priority p, ClanFormationInvitationEventArgs e)
        {
            foreach (ClanFormationInvitationEventHandler eh in __ClanFormationInvitationReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanFormationInvitationReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanFormationInvitationEventArgs> { Arguments = e, Target = new Invokee<ClanFormationInvitationEventArgs>(__InvokeClanFormationInvitationReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanFormationInvitationEventArgs> { Arguments = e, Target = new Invokee<ClanFormationInvitationEventArgs>(__InvokeClanFormationInvitationReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanDisbandCompleted event
        [NonSerialized]
        private Dictionary<Priority, List<ClanDisbandEventHandler>> __ClanDisbandCompleted = new Dictionary<Priority, List<ClanDisbandEventHandler>>(3)
        {
            { Priority.High, new List<ClanDisbandEventHandler>() },
            { Priority.Normal, new List<ClanDisbandEventHandler>() },
            { Priority.Low, new List<ClanDisbandEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that an attempt to disband the clan has been completed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanDisbandCompletedNotification</see> and
        /// <see>UnregisterClanDisbandCompletedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanDisbandEventHandler ClanDisbandCompleted
        {
            add
            {
                lock (__ClanDisbandCompleted)
                {
                    if (!__ClanDisbandCompleted.ContainsKey(Priority.Normal))
                    {
                        __ClanDisbandCompleted.Add(Priority.Normal, new List<ClanDisbandEventHandler>());
                    }
                }
                __ClanDisbandCompleted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanDisbandCompleted.ContainsKey(Priority.Normal))
                {
                    __ClanDisbandCompleted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanDisbandCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanDisbandCompleted" />
        /// <seealso cref="UnregisterClanDisbandCompletedNotification" />
        public void RegisterClanDisbandCompletedNotification(Priority p, ClanDisbandEventHandler callback)
        {
            lock (__ClanDisbandCompleted)
            {
                if (!__ClanDisbandCompleted.ContainsKey(p))
                {
                    __ClanDisbandCompleted.Add(p, new List<ClanDisbandEventHandler>());
                }
            }
            __ClanDisbandCompleted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanDisbandCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanDisbandCompleted" />
        /// <seealso cref="RegisterClanDisbandCompletedNotification" />
        public void UnregisterClanDisbandCompletedNotification(Priority p, ClanDisbandEventHandler callback)
        {
            if (__ClanDisbandCompleted.ContainsKey(p))
            {
                __ClanDisbandCompleted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanDisbandCompleted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanDisbandCompleted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanDisbandCompleted" />
        protected virtual void OnClanDisbandCompleted(ClanDisbandEventArgs e)
        {
            __InvokeClanDisbandCompleted(Priority.High, e);
        }

        private void __InvokeClanDisbandCompleted(Priority p, ClanDisbandEventArgs e)
        {
            foreach (ClanDisbandEventHandler eh in __ClanDisbandCompleted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanDisbandCompleted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanDisbandEventArgs> { Arguments = e, Target = new Invokee<ClanDisbandEventArgs>(__InvokeClanDisbandCompleted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanDisbandEventArgs> { Arguments = e, Target = new Invokee<ClanDisbandEventArgs>(__InvokeClanDisbandCompleted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanChangeChieftanCompleted event
        [NonSerialized]
        private Dictionary<Priority, List<ClanChieftanChangeEventHandler>> __ClanChangeChieftanCompleted = new Dictionary<Priority, List<ClanChieftanChangeEventHandler>>(3)
        {
            { Priority.High, new List<ClanChieftanChangeEventHandler>() },
            { Priority.Normal, new List<ClanChieftanChangeEventHandler>() },
            { Priority.Low, new List<ClanChieftanChangeEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a request to change the clan chieftan has completed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanChangeChieftanCompletedNotification</see> and
        /// <see>UnregisterClanChangeChieftanCompletedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanChieftanChangeEventHandler ClanChangeChieftanCompleted
        {
            add
            {
                lock (__ClanChangeChieftanCompleted)
                {
                    if (!__ClanChangeChieftanCompleted.ContainsKey(Priority.Normal))
                    {
                        __ClanChangeChieftanCompleted.Add(Priority.Normal, new List<ClanChieftanChangeEventHandler>());
                    }
                }
                __ClanChangeChieftanCompleted[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanChangeChieftanCompleted.ContainsKey(Priority.Normal))
                {
                    __ClanChangeChieftanCompleted[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanChangeChieftanCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanChangeChieftanCompleted" />
        /// <seealso cref="UnregisterClanChangeChieftanCompletedNotification" />
        public void RegisterClanChangeChieftanCompletedNotification(Priority p, ClanChieftanChangeEventHandler callback)
        {
            lock (__ClanChangeChieftanCompleted)
            {
                if (!__ClanChangeChieftanCompleted.ContainsKey(p))
                {
                    __ClanChangeChieftanCompleted.Add(p, new List<ClanChieftanChangeEventHandler>());
                }
            }
            __ClanChangeChieftanCompleted[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanChangeChieftanCompleted</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanChangeChieftanCompleted" />
        /// <seealso cref="RegisterClanChangeChieftanCompletedNotification" />
        public void UnregisterClanChangeChieftanCompletedNotification(Priority p, ClanChieftanChangeEventHandler callback)
        {
            if (__ClanChangeChieftanCompleted.ContainsKey(p))
            {
                __ClanChangeChieftanCompleted[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanChangeChieftanCompleted event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanChangeChieftanCompleted</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanChangeChieftanCompleted" />
        protected virtual void OnClanChangeChieftanCompleted(ClanChieftanChangeEventArgs e)
        {
            __InvokeClanChangeChieftanCompleted(Priority.High, e);
        }

        private void __InvokeClanChangeChieftanCompleted(Priority p, ClanChieftanChangeEventArgs e)
        {
            foreach (ClanChieftanChangeEventHandler eh in __ClanChangeChieftanCompleted[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanChangeChieftanCompleted"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanChieftanChangeEventArgs> { Arguments = e, Target = new Invokee<ClanChieftanChangeEventArgs>(__InvokeClanChangeChieftanCompleted) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanChieftanChangeEventArgs> { Arguments = e, Target = new Invokee<ClanChieftanChangeEventArgs>(__InvokeClanChangeChieftanCompleted) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanInvitationReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ClanInvitationEventHandler>> __ClanInvitationReceived = new Dictionary<Priority, List<ClanInvitationEventHandler>>(3)
        {
            { Priority.High, new List<ClanInvitationEventHandler>() },
            { Priority.Normal, new List<ClanInvitationEventHandler>() },
            { Priority.Low, new List<ClanInvitationEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has received an invitation to join an existing clan.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanInvitationReceivedNotification</see> and
        /// <see>UnregisterClanInvitationReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanInvitationEventHandler ClanInvitationReceived
        {
            add
            {
                lock (__ClanInvitationReceived)
                {
                    if (!__ClanInvitationReceived.ContainsKey(Priority.Normal))
                    {
                        __ClanInvitationReceived.Add(Priority.Normal, new List<ClanInvitationEventHandler>());
                    }
                }
                __ClanInvitationReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanInvitationReceived.ContainsKey(Priority.Normal))
                {
                    __ClanInvitationReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanInvitationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanInvitationReceived" />
        /// <seealso cref="UnregisterClanInvitationReceivedNotification" />
        public void RegisterClanInvitationReceivedNotification(Priority p, ClanInvitationEventHandler callback)
        {
            lock (__ClanInvitationReceived)
            {
                if (!__ClanInvitationReceived.ContainsKey(p))
                {
                    __ClanInvitationReceived.Add(p, new List<ClanInvitationEventHandler>());
                }
            }
            __ClanInvitationReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanInvitationReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanInvitationReceived" />
        /// <seealso cref="RegisterClanInvitationReceivedNotification" />
        public void UnregisterClanInvitationReceivedNotification(Priority p, ClanInvitationEventHandler callback)
        {
            if (__ClanInvitationReceived.ContainsKey(p))
            {
                __ClanInvitationReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanInvitationReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanInvitationReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanInvitationReceived" />
        protected virtual void OnClanInvitationReceived(ClanInvitationEventArgs e)
        {
            __InvokeClanInvitationReceived(Priority.High, e);
        }

        private void __InvokeClanInvitationReceived(Priority p, ClanInvitationEventArgs e)
        {
            foreach (ClanInvitationEventHandler eh in __ClanInvitationReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanInvitationReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanInvitationEventArgs> { Arguments = e, Target = new Invokee<ClanInvitationEventArgs>(__InvokeClanInvitationReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanInvitationEventArgs> { Arguments = e, Target = new Invokee<ClanInvitationEventArgs>(__InvokeClanInvitationReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region LeftClan event
        [NonSerialized]
        private Dictionary<Priority, List<LeftClanEventHandler>> __LeftClan = new Dictionary<Priority, List<LeftClanEventHandler>>(3)
        {
            { Priority.High, new List<LeftClanEventHandler>() },
            { Priority.Normal, new List<LeftClanEventHandler>() },
            { Priority.Low, new List<LeftClanEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client has either left the clan or been forcibly removed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterLeftClanNotification</see> and
        /// <see>UnregisterLeftClanNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event LeftClanEventHandler LeftClan
        {
            add
            {
                lock (__LeftClan)
                {
                    if (!__LeftClan.ContainsKey(Priority.Normal))
                    {
                        __LeftClan.Add(Priority.Normal, new List<LeftClanEventHandler>());
                    }
                }
                __LeftClan[Priority.Normal].Add(value);
            }
            remove
            {
                if (__LeftClan.ContainsKey(Priority.Normal))
                {
                    __LeftClan[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>LeftClan</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="LeftClan" />
        /// <seealso cref="UnregisterLeftClanNotification" />
        public void RegisterLeftClanNotification(Priority p, LeftClanEventHandler callback)
        {
            lock (__LeftClan)
            {
                if (!__LeftClan.ContainsKey(p))
                {
                    __LeftClan.Add(p, new List<LeftClanEventHandler>());
                }
            }
            __LeftClan[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>LeftClan</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="LeftClan" />
        /// <seealso cref="RegisterLeftClanNotification" />
        public void UnregisterLeftClanNotification(Priority p, LeftClanEventHandler callback)
        {
            if (__LeftClan.ContainsKey(p))
            {
                __LeftClan[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the LeftClan event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>LeftClan</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="LeftClan" />
        protected virtual void OnLeftClan(LeftClanEventArgs e)
        {
            __InvokeLeftClan(Priority.High, e);
        }

        private void __InvokeLeftClan(Priority p, LeftClanEventArgs e)
        {
            foreach (LeftClanEventHandler eh in __LeftClan[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "LeftClan"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<LeftClanEventArgs> { Arguments = e, Target = new Invokee<LeftClanEventArgs>(__InvokeLeftClan) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<LeftClanEventArgs> { Arguments = e, Target = new Invokee<LeftClanEventArgs>(__InvokeLeftClan) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanInvitationResponseReceived event
        [NonSerialized]
        private Dictionary<Priority, List<ClanInvitationResponseEventHandler>> __ClanInvitationResponseReceived = new Dictionary<Priority, List<ClanInvitationResponseEventHandler>>(3)
        {
            { Priority.High, new List<ClanInvitationResponseEventHandler>() },
            { Priority.Normal, new List<ClanInvitationResponseEventHandler>() },
            { Priority.Low, new List<ClanInvitationResponseEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that an invitation to join a clan has been responded to and that the response has been received.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanInvitationResponseReceivedNotification</see> and
        /// <see>UnregisterClanInvitationResponseReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanInvitationResponseEventHandler ClanInvitationResponseReceived
        {
            add
            {
                lock (__ClanInvitationResponseReceived)
                {
                    if (!__ClanInvitationResponseReceived.ContainsKey(Priority.Normal))
                    {
                        __ClanInvitationResponseReceived.Add(Priority.Normal, new List<ClanInvitationResponseEventHandler>());
                    }
                }
                __ClanInvitationResponseReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanInvitationResponseReceived.ContainsKey(Priority.Normal))
                {
                    __ClanInvitationResponseReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanInvitationResponseReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanInvitationResponseReceived" />
        /// <seealso cref="UnregisterClanInvitationResponseReceivedNotification" />
        public void RegisterClanInvitationResponseReceivedNotification(Priority p, ClanInvitationResponseEventHandler callback)
        {
            lock (__ClanInvitationResponseReceived)
            {
                if (!__ClanInvitationResponseReceived.ContainsKey(p))
                {
                    __ClanInvitationResponseReceived.Add(p, new List<ClanInvitationResponseEventHandler>());
                }
            }
            __ClanInvitationResponseReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanInvitationResponseReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanInvitationResponseReceived" />
        /// <seealso cref="RegisterClanInvitationResponseReceivedNotification" />
        public void UnregisterClanInvitationResponseReceivedNotification(Priority p, ClanInvitationResponseEventHandler callback)
        {
            if (__ClanInvitationResponseReceived.ContainsKey(p))
            {
                __ClanInvitationResponseReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanInvitationResponseReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanInvitationResponseReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanInvitationResponseReceived" />
        protected virtual void OnClanInvitationResponseReceived(ClanInvitationResponseEventArgs e)
        {
            __InvokeClanInvitationResponseReceived(Priority.High, e);
        }

        private void __InvokeClanInvitationResponseReceived(Priority p, ClanInvitationResponseEventArgs e)
        {
            foreach (ClanInvitationResponseEventHandler eh in __ClanInvitationResponseReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanInvitationResponseReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanInvitationResponseEventArgs> { Arguments = e, Target = new Invokee<ClanInvitationResponseEventArgs>(__InvokeClanInvitationResponseReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanInvitationResponseEventArgs> { Arguments = e, Target = new Invokee<ClanInvitationResponseEventArgs>(__InvokeClanInvitationResponseReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region ClanRemovalResponse event
        [NonSerialized]
        private Dictionary<Priority, List<ClanRemovalResponseEventHandler>> __ClanRemovalResponse = new Dictionary<Priority, List<ClanRemovalResponseEventHandler>>(3)
        {
            { Priority.High, new List<ClanRemovalResponseEventHandler>() },
            { Priority.Normal, new List<ClanRemovalResponseEventHandler>() },
            { Priority.Low, new List<ClanRemovalResponseEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a request to remove a clan member has completed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterClanRemovalResponseNotification</see> and
        /// <see>UnregisterClanRemovalResponseNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event ClanRemovalResponseEventHandler ClanRemovalResponse
        {
            add
            {
                lock (__ClanRemovalResponse)
                {
                    if (!__ClanRemovalResponse.ContainsKey(Priority.Normal))
                    {
                        __ClanRemovalResponse.Add(Priority.Normal, new List<ClanRemovalResponseEventHandler>());
                    }
                }
                __ClanRemovalResponse[Priority.Normal].Add(value);
            }
            remove
            {
                if (__ClanRemovalResponse.ContainsKey(Priority.Normal))
                {
                    __ClanRemovalResponse[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>ClanRemovalResponse</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="ClanRemovalResponse" />
        /// <seealso cref="UnregisterClanRemovalResponseNotification" />
        public void RegisterClanRemovalResponseNotification(Priority p, ClanRemovalResponseEventHandler callback)
        {
            lock (__ClanRemovalResponse)
            {
                if (!__ClanRemovalResponse.ContainsKey(p))
                {
                    __ClanRemovalResponse.Add(p, new List<ClanRemovalResponseEventHandler>());
                }
            }
            __ClanRemovalResponse[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>ClanRemovalResponse</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="ClanRemovalResponse" />
        /// <seealso cref="RegisterClanRemovalResponseNotification" />
        public void UnregisterClanRemovalResponseNotification(Priority p, ClanRemovalResponseEventHandler callback)
        {
            if (__ClanRemovalResponse.ContainsKey(p))
            {
                __ClanRemovalResponse[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the ClanRemovalResponse event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>ClanRemovalResponse</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="ClanRemovalResponse" />
        protected virtual void OnClanRemovalResponse(ClanRemovalResponseEventArgs e)
        {
            __InvokeClanRemovalResponse(Priority.High, e);
        }

        private void __InvokeClanRemovalResponse(Priority p, ClanRemovalResponseEventArgs e)
        {
            foreach (ClanRemovalResponseEventHandler eh in __ClanRemovalResponse[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "ClanRemovalResponse"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<ClanRemovalResponseEventArgs> { Arguments = e, Target = new Invokee<ClanRemovalResponseEventArgs>(__InvokeClanRemovalResponse) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<ClanRemovalResponseEventArgs> { Arguments = e, Target = new Invokee<ClanRemovalResponseEventArgs>(__InvokeClanRemovalResponse) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region friend events
        #region FriendListReceived event
        [NonSerialized]
        private Dictionary<Priority, List<FriendListReceivedEventHandler>> __FriendListReceived = new Dictionary<Priority, List<FriendListReceivedEventHandler>>(3)
        {
            { Priority.High, new List<FriendListReceivedEventHandler>() },
            { Priority.Normal, new List<FriendListReceivedEventHandler>() },
            { Priority.Low, new List<FriendListReceivedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that the client's friend list has been received.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterFriendListReceivedNotification</see> and
        /// <see>UnregisterFriendListReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event FriendListReceivedEventHandler FriendListReceived
        {
            add
            {
                lock (__FriendListReceived)
                {
                    if (!__FriendListReceived.ContainsKey(Priority.Normal))
                    {
                        __FriendListReceived.Add(Priority.Normal, new List<FriendListReceivedEventHandler>());
                    }
                }
                __FriendListReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__FriendListReceived.ContainsKey(Priority.Normal))
                {
                    __FriendListReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>FriendListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="FriendListReceived" />
        /// <seealso cref="UnregisterFriendListReceivedNotification" />
        public void RegisterFriendListReceivedNotification(Priority p, FriendListReceivedEventHandler callback)
        {
            lock (__FriendListReceived)
            {
                if (!__FriendListReceived.ContainsKey(p))
                {
                    __FriendListReceived.Add(p, new List<FriendListReceivedEventHandler>());
                }
            }
            __FriendListReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>FriendListReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="FriendListReceived" />
        /// <seealso cref="RegisterFriendListReceivedNotification" />
        public void UnregisterFriendListReceivedNotification(Priority p, FriendListReceivedEventHandler callback)
        {
            if (__FriendListReceived.ContainsKey(p))
            {
                __FriendListReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the FriendListReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>FriendListReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="FriendListReceived" />
        protected virtual void OnFriendListReceived(FriendListReceivedEventArgs e)
        {
            __InvokeFriendListReceived(Priority.High, e);
        }

        private void __InvokeFriendListReceived(Priority p, FriendListReceivedEventArgs e)
        {
            foreach (FriendListReceivedEventHandler eh in __FriendListReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "FriendListReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<FriendListReceivedEventArgs> { Arguments = e, Target = new Invokee<FriendListReceivedEventArgs>(__InvokeFriendListReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<FriendListReceivedEventArgs> { Arguments = e, Target = new Invokee<FriendListReceivedEventArgs>(__InvokeFriendListReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region FriendUpdated event
        [NonSerialized]
        private Dictionary<Priority, List<FriendUpdatedEventHandler>> __FriendUpdated = new Dictionary<Priority, List<FriendUpdatedEventHandler>>(3)
        {
            { Priority.High, new List<FriendUpdatedEventHandler>() },
            { Priority.Normal, new List<FriendUpdatedEventHandler>() },
            { Priority.Low, new List<FriendUpdatedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a friend on the client's friend list has had its status changed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterFriendUpdatedNotification</see> and
        /// <see>UnregisterFriendUpdatedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event FriendUpdatedEventHandler FriendUpdated
        {
            add
            {
                lock (__FriendUpdated)
                {
                    if (!__FriendUpdated.ContainsKey(Priority.Normal))
                    {
                        __FriendUpdated.Add(Priority.Normal, new List<FriendUpdatedEventHandler>());
                    }
                }
                __FriendUpdated[Priority.Normal].Add(value);
            }
            remove
            {
                if (__FriendUpdated.ContainsKey(Priority.Normal))
                {
                    __FriendUpdated[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>FriendUpdated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="FriendUpdated" />
        /// <seealso cref="UnregisterFriendUpdatedNotification" />
        public void RegisterFriendUpdatedNotification(Priority p, FriendUpdatedEventHandler callback)
        {
            lock (__FriendUpdated)
            {
                if (!__FriendUpdated.ContainsKey(p))
                {
                    __FriendUpdated.Add(p, new List<FriendUpdatedEventHandler>());
                }
            }
            __FriendUpdated[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>FriendUpdated</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="FriendUpdated" />
        /// <seealso cref="RegisterFriendUpdatedNotification" />
        public void UnregisterFriendUpdatedNotification(Priority p, FriendUpdatedEventHandler callback)
        {
            if (__FriendUpdated.ContainsKey(p))
            {
                __FriendUpdated[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the FriendUpdated event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>FriendUpdated</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="FriendUpdated" />
        protected virtual void OnFriendUpdated(FriendUpdatedEventArgs e)
        {
            __InvokeFriendUpdated(Priority.High, e);
        }

        private void __InvokeFriendUpdated(Priority p, FriendUpdatedEventArgs e)
        {
            foreach (FriendUpdatedEventHandler eh in __FriendUpdated[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "FriendUpdated"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<FriendUpdatedEventArgs> { Arguments = e, Target = new Invokee<FriendUpdatedEventArgs>(__InvokeFriendUpdated) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<FriendUpdatedEventArgs> { Arguments = e, Target = new Invokee<FriendUpdatedEventArgs>(__InvokeFriendUpdated) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region FriendAdded event
        [NonSerialized]
        private Dictionary<Priority, List<FriendAddedEventHandler>> __FriendAdded = new Dictionary<Priority, List<FriendAddedEventHandler>>(3)
        {
            { Priority.High, new List<FriendAddedEventHandler>() },
            { Priority.Normal, new List<FriendAddedEventHandler>() },
            { Priority.Low, new List<FriendAddedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a new friend has been added to the client's friends list.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterFriendAddedNotification</see> and
        /// <see>UnregisterFriendAddedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event FriendAddedEventHandler FriendAdded
        {
            add
            {
                lock (__FriendAdded)
                {
                    if (!__FriendAdded.ContainsKey(Priority.Normal))
                    {
                        __FriendAdded.Add(Priority.Normal, new List<FriendAddedEventHandler>());
                    }
                }
                __FriendAdded[Priority.Normal].Add(value);
            }
            remove
            {
                if (__FriendAdded.ContainsKey(Priority.Normal))
                {
                    __FriendAdded[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>FriendAdded</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="FriendAdded" />
        /// <seealso cref="UnregisterFriendAddedNotification" />
        public void RegisterFriendAddedNotification(Priority p, FriendAddedEventHandler callback)
        {
            lock (__FriendAdded)
            {
                if (!__FriendAdded.ContainsKey(p))
                {
                    __FriendAdded.Add(p, new List<FriendAddedEventHandler>());
                }
            }
            __FriendAdded[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>FriendAdded</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="FriendAdded" />
        /// <seealso cref="RegisterFriendAddedNotification" />
        public void UnregisterFriendAddedNotification(Priority p, FriendAddedEventHandler callback)
        {
            if (__FriendAdded.ContainsKey(p))
            {
                __FriendAdded[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the FriendAdded event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>FriendAdded</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="FriendAdded" />
        protected virtual void OnFriendAdded(FriendAddedEventArgs e)
        {
            __InvokeFriendAdded(Priority.High, e);
        }

        private void __InvokeFriendAdded(Priority p, FriendAddedEventArgs e)
        {
            foreach (FriendAddedEventHandler eh in __FriendAdded[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "FriendAdded"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<FriendAddedEventArgs> { Arguments = e, Target = new Invokee<FriendAddedEventArgs>(__InvokeFriendAdded) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<FriendAddedEventArgs> { Arguments = e, Target = new Invokee<FriendAddedEventArgs>(__InvokeFriendAdded) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region FriendRemoved event
        [NonSerialized]
        private Dictionary<Priority, List<FriendRemovedEventHandler>> __FriendRemoved = new Dictionary<Priority, List<FriendRemovedEventHandler>>(3)
        {
            { Priority.High, new List<FriendRemovedEventHandler>() },
            { Priority.Normal, new List<FriendRemovedEventHandler>() },
            { Priority.Low, new List<FriendRemovedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a friend has been removed from the client's friends list.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterFriendRemovedNotification</see> and
        /// <see>UnregisterFriendRemovedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event FriendRemovedEventHandler FriendRemoved
        {
            add
            {
                lock (__FriendRemoved)
                {
                    if (!__FriendRemoved.ContainsKey(Priority.Normal))
                    {
                        __FriendRemoved.Add(Priority.Normal, new List<FriendRemovedEventHandler>());
                    }
                }
                __FriendRemoved[Priority.Normal].Add(value);
            }
            remove
            {
                if (__FriendRemoved.ContainsKey(Priority.Normal))
                {
                    __FriendRemoved[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>FriendRemoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="FriendRemoved" />
        /// <seealso cref="UnregisterFriendRemovedNotification" />
        public void RegisterFriendRemovedNotification(Priority p, FriendRemovedEventHandler callback)
        {
            lock (__FriendRemoved)
            {
                if (!__FriendRemoved.ContainsKey(p))
                {
                    __FriendRemoved.Add(p, new List<FriendRemovedEventHandler>());
                }
            }
            __FriendRemoved[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>FriendRemoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="FriendRemoved" />
        /// <seealso cref="RegisterFriendRemovedNotification" />
        public void UnregisterFriendRemovedNotification(Priority p, FriendRemovedEventHandler callback)
        {
            if (__FriendRemoved.ContainsKey(p))
            {
                __FriendRemoved[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the FriendRemoved event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>FriendRemoved</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="FriendRemoved" />
        protected virtual void OnFriendRemoved(FriendRemovedEventArgs e)
        {
            __InvokeFriendRemoved(Priority.High, e);
        }

        private void __InvokeFriendRemoved(Priority p, FriendRemovedEventArgs e)
        {
            foreach (FriendRemovedEventHandler eh in __FriendRemoved[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "FriendRemoved"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<FriendRemovedEventArgs> { Arguments = e, Target = new Invokee<FriendRemovedEventArgs>(__InvokeFriendRemoved) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<FriendRemovedEventArgs> { Arguments = e, Target = new Invokee<FriendRemovedEventArgs>(__InvokeFriendRemoved) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion

        #region FriendMoved event
        [NonSerialized]
        private Dictionary<Priority, List<FriendMovedEventHandler>> __FriendMoved = new Dictionary<Priority, List<FriendMovedEventHandler>>(3)
        {
            { Priority.High, new List<FriendMovedEventHandler>() },
            { Priority.Normal, new List<FriendMovedEventHandler>() },
            { Priority.Low, new List<FriendMovedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a friend on the user's friends list has changed position on the list.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterFriendMovedNotification</see> and
        /// <see>UnregisterFriendMovedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event FriendMovedEventHandler FriendMoved
        {
            add
            {
                lock (__FriendMoved)
                {
                    if (!__FriendMoved.ContainsKey(Priority.Normal))
                    {
                        __FriendMoved.Add(Priority.Normal, new List<FriendMovedEventHandler>());
                    }
                }
                __FriendMoved[Priority.Normal].Add(value);
            }
            remove
            {
                if (__FriendMoved.ContainsKey(Priority.Normal))
                {
                    __FriendMoved[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>FriendMoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="FriendMoved" />
        /// <seealso cref="UnregisterFriendMovedNotification" />
        public void RegisterFriendMovedNotification(Priority p, FriendMovedEventHandler callback)
        {
            lock (__FriendMoved)
            {
                if (!__FriendMoved.ContainsKey(p))
                {
                    __FriendMoved.Add(p, new List<FriendMovedEventHandler>());
                }
            }
            __FriendMoved[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>FriendMoved</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="FriendMoved" />
        /// <seealso cref="RegisterFriendMovedNotification" />
        public void UnregisterFriendMovedNotification(Priority p, FriendMovedEventHandler callback)
        {
            if (__FriendMoved.ContainsKey(p))
            {
                __FriendMoved[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the FriendMoved event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>FriendMoved</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="FriendMoved" />
        protected virtual void OnFriendMoved(FriendMovedEventArgs e)
        {
            __InvokeFriendMoved(Priority.High, e);
        }

        private void __InvokeFriendMoved(Priority p, FriendMovedEventArgs e)
        {
            foreach (FriendMovedEventHandler eh in __FriendMoved[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "FriendMoved"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<FriendMovedEventArgs> { Arguments = e, Target = new Invokee<FriendMovedEventArgs>(__InvokeFriendMoved) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<FriendMovedEventArgs> { Arguments = e, Target = new Invokee<FriendMovedEventArgs>(__InvokeFriendMoved) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
        #endregion

        #region UserProfileReceived event
        [NonSerialized]
        private Dictionary<Priority, List<UserProfileEventHandler>> __UserProfileReceived = new Dictionary<Priority, List<UserProfileEventHandler>>(3)
        {
            { Priority.High, new List<UserProfileEventHandler>() },
            { Priority.Normal, new List<UserProfileEventHandler>() },
            { Priority.Low, new List<UserProfileEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a user profile has been received.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterUserProfileReceivedNotification</see> and
        /// <see>UnregisterUserProfileReceivedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event UserProfileEventHandler UserProfileReceived
        {
            add
            {
                lock (__UserProfileReceived)
                {
                    if (!__UserProfileReceived.ContainsKey(Priority.Normal))
                    {
                        __UserProfileReceived.Add(Priority.Normal, new List<UserProfileEventHandler>());
                    }
                }
                __UserProfileReceived[Priority.Normal].Add(value);
            }
            remove
            {
                if (__UserProfileReceived.ContainsKey(Priority.Normal))
                {
                    __UserProfileReceived[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>UserProfileReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority at which to register.</param>
        /// <param name="callback">The event handler that should be registered for this event.</param>
        /// <seealso cref="UserProfileReceived" />
        /// <seealso cref="UnregisterUserProfileReceivedNotification" />
        public void RegisterUserProfileReceivedNotification(Priority p, UserProfileEventHandler callback)
        {
            lock (__UserProfileReceived)
            {
                if (!__UserProfileReceived.ContainsKey(p))
                {
                    __UserProfileReceived.Add(p, new List<UserProfileEventHandler>());
                }
            }
            __UserProfileReceived[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>UserProfileReceived</see> event at the specified priority.
        /// </summary>
        /// <remarks>
        /// <para>The event system in the JinxBot API supports normal event registration and prioritized event registration.  You can use
        /// normal syntax to register for events at <see cref="Priority">Normal priority</see>, so no special registration is needed; this is 
        /// accessed through normal event handling syntax (the += syntax in C#, or the <see langword="Handles" lang="VB" /> in Visual Basic.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        ///	<para>To be well-behaved within JinxBot, plugins should always unregister themselves when they are being unloaded or when they 
        /// otherwise need to do so.  Plugins may opt-in to a Reflection-based event handling registration system which uses attributes to 
        /// mark methods that should be used as event handlers.</para>
        /// </remarks>
        /// <param name="p">The priority from which to unregister.</param>
        /// <param name="callback">The event handler that should be unregistered for this event.</param>
        /// <seealso cref="UserProfileReceived" />
        /// <seealso cref="RegisterUserProfileReceivedNotification" />
        public void UnregisterUserProfileReceivedNotification(Priority p, UserProfileEventHandler callback)
        {
            if (__UserProfileReceived.ContainsKey(p))
            {
                __UserProfileReceived[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the UserProfileReceived event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>UserProfileReceived</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="UserProfileReceived" />
        protected virtual void OnUserProfileReceived(UserProfileEventArgs e)
        {
            __InvokeUserProfileReceived(Priority.High, e);
        }

        private void __InvokeUserProfileReceived(Priority p, UserProfileEventArgs e)
        {
            foreach (UserProfileEventHandler eh in __UserProfileReceived[p])
            {
                try
                {
                    eh(this, e);
                }
                catch (Exception ex)
                {
                    ReportException(
                        ex,
                        new KeyValuePair<string, object>("delegate", eh),
                        new KeyValuePair<string, object>("Event", "UserProfileReceived"),
                        new KeyValuePair<string, object>("param: priority", p),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            if (p == Priority.High)
            {
                e_medPriorityEvents.Enqueue(new InvokeHelper<UserProfileEventArgs> { Arguments = e, Target = new Invokee<UserProfileEventArgs>(__InvokeUserProfileReceived) });
                e_medBlocker.Set();
            }
            else if (p == Priority.Normal)
            {
                e_lowPriorityEvents.Enqueue(new InvokeHelper<UserProfileEventArgs> { Arguments = e, Target = new Invokee<UserProfileEventArgs>(__InvokeUserProfileReceived) });
            }
            else // if (p == Priority.Low)
            {
                FreeArgumentResources(e as BaseEventArgs);
            }
        }
        #endregion
		
    }
}
