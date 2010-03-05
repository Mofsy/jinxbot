using System;
using System.Xml.Serialization;

namespace JinxBot.Configuration
{
    /// <summary>
    /// Specifies the file data for an XML list for icon files to be downloaded from the web.  This class supports the 
    /// JinxBot icon infrastructure and is not intended to be used from your code.
    /// </summary>
    [XmlRoot("WebIcons")]
    public class WebIconList
    {
        /// <summary>
        /// Creates a new WebIconList.
        /// </summary>
        /// <remarks>
        /// <para>This constructor supports the 
        /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
        /// </remarks>
        public WebIconList() { }

        /// <summary>
        /// Gets or sets the list of icons exposed by this list.
        /// </summary>
        /// <remarks>
        /// This property supports the 
        /// JinxBot icon infrastructure and is not intended to be used from your code.
        /// </remarks>
        [XmlArray(ElementName="IconList")]
        public Icon[] Icons { get; set; }

        /// <summary>
        /// Specifies information about an icon contained within an XML list for icon files to be downloaded from the web.  This class supports the 
        /// JinxBot icon infrastructure and is not intended to be used from your code.
        /// </summary>
        public class Icon
        {
            /// <summary>
            /// Creates a new <see>Icon</see>.
            /// </summary>
            public Icon() { }

            /// <summary>
            /// Gets or sets the client ID for which this icon is used, if any.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Client")]
            public string ClientID { get; set; }

            /// <summary>
            /// Gets or sets the clan ranks for which this icon is used, if any.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("ClanRank")]
            public string ClanRank { get; set; }

            /// <summary>
            /// Gets or sets the user flags for which this icon is used, if any.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("UserFlags")]
            public string UserFlags { get; set; }

            /// <summary>
            /// Gets or sets the icon tier, if any, for which this icon is used.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Tier")]
            public int Tier { get; set; }

            internal char Race { get { return Convert.ToChar(IconRace); } }

            /// <summary>
            /// Gets or sets the icon race, if any, for which this icon is used.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Race")]
            public string IconRace { get; set; }

            /// <summary>
            /// Gets or sets the URL from which this icon is downloaded.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Url")]
            public string Uri { get; set; }

            /// <summary>
            /// Gets or sets the URL from which the corresponding animated icon can be downloaded.
            /// </summary>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            [XmlAttribute("AnimationUrl")]
            public string AnimationUri { get; set; }

            /// <summary>
            /// Gets or sets the name of the animation as it is locally-stored.
            /// </summary>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            [XmlAttribute("AnimationLocal")]
            public string LocalAnimationName { get; set; }

            /// <summary>
            /// Gets or sets the name of the icon as it is locally-stored.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Local")]
            public string LocalName { get; set; }

            /// <summary>
            /// Gets or sets whether this icon should be cropped when downloaded.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Crop")]
            public bool Crop { get; set; }

            /// <summary>
            /// Gets or sets the top pixel at which this icon should be cropped.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Top")]
            public int Top { get; set; }

            /// <summary>
            /// Gets or sets the relative bottom pixel at which this icon should be cropped.
            /// </summary>
            /// <remarks>
            /// <para>This property supports the 
            /// JinxBot icon infrastructure and is not intended to be used from your code.</para>
            /// </remarks>
            [XmlAttribute("Bottom")]
            public int Bottom { get; set; }
        }
    }
}
