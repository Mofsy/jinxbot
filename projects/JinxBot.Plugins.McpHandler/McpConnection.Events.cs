using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp;
using System.Threading;

namespace JinxBot.Plugins.McpHandler
{
    partial class McpConnection
    {
        partial void ReportException(Exception ex, params KeyValuePair<string, object>[] parameters);
        partial void FreeArgumentResources(BaseEventArgs e);

        #region RealmsRetrieved event
        [NonSerialized]
        private Dictionary<Priority, List<AvailableRealmsEventHandler>> __RealmsRetrieved = new Dictionary<Priority, List<AvailableRealmsEventHandler>>(3)
        {
            { Priority.High, new List<AvailableRealmsEventHandler>() },
            { Priority.Normal, new List<AvailableRealmsEventHandler>() },
            { Priority.Low, new List<AvailableRealmsEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a list of available realms has been retrieved from Battle.net.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterRealmsRetrievedNotification</see> and
        /// <see>UnregisterRealmsRetrievedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event AvailableRealmsEventHandler RealmsRetrieved
        {
            add
            {
                lock (__RealmsRetrieved)
                {
                    if (!__RealmsRetrieved.ContainsKey(Priority.Normal))
                    {
                        __RealmsRetrieved.Add(Priority.Normal, new List<AvailableRealmsEventHandler>());
                    }
                }
                __RealmsRetrieved[Priority.Normal].Add(value);
            }
            remove
            {
                if (__RealmsRetrieved.ContainsKey(Priority.Normal))
                {
                    __RealmsRetrieved[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>RealmsRetrieved</see> event at the specified priority.
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
        /// <seealso cref="RealmsRetrieved" />
        /// <seealso cref="UnregisterRealmsRetrievedNotification" />
        public void RegisterRealmsRetrievedNotification(Priority p, AvailableRealmsEventHandler callback)
        {
            lock (__RealmsRetrieved)
            {
                if (!__RealmsRetrieved.ContainsKey(p))
                {
                    __RealmsRetrieved.Add(p, new List<AvailableRealmsEventHandler>());
                }
            }
            __RealmsRetrieved[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>RealmsRetrieved</see> event at the specified priority.
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
        /// <seealso cref="RealmsRetrieved" />
        /// <seealso cref="RegisterRealmsRetrievedNotification" />
        public void UnregisterRealmsRetrievedNotification(Priority p, AvailableRealmsEventHandler callback)
        {
            if (__RealmsRetrieved.ContainsKey(p))
            {
                __RealmsRetrieved[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the RealmsRetrieved event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>RealmsRetrieved</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="RealmsRetrieved" />
        protected virtual void OnRealmsRetrieved(AvailableRealmsEventArgs e)
        {
            foreach (AvailableRealmsEventHandler eh in __RealmsRetrieved[Priority.High])
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
                        new KeyValuePair<string, object>("Event", "RealmsRetrieved"),
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (AvailableRealmsEventHandler eh in __RealmsRetrieved[Priority.Normal])
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
                            new KeyValuePair<string, object>("Event", "RealmsRetrieved"),
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (AvailableRealmsEventHandler eh in __RealmsRetrieved[Priority.Low])
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
                                new KeyValuePair<string, object>("Event", "RealmsRetrieved"),
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

        #region RealmConnectionFailed event
        [NonSerialized]
        private Dictionary<Priority, List<RealmFailedEventHandler>> __RealmConnectionFailed = new Dictionary<Priority, List<RealmFailedEventHandler>>(3)
        {
            { Priority.High, new List<RealmFailedEventHandler>() },
            { Priority.Normal, new List<RealmFailedEventHandler>() },
            { Priority.Low, new List<RealmFailedEventHandler>() }
        };
        /// <summary>
        /// Informs listeners that a connection to a realm server has failed.
        /// </summary>
        /// <remarks>
        /// <para>Registering for this event with this member will register with <see cref="Priority">Normal priority</see>.  To register for 
        /// <see cref="Priority">High</see> or <see cref="Priority">Low</see> priority, use the <see>RegisterRealmConnectionFailedNotification</see> and
        /// <see>UnregisterRealmConnectionFailedNotification</see> methods.</para>
        /// <para>Events in the JinxBot API are never guaranteed to be executed on the UI thread.  Events that affect the user interface should
        /// be marshaled back to the UI thread by the event handling code.  Generally, high-priority event handlers are
        /// raised on the thread that is parsing data from Battle.net, and lower-priority event handler are executed from the thread pool.</para>
        /// <para>JinxBot guarantees that all event handlers will be fired regardless of exceptions raised in previous event handlers.  However, 
        /// if a plugin repeatedly raises an exception, it may be forcefully unregistered from events.</para>
        /// </remarks>
        public event RealmFailedEventHandler RealmConnectionFailed
        {
            add
            {
                lock (__RealmConnectionFailed)
                {
                    if (!__RealmConnectionFailed.ContainsKey(Priority.Normal))
                    {
                        __RealmConnectionFailed.Add(Priority.Normal, new List<RealmFailedEventHandler>());
                    }
                }
                __RealmConnectionFailed[Priority.Normal].Add(value);
            }
            remove
            {
                if (__RealmConnectionFailed.ContainsKey(Priority.Normal))
                {
                    __RealmConnectionFailed[Priority.Normal].Remove(value);
                }
            }
        }

        /// <summary>
        /// Registers for notification of the <see>RealmConnectionFailed</see> event at the specified priority.
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
        /// <seealso cref="RealmConnectionFailed" />
        /// <seealso cref="UnregisterRealmConnectionFailedNotification" />
        public void RegisterRealmConnectionFailedNotification(Priority p, RealmFailedEventHandler callback)
        {
            lock (__RealmConnectionFailed)
            {
                if (!__RealmConnectionFailed.ContainsKey(p))
                {
                    __RealmConnectionFailed.Add(p, new List<RealmFailedEventHandler>());
                }
            }
            __RealmConnectionFailed[p].Add(callback);
        }

        /// <summary>
        /// Unregisters for notification of the <see>RealmConnectionFailed</see> event at the specified priority.
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
        /// <seealso cref="RealmConnectionFailed" />
        /// <seealso cref="RegisterRealmConnectionFailedNotification" />
        public void UnregisterRealmConnectionFailedNotification(Priority p, RealmFailedEventHandler callback)
        {
            if (__RealmConnectionFailed.ContainsKey(p))
            {
                __RealmConnectionFailed[p].Remove(callback);
            }
        }

        /// <summary>
        /// Raises the RealmConnectionFailed event.
        /// </summary>
        /// <remarks>
        /// <para>Only high-priority events are invoked immediately; others are deferred.  For more information, see <see>RealmConnectionFailed</see>.</para>
        /// </remarks>
        /// <param name="e">The event arguments.</param>
        /// <seealso cref="RealmConnectionFailed" />
        protected virtual void OnRealmConnectionFailed(RealmFailedEventArgs e)
        {
            foreach (RealmFailedEventHandler eh in __RealmConnectionFailed[Priority.High])
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
                        new KeyValuePair<string, object>("Event", "RealmConnectionFailed"),
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (RealmFailedEventHandler eh in __RealmConnectionFailed[Priority.Normal])
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
                            new KeyValuePair<string, object>("Event", "RealmConnectionFailed"),
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (RealmFailedEventHandler eh in __RealmConnectionFailed[Priority.Low])
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
                                new KeyValuePair<string, object>("Event", "RealmConnectionFailed"),
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
		
    }
}
