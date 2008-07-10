using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using JinxBot.Controls.Design;

namespace JinxBot.Controls
{
    /// <summary>
    /// Implements a chat display window and text input field.
    /// </summary>
    /// <remarks>
    /// <para>For users who wish to utilize their own text input field or do not like the layout of the ChatBox control, an alternative is the 
    /// <see>DisplayBox</see>.  The <see>DisplayBox</see> control only displays output text; there is no mechanism for input.</para>
    /// </remarks>
    [DefaultEvent("MessageReady")]
    [DefaultProperty("IncludeTimestamp")]
    public partial class ChatBox : UserControl
    {
        #region fields
        #region property backers
        private int m_maxMostRecentMessages = 15;
        #endregion
        #region implementation/technical details
        #endregion
        #endregion

        /// <summary>
        /// Creates a new <see>ChatBox</see>.
        /// </summary>
        public ChatBox()
        {
            InitializeComponent();
            display.DisplayReady += new EventHandler(display_DisplayReady);
        }

        #region event handlers
        private void display_DisplayReady(object sender, EventArgs e)
        {
            OnDisplayReady(e);
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                GetTextAndClear();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                inputBox.Text = string.Empty;
                OnCleared(e);
                e.Handled = true;
            }
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            if (inputBox.Text.Length > 0)
                OnMessageChanged(new MessageEventArgs(inputBox.Text));
            else
                OnCleared(e);
        }

        #endregion

        #region methods
        #region AddChat wrappers
        /// <summary>
        /// Adds a chat node to the display.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(ChatNode node)
        {
            display.AddChat(node);
        }

        /// <summary>
        /// Adds chat nodes to the display in a single line.
        /// </summary>
        /// <param name="node1">The first node to add.</param>
        /// <param name="node2">The second node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(ChatNode node1, ChatNode node2)
        {
            display.AddChat(node1, node2);
        }

        /// <summary>
        /// Adds chat nodes to the display in a single line.
        /// </summary>
        /// <param name="node1">The first node to add.</param>
        /// <param name="node2">The second node to add.</param>
        /// <param name="node3">The third node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(ChatNode node1, ChatNode node2, ChatNode node3)
        {
            display.AddChat(node1, node2, node3);
        }

        /// <summary>
        /// Adds a collection of chat nodes to display on a single line.
        /// </summary>
        /// <param name="chatNodes">An enumerable list of chat nodes to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <see langword="null" />.</exception>
        public void AddChat(IEnumerable<ChatNode> chatNodes)
        {
            display.AddChat(chatNodes);
        }

        /// <summary>
        /// Adds chat nodes to the display in a single line.
        /// </summary>
        /// <param name="nodes">The nodes to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(params ChatNode[] nodes)
        {
            display.AddChat(new List<ChatNode>(nodes));
        }
        #endregion
        #region combo box input management
        // checks to make sure that the max most-recently-used messages are obeyed
        private void CheckMaxMruMessages()
        {
            for (int i = inputBox.Items.Count - 1; i >= 0; i--)
            {
                if (i > m_maxMostRecentMessages)
                    inputBox.Items.RemoveAt(i);
            }
            inputBox.MaxDropDownItems = m_maxMostRecentMessages + 1;
        }

        private void GetTextAndClear()
        {
            if (inputBox.Text.Length > 0)
            {
                OnMessageReady(new MessageEventArgs(inputBox.Text));
            }
            inputBox.Items.Insert(0, inputBox.Text);
            CheckMaxMruMessages();

            inputBox.Text = String.Empty;
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the maximum number of recent messages that should be stored.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is negative.</exception>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("MaxMostRecentMessages")]
        [DefaultValue(15)]
        public int MaxMostRecentMessages
        {
            get { return m_maxMostRecentMessages; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", Resources.MostRecentMessagesMustBePositive);
                m_maxMostRecentMessages = value;
                CheckMaxMruMessages();
            }
        }
        /// <summary>
        /// Gets or sets whether timestamps should be automatically prepended to each new chat entry.
        /// </summary>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("IncludeTimestampChatBox")]
        [DefaultValue(true)]
        public bool IncludeTimestamp
        {
            get { return display.IncludeTimestamp; }
            set { display.IncludeTimestamp = value; }
        }

        /// <summary>
        /// Gets or sets the color that should be used for new timestamps.
        /// </summary>
        /// <remarks>
        /// <para>This setting only applied to timestamps included when the <see cref="IncludeTimestamp">IncludeTimestamp property</see> 
        /// is set to <b>true</b>.</para>
        /// <para>The default value of this property is <see>Color.Gray</see>.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the assigned value is <see>Color.Empty</see>.</exception>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("TimestampColor")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color TimestampColor
        {
            get { return display.TimestampColor; }
            set 
            {
                display.TimestampColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp format that should be used for new timestamps.
        /// </summary>
        /// <remarks>
        /// <para>This setting is applied when timestamps are included by setting the <see cref="IncludeTimestamp">IncludeTimestamp property</see>
        /// to <b>true</b>.</para>
        /// <para>The property should have at most three format parameters.  The first parameter is assigned the current hour, the second parameter
        /// is assigned the current minute, and the third parameter is assigned the current second.</para>
        /// <para>The default value of this property is <c>"[{0}.{1:d2}.{2:d2}]"</c>.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the current time could not be formatted when a new format is assigned.</exception>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("TimestampFormat")]
        [DefaultValue("[{0}.{1:d2}.{2:d2}]")]
        public string TimestampFormat
        {
            get { return display.TimestampFormat; }
            set
            {
                display.TimestampFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of paragraphs to keep in the display.
        /// </summary>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("MaxDisplayedParagraphs")]
        [DefaultValue(300)]
        public int MaxDisplayedParagraphs
        {
            get { return display.MaxDisplayedParagraphs; }
            set
            {
                display.MaxDisplayedParagraphs = value;
            }
        }
        #endregion

        #region events
        /// <summary>
        /// Informs listeners that the input box has been cleared.
        /// </summary>
        [LocalizedCategory("CatInput")]
        [LocalizedDescription("InputCLearedEvent")]
        public event EventHandler InputCleared;
        /// <summary>
        /// Raises the <see>InputCleared</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnInputCleared(EventArgs e)
        {
            if (InputCleared != null)
                InputCleared(this, e);
        }
        /// <summary>
        /// Informs listeners that the display is ready for chat nodes to be added.
        /// </summary>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("DisplayReadyEvent")]
        public event EventHandler DisplayReady;
        /// <summary>
        /// Raises the <see cref="DisplayReady">DisplayReady</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        // RAP: Suppressed CA2109 because FxCop is dumb.
        protected virtual void OnDisplayReady(EventArgs e)
        {
            if (DisplayReady != null)
                DisplayReady(this, e);
        }

        /// <summary>
        /// Informs listeners that the display has a message ready to be sent.
        /// </summary>
        [LocalizedCategory("CatInput")]
        [LocalizedDescription("MessageReadyEvent")]
        public event MessageEventHandler MessageReady;
        /// <summary>
        /// Raises the <see>MessageReady</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnMessageReady(MessageEventArgs e)
        {
            if (MessageReady != null)
                MessageReady(this, e);
        }

        /// <summary>
        /// Informs listeners that the input text has changed, though it is not necessarily ready to be sent.
        /// </summary>
        [LocalizedCategory("CatInput")]
        [LocalizedDescription("MessageChangedEvent")]
        public event MessageEventHandler MessageChanged;
        /// <summary>
        /// Raises the <see>MessageChanged</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnMessageChanged(MessageEventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }

        /// <summary>
        /// Informs listeners that the input text has been emptied, either by the user or by the control.
        /// </summary>
        [LocalizedCategory("CatInput")]
        [LocalizedDescription("ClearedEvent")]
        public event EventHandler Cleared;
        /// <summary>
        /// Raises the <see>Cleared</see> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnCleared(EventArgs e)
        {
            if (Cleared != null)
                Cleared(this, e);
        }
        #endregion


    }
}
