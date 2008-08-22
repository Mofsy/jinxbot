using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Globalization;
using mshtml;
using JinxBot.Controls.Design;
using System.Threading;
using System.IO;

namespace JinxBot.Controls
{
    /// <summary>
    /// A general text display box that supports chat-centric features.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [DefaultEvent("DisplayReady")]
    public partial class DisplayBox : UserControl
    {
        private static string scriptInitializer;
        #region HTML document
        private const string HTML = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
	<head>
		<title>JinxBot Display Interface</title>
<!-- Script injection -->
{0}

	</head>	
	<body>
	    <div id=""enterText""></div>
        <div id=""scrollTo"">&nbsp;</div>
	</body>
</html>";
        #endregion
        #region contained types
        private delegate void AddChatCallback(List<ChatNode> nodes);
        #endregion

        #region fields
        #region technical/implementation fields
        private Dictionary<Type, ChatNodeRenderer> m_renderers;
        private AddChatCallback AddChatImplementation;
        private HtmlElement m_enterTextElement, m_scrollToElement;
        #endregion
        #region property backers
        private Color m_tsColor = Color.Gray;
        private bool m_useTimestamp = true;
        private string m_timestampFormat = "[{0}.{1:d2}.{2:d2}]";
        private int m_parasToKeep = 300;
        private Uri m_stylesUri;
        #endregion
        #endregion

        #region constructor
        /// <summary>
        /// Creates a new <see>ChatBox</see>.
        /// </summary>
        public DisplayBox()
        {
            InitializeExternals();
            InitializeComponent();
            this.AddChatImplementation = new AddChatCallback(AddChatImpl);
            display.DocumentText = string.Format(CultureInfo.InvariantCulture, HTML, scriptInitializer);
        }

        private void InitializeExternals()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JinxBot.Controls");
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            string mootoolsBasePath = Path.Combine(appDataPath, "mootools-base.js");
            string mootoolsExtensionsPath = Path.Combine(appDataPath, "jinxbot-mootools.js");
            string defaultStylesPath = Path.Combine(appDataPath, "DefaultStyles.css");
            if (!File.Exists(mootoolsBasePath) || new FileInfo(mootoolsBasePath).Length != Resources.mootools_core.Length)
            {
                File.WriteAllText(mootoolsBasePath, Resources.mootools_core, Encoding.UTF8);
            }

            if (!File.Exists(mootoolsExtensionsPath) || new FileInfo(mootoolsExtensionsPath).Length != Resources.jinxbot_mootools.Length)
            {
                File.WriteAllText(mootoolsExtensionsPath, Resources.jinxbot_mootools, Encoding.UTF8);
            }

            if (!File.Exists(defaultStylesPath) || new FileInfo(defaultStylesPath).Length != Resources.DefaultStyles_css.Length)
            {
                File.WriteAllText(defaultStylesPath, Resources.DefaultStyles_css, Encoding.UTF8);
            }

            scriptInitializer = string.Format(CultureInfo.InvariantCulture, @"<link type=""text/css"" src=""file:///{2}"" rel=""stylesheet"" />
<script type=""text/javascript"" src=""file:///{0}"" />
<script type=""text/javascript"" src=""file:///{1}"" />
", mootoolsBasePath.Replace(Path.PathSeparator, '/'), mootoolsExtensionsPath.Replace(Path.PathSeparator, '/'),
 defaultStylesPath.Replace(Path.PathSeparator, '/'));
            scriptInitializer = string.Format(CultureInfo.InvariantCulture, @"
<script type=""text/javascript"">
{0}

{1}
</script>
<style type=""text/css"">
{2}
</style>
", Resources.mootools_core, Resources.jinxbot_mootools, Resources.DefaultStyles_css);
            
            m_stylesUri = new Uri(string.Concat("file:///", defaultStylesPath.Replace(Path.PathSeparator, '/')));
        }
        #endregion

        #region event handlers
        private void display_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.m_enterTextElement = display.Document.GetElementById("enterText");
            this.m_scrollToElement = display.Document.GetElementById("scrollTo");

            //PrintDom(display.Document.DomDocument as IHTMLDocument2);

            m_renderers = new Dictionary<Type, ChatNodeRenderer>();
            InitializeRenderer(typeof(ChatNode), typeof(ChatNodeRenderer));

            IHTMLDocument doc = display.Document.DomDocument as IHTMLDocument;
            
            OnDisplayReady(e);
        }
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

            HtmlElement element = ParseChatNodesIntoHtmlElement(nodes.ToArray());
            m_enterTextElement.AppendChild(element);

            ScrollIntoView();

            UpdateParagraphs();
        }

        private void ScrollIntoView()
        {
            IHTMLDocument2 doc = (IHTMLDocument2)display.Document.DomDocument;

            if (doc.selection != null && doc.selection.type.Equals("None", StringComparison.Ordinal))
                m_scrollToElement.ScrollIntoView(false);
        }

        private void UpdateParagraphs()
        {
            ThreadStart ts = delegate
            {
                if (m_enterTextElement == null) return;

                if (m_enterTextElement.Children.Count > m_parasToKeep)
                {
                    int maxChildrenToRemove = m_parasToKeep / 8;
                    int currentChildrenRemoved = 0;
                    while (currentChildrenRemoved <= maxChildrenToRemove && m_enterTextElement != null && m_enterTextElement.Children.Count > 1)
                    {
                        IHTMLDOMNode node = (IHTMLDOMNode)m_enterTextElement.DomElement;
                        node.removeChild((IHTMLDOMNode)m_enterTextElement.FirstChild.DomElement);
                        currentChildrenRemoved++;
                    }
                }
            };

            if (InvokeRequired)
                BeginInvoke(ts);
            else
                ts();
        }
        #endregion

        #region Parsing logic
        private HtmlElement ParseChatNodesIntoHtmlElement(ChatNode[] nodes)
        {
            HtmlElement fullElement = display.Document.CreateElement("p");
            foreach (ChatNode node in nodes)
            {
                HtmlElement childElement = RenderNode(node);
                fullElement.AppendChild(childElement);
            }

            return fullElement;
        }

        private HtmlElement RenderNode(ChatNode node)
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
            renderer.HtmlDomDocument = this.display.Document;
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
                try
                {
                    m_stylesUri = value;
                } catch (Exception ex)
                {
                    throw new ArgumentException("Could not set the stylesheet to the specified URI.", "value", ex);
                }

                string uri = value.ToString();

                IHTMLDocument2 doc = display.Document.DomDocument as IHTMLDocument2;
                IHTMLWindow2 window = doc.parentWindow;
                window.execScript(string.Format(CultureInfo.InvariantCulture, "new Asset.css('{0}');", uri.Replace("'", "\\'")), "javascript");

                this.display.Dock = DockStyle.None;
                this.display.Size = this.display.Size;
                this.display.Dock = DockStyle.Fill;
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
    }
}
