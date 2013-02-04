using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using JinxBot.Controls.Design;

namespace JinxBot.Controls.Docking
{
    /// <summary>
    /// Represents contents of a docking window.
    /// </summary>
    public class DockContent : Form, IDockContent
    {
        /// <summary>
        /// Creates a new <see>DockContent</see> object.
        /// </summary>
        public DockContent()
        {
            m_dockHandler = new DockContentHandler(this, new GetPersistStringCallback(GetPersistString));
            m_dockHandler.DockStateChanged += new EventHandler(DockHandler_DockStateChanged);
        }

        private DockContentHandler m_dockHandler;
        /// <summary>
        /// Gets the docking handler for this docking content control.
        /// </summary>
        [Browsable(false)]
        public DockContentHandler DockHandler
        {
            get { return m_dockHandler; }
        }

        /// <summary>
        /// Gets or sets whether the end user can control docking.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserDocking
        {
            get { return DockHandler.AllowEndUserDocking; }
            set { DockHandler.AllowEndUserDocking = value; }
        }

        /// <summary>
        /// Gets or sets the docking areas permitted in this control.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_DockAreas_Description")]
        [DefaultValue(DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float)]
        public DockAreas DockAreas
        {
            get { return DockHandler.DockAreas; }
            set { DockHandler.DockAreas = value; }
        }

        /// <summary>
        /// Gets or sets the area that will be defined for auto-hide capability.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_AutoHidePortion_Description")]
        [DefaultValue(0.25)]
        public double AutoHidePortion
        {
            get { return DockHandler.AutoHidePortion; }
            set { DockHandler.AutoHidePortion = value; }
        }

        /// <summary>
        /// Gets or sets the text of the tab.
        /// </summary>
        [Localizable(true)]
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabText_Description")]
        [DefaultValue(null)]
        public string TabText
        {
            get { return DockHandler.TabText; }
            set { DockHandler.TabText = value; }
        }
        private bool ShouldSerializeTabText()
        {
            return (DockHandler.TabText != null);
        }

        /// <summary>
        /// Gets or sets whether a close button is displayed.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_CloseButton_Description")]
        [DefaultValue(true)]
        public bool CloseButton
        {
            get { return DockHandler.CloseButton; }
            set { DockHandler.CloseButton = value; }
        }

        /// <summary>
        /// Gets or sets the docking panel associated with this control.
        /// </summary>
        [Browsable(false)]
        public DockPanel DockPanel
        {
            get { return DockHandler.DockPanel; }
            set { DockHandler.DockPanel = value; }
        }

        /// <summary>
        /// Gets or sets the docking state of this control.
        /// </summary>
        [Browsable(false)]
        public DockState DockState
        {
            get { return DockHandler.DockState; }
            set { DockHandler.DockState = value; }
        }

        /// <summary>
        /// Gets or sets the docking pane associated with this control.
        /// </summary>
        [Browsable(false)]
        public DockPane Pane
        {
            get { return DockHandler.Pane; }
            set { DockHandler.Pane = value; }
        }

        /// <summary>
        /// Gets or sets whether this docking content control is hidden.
        /// </summary>
        [Browsable(false)]
        public bool IsHidden
        {
            get { return DockHandler.IsHidden; }
            set { DockHandler.IsHidden = value; }
        }

        /// <summary>
        /// Gets or sets the docking state of this content control.
        /// </summary>
        [Browsable(false)]
        public DockState VisibleState
        {
            get { return DockHandler.VisibleState; }
            set { DockHandler.VisibleState = value; }
        }

        /// <summary>
        /// Gets or sets whether this control is floating.
        /// </summary>
        [Browsable(false)]
        public bool IsFloat
        {
            get { return DockHandler.IsFloat; }
            set { DockHandler.IsFloat = value; }
        }

        /// <summary>
        /// Gets or sets the docking pane panel associated with this control.
        /// </summary>
        [Browsable(false)]
        public DockPane PanelPane
        {
            get { return DockHandler.PanelPane; }
            set { DockHandler.PanelPane = value; }
        }

        /// <summary>
        /// Gets or sets the floating pane panel associated with this control.
        /// </summary>
        [Browsable(false)]
        public DockPane FloatPane
        {
            get { return DockHandler.FloatPane; }
            set { DockHandler.FloatPane = value; }
        }

        /// <summary>
        /// Gets a string that represents the docking content as XML.
        /// </summary>
        /// <returns>A string designed for persistence.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual string GetPersistString()
        {
            return GetType().ToString();
        }

        /// <summary>
        /// Gets or sets whether to hide content when it is closed.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_HideOnClose_Description")]
        [DefaultValue(false)]
        public bool HideOnClose
        {
            get { return DockHandler.HideOnClose; }
            set { DockHandler.HideOnClose = value; }
        }

        /// <summary>
        /// Gets or sets the docking hits for this control.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_ShowHint_Description")]
        [DefaultValue(DockState.Unknown)]
        public DockState ShowHint
        {
            get { return DockHandler.ShowHint; }
            set { DockHandler.ShowHint = value; }
        }

        /// <summary>
        /// Gets whether this control is currently activated.
        /// </summary>
        [Browsable(false)]
        public bool IsActivated
        {
            get { return DockHandler.IsActivated; }
        }

        /// <summary>
        /// Gets whether the dock state on this control is valid.
        /// </summary>
        /// <param name="dockState">The current docking state.</param>
        /// <returns><see langword="true" /> if the docking state is valid; otherwise <see langword="false" />.</returns>
        public bool IsDockStateValid(DockState dockState)
        {
            return DockHandler.IsDockStateValid(dockState);
        }

        /// <summary>
        /// Gets or sets the context menu associated with the tab page.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabPageContextMenu_Description")]
        [DefaultValue(null)]
        public ContextMenu TabPageContextMenu
        {
            get { return DockHandler.TabPageContextMenu; }
            set { DockHandler.TabPageContextMenu = value; }
        }

        /// <summary>
        /// Gets or sets the context menu strip associated with the tab page.
        /// </summary>
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockContent_TabPageContextMenuStrip_Description")]
        [DefaultValue(null)]
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return DockHandler.TabPageContextMenuStrip; }
            set { DockHandler.TabPageContextMenuStrip = value; }
        }

        /// <summary>
        /// Gets or sets tool tip text.
        /// </summary>
        [Localizable(true)]
        [Category("Appearance")]
        [LocalizedDescription("DockContent_ToolTipText_Description")]
        [DefaultValue(null)]
        public string ToolTipText
        {
            get { return DockHandler.ToolTipText; }
            set { DockHandler.ToolTipText = value; }
        }

        /// <summary>
        /// Activates a docking container.
        /// </summary>
        public new void Activate()
        {
            DockHandler.Activate();
        }

        /// <summary>
        /// Hides the docking container.
        /// </summary>
        public new void Hide()
        {
            DockHandler.Hide();
        }

        /// <summary>
        /// Shows the docking container.
        /// </summary>
        public new void Show()
        {
            DockHandler.Show();
        }

        /// <summary>
        /// Shows the docking container on the specified docking panel.
        /// </summary>
        /// <param name="dockPanel">The docking panel on which to dock.</param>
        public void Show(DockPanel dockPanel)
        {
            DockHandler.Show(dockPanel);
        }

        /// <summary>
        /// Shows the docking container on the specified docking panel in the specified docking state.
        /// </summary>
        /// <param name="dockPanel">The docking panel on which to dock.</param>
        /// <param name="dockState">The docking state to set.</param>
        public void Show(DockPanel dockPanel, DockState dockState)
        {
            DockHandler.Show(dockPanel, dockState);
        }

        /// <summary>
        /// Shows the docking container in the specified floating area.
        /// </summary>
        /// <param name="dockPanel">The docking panel on which to dock.</param>
        /// <param name="floatWindowBounds">The area in which to float.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
        {
            DockHandler.Show(dockPanel, floatWindowBounds);
        }

        /// <summary>
        /// Shows the docking container next to the docking pane.
        /// </summary>
        /// <param name="pane">The docking panel on which to dock.</param>
        /// <param name="beforeContent">The docking container previous to this.</param>
        public void Show(DockPane pane, IDockContent beforeContent)
        {
            DockHandler.Show(pane, beforeContent);
        }

        /// <summary>
        /// Shows the docking container in the specified order.
        /// </summary>
        /// <param name="previousPane">The pane after which to insert this item.</param>
        /// <param name="alignment">The docking alignment.</param>
        /// <param name="proportion">The proportion.</param>
        public void Show(DockPane previousPane, DockAlignment alignment, double proportion)
        {
            DockHandler.Show(previousPane, alignment, proportion);
        }

        /// <summary>
        /// Floats this docking container at the specified boundaries.
        /// </summary>
        /// <param name="floatWindowBounds">The boundaries.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void FloatAt(Rectangle floatWindowBounds)
        {
            DockHandler.FloatAt(floatWindowBounds);
        }

        /// <summary>
        /// Docks the content area at the specified location.
        /// </summary>
        /// <param name="paneTo">The pane to which to dock.</param>
        /// <param name="dockStyle">The docking style.</param>
        /// <param name="contentIndex">The index to insert the panel.</param>
        public void DockTo(DockPane paneTo, DockStyle dockStyle, int contentIndex)
        {
            DockHandler.DockTo(paneTo, dockStyle, contentIndex);
        }

        /// <summary>
        /// Docks the content area at the specified location.
        /// </summary>
        /// <param name="panel">The pane to which to dock.</param>
        /// <param name="dockStyle">The docking style.</param>
        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            DockHandler.DockTo(panel, dockStyle);
        }

        #region Events
        private void DockHandler_DockStateChanged(object sender, EventArgs e)
        {
            OnDockStateChanged(e);
        }

        private static readonly object DockStateChangedEvent = new object();
        /// <summary>
        /// Informs listeners that a docking state has changed.
        /// </summary>
        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("Pane_DockStateChanged_Description")]
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(DockStateChangedEvent, value); }
            remove { Events.RemoveHandler(DockStateChangedEvent, value); }
        }

        /// <summary>
        /// Raises the <see>DockStateChanged</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[DockStateChangedEvent];
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
