using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BNSharp.BattleNet;
using BNSharp.BattleNet.Clans;

namespace BNSharp.Net
{
    partial class BattleNetClient
    {
        partial void ReportException(Exception ex, params KeyValuePair<string, object>[] notes);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        partial void FreeArgumentResources(BaseEventArgs e);

        /* 
         * This part of the class contains the implementation and registration parts of all of the events.  It is implemented
         * rapidly by using code snippets and then using the #region directive to hide the code.
         */

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
            foreach (UserEventHandler eh in __UserJoined[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (UserEventHandler eh in __UserJoined[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (UserEventHandler eh in __UserJoined[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (UserEventHandler eh in __UserLeft[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (UserEventHandler eh in __UserLeft[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (UserEventHandler eh in __UserLeft[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (UserEventHandler eh in __UserShown[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (UserEventHandler eh in __UserShown[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (UserEventHandler eh in __UserShown[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (UserEventHandler eh in __UserFlagsChanged[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (UserEventHandler eh in __UserFlagsChanged[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (UserEventHandler eh in __UserFlagsChanged[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __ServerBroadcast[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __ServerBroadcast[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __ServerBroadcast[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __JoinedChannel[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __JoinedChannel[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __JoinedChannel[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __ChannelWasFull[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __ChannelWasFull[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __ChannelWasFull[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __ChannelDidNotExist[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __ChannelDidNotExist[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __ChannelDidNotExist[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __ChannelWasRestricted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __ChannelWasRestricted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __ChannelWasRestricted[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __InformationReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __InformationReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __InformationReceived[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ServerChatEventHandler eh in __ServerErrorReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerChatEventHandler eh in __ServerErrorReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerChatEventHandler eh in __ServerErrorReceived[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ChatMessageEventHandler eh in __WhisperSent[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChatMessageEventHandler eh in __WhisperSent[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChatMessageEventHandler eh in __WhisperSent[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ChatMessageEventHandler eh in __WhisperReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChatMessageEventHandler eh in __WhisperReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChatMessageEventHandler eh in __WhisperReceived[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ChatMessageEventHandler eh in __UserSpoke[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChatMessageEventHandler eh in __UserSpoke[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChatMessageEventHandler eh in __UserSpoke[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ChatMessageEventHandler eh in __UserEmoted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChatMessageEventHandler eh in __UserEmoted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChatMessageEventHandler eh in __UserEmoted[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ChatMessageEventHandler eh in __MessageSent[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChatMessageEventHandler eh in __MessageSent[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChatMessageEventHandler eh in __MessageSent[Priority.Low])
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
            foreach (InformationEventHandler eh in __CommandSent[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (InformationEventHandler eh in __CommandSent[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (InformationEventHandler eh in __CommandSent[Priority.Low])
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
            foreach (EventHandler eh in __ClientCheckPassed[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __ClientCheckPassed[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __ClientCheckPassed[Priority.Low])
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
                                new KeyValuePair<string, object>("param: priority", Priority.Low),
                                new KeyValuePair<string, object>("param: this", this),
                                new KeyValuePair<string, object>("param: e", e)
                                );
                        }
                    }
                    FreeArgumentResources(e);
                });
            });
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
            foreach (ClientCheckFailedEventHandler eh in __ClientCheckFailed[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClientCheckFailedEventHandler eh in __ClientCheckFailed[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClientCheckFailedEventHandler eh in __ClientCheckFailed[Priority.Low])
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
            foreach (EventHandler eh in __LoginSucceeded[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __LoginSucceeded[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __LoginSucceeded[Priority.Low])
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

        #region LoginFailed event
        [NonSerialized]
        private Dictionary<Priority, List<EventHandler>> __LoginFailed = new Dictionary<Priority, List<EventHandler>>(3)
        {
            { Priority.High, new List<EventHandler>() },
            { Priority.Normal, new List<EventHandler>() },
            { Priority.Low, new List<EventHandler>() }
        };
        /// <summary>
        /// Informs listeners that client login failed.
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
        public event EventHandler LoginFailed
        {
            add
            {
                lock (__LoginFailed)
                {
                    if (!__LoginFailed.ContainsKey(Priority.Normal))
                    {
                        __LoginFailed.Add(Priority.Normal, new List<EventHandler>());
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
        public void RegisterLoginFailedNotification(Priority p, EventHandler callback)
        {
            lock (__LoginFailed)
            {
                if (!__LoginFailed.ContainsKey(p))
                {
                    __LoginFailed.Add(p, new List<EventHandler>());
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
        public void UnregisterLoginFailedNotification(Priority p, EventHandler callback)
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
        protected virtual void OnLoginFailed(EventArgs e)
        {
            foreach (EventHandler eh in __LoginFailed[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __LoginFailed[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __LoginFailed[Priority.Low])
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
            foreach (EnteredChatEventHandler eh in __EnteredChat[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EnteredChatEventHandler eh in __EnteredChat[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EnteredChatEventHandler eh in __EnteredChat[Priority.Low])
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
            foreach (ChannelListEventHandler eh in __ChannelListReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ChannelListEventHandler eh in __ChannelListReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ChannelListEventHandler eh in __ChannelListReceived[Priority.Low])
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
            foreach (ErrorEventHandler eh in __Error[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ErrorEventHandler eh in __Error[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ErrorEventHandler eh in __Error[Priority.Low])
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
            foreach (InformationEventHandler eh in __Information[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (InformationEventHandler eh in __Information[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (InformationEventHandler eh in __Information[Priority.Low])
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
            foreach (EventHandler eh in __Connected[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __Connected[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }

                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __Connected[Priority.Low])
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
            foreach (EventHandler eh in __Disconnected[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (EventHandler eh in __Disconnected[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (EventHandler eh in __Disconnected[Priority.Low])
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
            foreach (ServerNewsEventHandler eh in __ServerNews[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ServerNewsEventHandler eh in __ServerNews[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ServerNewsEventHandler eh in __ServerNews[Priority.Low])
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
            foreach (ClanMemberListEventHandler eh in __ClanMemberListReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanMemberListEventHandler eh in __ClanMemberListReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanMemberListEventHandler eh in __ClanMemberListReceived[Priority.Low])
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
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberStatusChanged[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanMemberStatusEventHandler eh in __ClanMemberStatusChanged[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanMemberStatusEventHandler eh in __ClanMemberStatusChanged[Priority.Low])
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
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberQuit[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanMemberStatusEventHandler eh in __ClanMemberQuit[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanMemberStatusEventHandler eh in __ClanMemberQuit[Priority.Low])
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
            foreach (ClanMemberStatusEventHandler eh in __ClanMemberRemoved[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanMemberStatusEventHandler eh in __ClanMemberRemoved[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanMemberStatusEventHandler eh in __ClanMemberRemoved[Priority.Low])
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
            foreach (InformationEventHandler eh in __ClanMessageOfTheDay[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (InformationEventHandler eh in __ClanMessageOfTheDay[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (InformationEventHandler eh in __ClanMessageOfTheDay[Priority.Low])
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
            foreach (ClanRankChangeEventHandler eh in __ClanRankChanged[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanRankChangeEventHandler eh in __ClanRankChanged[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanRankChangeEventHandler eh in __ClanRankChanged[Priority.Low])
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
            foreach (ClanMembershipEventHandler eh in __ClanMembershipReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanMembershipEventHandler eh in __ClanMembershipReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanMembershipEventHandler eh in __ClanMembershipReceived[Priority.Low])
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
            foreach (ClanCandidatesSearchEventHandler eh in __ClanCandidatesSearchCompleted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanCandidatesSearchEventHandler eh in __ClanCandidatesSearchCompleted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanCandidatesSearchEventHandler eh in __ClanCandidatesSearchCompleted[Priority.Low])
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
            foreach (ClanFormationEventHandler eh in __ClanFormationCompleted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanFormationEventHandler eh in __ClanFormationCompleted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanFormationEventHandler eh in __ClanFormationCompleted[Priority.Low])
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
            foreach (ClanFormationInvitationEventHandler eh in __ClanFormationInvitationReceived[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanFormationInvitationEventHandler eh in __ClanFormationInvitationReceived[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanFormationInvitationEventHandler eh in __ClanFormationInvitationReceived[Priority.Low])
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
            foreach (ClanDisbandEventHandler eh in __ClanDisbandCompleted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanDisbandEventHandler eh in __ClanDisbandCompleted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanDisbandEventHandler eh in __ClanDisbandCompleted[Priority.Low])
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
            foreach (ClanChieftanChangeEventHandler eh in __ClanChangeChieftanCompleted[Priority.High])
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
                        new KeyValuePair<string, object>("param: priority", Priority.High),
                        new KeyValuePair<string, object>("param: this", this),
                        new KeyValuePair<string, object>("param: e", e)
                        );
                }
            }

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                foreach (ClanChieftanChangeEventHandler eh in __ClanChangeChieftanCompleted[Priority.Normal])
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
                            new KeyValuePair<string, object>("param: priority", Priority.Normal),
                            new KeyValuePair<string, object>("param: this", this),
                            new KeyValuePair<string, object>("param: e", e)
                            );
                    }
                }
                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    foreach (ClanChieftanChangeEventHandler eh in __ClanChangeChieftanCompleted[Priority.Low])
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
    }
}
