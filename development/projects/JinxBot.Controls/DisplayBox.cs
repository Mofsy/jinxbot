using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Globalization;
using JinxBot.Controls.Design;
using System.Threading;
using System.IO;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;

namespace JinxBot.Controls
{
    /// <summary>
    /// A general text display box that supports chat-centric features.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [DefaultEvent("DisplayReady")]
    public partial class DisplayBox : System.Windows.Forms.UserControl
    {
        #region contained types
        private delegate void AddChatCallback(List<ChatNode> nodes);
        #endregion

        #region fields
        #region technical/implementation fields
        private Dictionary<Type, ChatNodeRenderer> m_renderers;
        private AddChatCallback AddChatImplementation;
        private bool m_docReady = true;
        #endregion
        #region property backers
        private Color m_tsColor = Color.Gray;
        private bool m_useTimestamp = true;
        private string m_timestampFormat = "[{0}.{1:d2}.{2:d2}]";
        private int m_parasToKeep = 300;
        private Uri m_stylesUri;

        private List<List<ChatNode>> m_notReadyYetNodes = new List<List<ChatNode>>();
        #endregion
        #endregion

        #region constructor
        /// <summary>
        /// Creates a new <see>ChatBox</see>.
        /// </summary>
        public DisplayBox()
        {
            InitializeComponent();
            this.AddChatImplementation = new AddChatCallback(AddChatImpl);

            m_renderers = new Dictionary<Type, ChatNodeRenderer>();
            InitializeRenderer(typeof(ChatNode), typeof(ChatNodeRenderer));

            this.StylesheetUri = new Uri(Path.Combine(Environment.CurrentDirectory, "StandardStyles.xaml"), UriKind.Absolute);
        }

        #endregion

        #region event handlers
        //private void display_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    this.m_enterTextElement = display.Document.GetElementById("enterText");
        //    this.m_scrollToElement = display.Document.GetElementById("scrollTo");

        //    foreach (List<ChatNode> nodes in m_notReadyYetNodes)
        //    {
        //        if (InvokeRequired)
        //            BeginInvoke(AddChatImplementation, nodes);
        //        else
        //            AddChatImpl(nodes);
        //    }

        //    m_docReady = true;

        //    //PrintDom(display.Document.DomDocument as IHTMLDocument2);

        //    IHTMLDocument doc = display.Document.DomDocument as IHTMLDocument;
            
        //    OnDisplayReady(e);
        //}
        #endregion

        #region methods
        /// <summary>
        /// Adds a chat node to the display.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(ChatNode node)
        {
            AddChatPrivate(new List<ChatNode> { node });
        }

        /// <summary>
        /// Adds chat nodes to the display in a single line.
        /// </summary>
        /// <param name="node1">The first node to add.</param>
        /// <param name="node2">The second node to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(ChatNode node1, ChatNode node2)
        {
            AddChatPrivate(new List<ChatNode> { node1, node2 });
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
            AddChatPrivate(new List<ChatNode> { node1, node2, node3 });
        }

        /// <summary>
        /// Adds chat nodes to the display in a single line.
        /// </summary>
        /// <param name="nodes">The nodes to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        public void AddChat(params ChatNode[] nodes)
        {
            AddChatPrivate(new List<ChatNode>(nodes));
        }

        /// <summary>
        /// Adds a collection of chat nodes to display on a single line.
        /// </summary>
        /// <param name="chatNodes">An enumerable list of chat nodes to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if a node is <see langword="null" />.</exception>
        public void AddChat(IEnumerable<ChatNode> chatNodes)
        {
            AddChatPrivate(new List<ChatNode>(chatNodes));
        }

        // Checks to see if Invoke is required to add the chat nodes to the window.
        private void AddChatPrivate(List<ChatNode> nodes)
        {
            if (!m_docReady)
            {
                m_notReadyYetNodes.Add(nodes);
            }
            else
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] == null)
                        throw new ArgumentNullException("nodes");
                }

                if (InvokeRequired)
                    BeginInvoke(AddChatImplementation, nodes);
                else
                    AddChatImplementation(nodes);
            }
        }

        // Actually adds the nodes to the chat window.
        private void AddChatImpl(List<ChatNode> nodes)
        {
            if (IncludeTimestamp)
            {
                DateTime now = DateTime.Now;
                string timestamp = string.Format(CultureInfo.CurrentCulture, m_timestampFormat, now.Hour, now.Minute, now.Second);
                ChatNode ts = new ChatNode(timestamp, m_tsColor);
                nodes.InsertRange(0, new ChatNode[] { ts, new ChatNode(" ", Color.Black) });
            }

            Paragraph element = ParseChatNodesIntoParagraph(nodes.ToArray());
            ThreadStart invokee = delegate
            {
                display.Document.Blocks.Add(element);
                if (display.Viewer.Selection.IsEmpty) 
                    display.Viewer.FindFirstVisualDescendantOfType<ScrollViewer>().ScrollToBottom();
            };
            display.Dispatcher.BeginInvoke(invokee);

            UpdateParagraphs();
        }

        private void ScrollIntoView()
        {
            ThreadStart ts = delegate
            {
                display.Viewer.FindFirstVisualDescendantOfType<ScrollViewer>().ScrollToBottom();
            };
            display.Dispatcher.BeginInvoke(ts);
        }

        private void UpdateParagraphs()
        {
            ThreadStart ts = delegate
            {
                if (display.Document.Blocks.Count > m_parasToKeep)
                {
                    int maxChildrenToRemove = m_parasToKeep / 8;
                    int currentChildrenRemoved = 0;
                    while (currentChildrenRemoved <= maxChildrenToRemove && display.Document.Blocks.Count > 1)
                    {
                        display.Document.Blocks.Remove(display.Document.Blocks.FirstBlock);
                        currentChildrenRemoved++;
                    }
                }
            };

            display.Dispatcher.BeginInvoke(ts);
        }
        #endregion

        #region Parsing logic
        private Paragraph ParseChatNodesIntoParagraph(ChatNode[] nodes)
        {
            Paragraph fullElement = new Paragraph();
            foreach (ChatNode node in nodes)
            {
                Inline child = RenderNode(node);
                fullElement.Inlines.Add(child);
            }

            return fullElement;
        }

        private Inline RenderNode(ChatNode node)
        {
            if (!m_renderers.ContainsKey(node.GetType()))
            {
                Type rendererType = ChatNodeRendererAttribute.RetrieveFromType(node.GetType());
                InitializeRenderer(node.GetType(), rendererType);
            }

            return m_renderers[node.GetType()].Render(node);
        }

        private void InitializeRenderer(Type nodeType, Type rendererType)
        {
            if (!typeof(ChatNodeRenderer).IsAssignableFrom(rendererType))
                throw new InvalidCastException("Renderer type must be derived from ChatNodeRenderer.");
            if (!typeof(ChatNode).IsAssignableFrom(nodeType))
                throw new InvalidCastException("Node type must be derived from ChatNode.");

            ChatNodeRenderer renderer = Activator.CreateInstance(rendererType) as ChatNodeRenderer;
            m_renderers.Add(nodeType, renderer);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether timestamps should be automatically prepended to each new chat entry.
        /// </summary>
        [LocalizedCategory("CatBehavior")]
        [LocalizedDescription("IncludeTimestamp.")]
        [DefaultValue(true)]
        public bool IncludeTimestamp
        {
            get { return m_useTimestamp; }
            set { m_useTimestamp = value; }
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
            get { return m_tsColor; }
            set 
            {
                if (value == Color.Empty)
                    throw new ArgumentOutOfRangeException("value");

                m_tsColor = value; 
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
            get { return m_timestampFormat; }
            set
            {
                try
                {
                    DateTime test = DateTime.Now;
                    // suppressed CA1806 because we want to make sure the value is functional, not do anything with the test result.
                    string.Format(CultureInfo.CurrentCulture, value, test.Hour, test.Minute, test.Second);
                }
                catch (FormatException fe)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(CultureInfo.CurrentCulture, Resources.TimestampFormatInvalid, value), fe);
                }
                m_timestampFormat = value;
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
            get { return m_parasToKeep; }
            set 
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", value, "Value must be nonzero and nonnegative.");

                m_parasToKeep = value;

                UpdateParagraphs();
            }
        }

        
        /// <summary>
        /// Gets or sets the URI of the stylesheet to use.
        /// </summary>
        [LocalizedCategory("CatAppearance")]
        [LocalizedDescription("StylesheetUri")]
        [Browsable(false)]
        [TypeConverter(typeof(UriTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Uri StylesheetUri
        {
            get
            {
                return m_stylesUri;
            }
            set
            {
                if (object.ReferenceEquals(value, null))
                    throw new ArgumentNullException("value");

                ResourceDictionary dictionary = new ResourceDictionary();
                dictionary.Source = value;

                foreach (string key in dictionary.Keys)
                {
                    display.Resources[key] = dictionary[key];
                }
            }
        }
        #endregion

        #region events
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

        #endregion

        /// <summary>
        /// Copies the selected text as UBBC code.  This is experimental.
        /// </summary>
        public void CopySelectionAsUbbc()
        {

        }

        /// <summary>
        /// Copies the selected text as plain text.
        /// </summary>
        public void CopySelectionAsPlainText()
        {
            if (display.Viewer.Selection != null)
            {
                System.Windows.Clipboard.SetText(display.Viewer.Selection.Text, System.Windows.TextDataFormat.Text);
            }
        }

        /// <summary>
        /// Copies the selected text as Rich Text Format.
        /// </summary>
        public void CopySelectionAsRtf()
        {
            if (display.Viewer.Selection != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    display.Viewer.Selection.Save(stream, System.Windows.DataFormats.Rtf);
                    System.Windows.Clipboard.SetData(System.Windows.DataFormats.Rtf, Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Copies the selected text as HTML.
        /// </summary>
        public void CopySelectionAsHtml()
        {
            if (display.Viewer.Selection != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    display.Viewer.Selection.Save(stream, System.Windows.DataFormats.Html);
                    System.Windows.Clipboard.SetData(System.Windows.DataFormats.Html, Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
        }

        private void display_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void copyAsPlainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectionAsPlainText();
        }

        private void copyAsHTMLTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectionAsHtml();
        }

        private void copyAsUBBCForumTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectionAsUbbc();
        }
    }
}
