﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp;
using System.ComponentModel;
using JinxBot.Design;
using JinxBot.Controls.Design;
using System.Drawing.Design;
using System.Xml.Serialization;
using BNSharp.BattleNet;
using JinxBot.Configuration;

namespace JinxBot
{
    [Serializable]
    public class ClientProfile : IBattleNetSettings
    {
        public ClientProfile()
        {
            this.Server = "useast.battle.net";
            this.Port = 6112;
        }

        #region IBattleNetSettings Members

        [Browsable(true)]
        [TypeConverter(typeof(ProductStringTypeConverter))]
        [Name("Emulated Client")]
        [Category("Emulation")]
        [Description("Specifies the client to be emulated when connecting to Battle.net.")]
        [XmlElement("Client")]
        public string Client
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Version Byte")]
        [TypeConverter(typeof(VersionByteTypeConverter))]
        [Category("Emulation")]
        [Description("Specifies the current version byte of the selected client.  This information is typically provided an active Battle.net communities, and is typically a hexadecimal number, such as d1 or 0xd1.")]
        [XmlElement("VersionByte")]
        public int VersionByte
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Primary CD Key")]
        [Category("CD Keys")]
        [Description("The main CD key for your game.  This would be your Starcraft, Warcraft II: Battle.net Edition, Diablo II, or Warcraft III: The Reign of Chaos CD key.")]
        [XmlElement("PrimaryCdKey")]
        public string CdKey1
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Secondary CD Key")]
        [Category("CD Keys")]
        [Description("The expansion CD key for your game.  This would be your Diablo II: Lord of Destruction or Warcraft III: The Frozen Throne CD key.  Starcraft: Brood War does not require a CD key.")]
        [XmlElement("SecondaryCdKey")]
        public string CdKey2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Program File Path")]
        [Category("Versioning")]
        [Description("The path to your game executable file.  This should be Starcraft.exe, Warcraft II BNE.exe, Game.exe (for Diablo II), or War3.exe.")]
        [Editor(typeof(ExeFileBrowserTypeEditor), typeof(UITypeEditor))]
        [XmlElement("GameExePath")]
        public string GameExe
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Storm File Path")]
        [Category("Versioning")]
        [Description("The path to the Storm implementation for your client.  This is typically Storm.dll, or in Diablo II, Bnclient.dll.")]
        [Editor(typeof(DllFileBrowserTypeEditor), typeof(UITypeEditor))]
        [XmlElement("StormDllPath")]
        public string GameFile2
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Battle.snp File Path")]
        [Category("Versioning")]
        [Description("The path to the Battle.net loadable module for your client.  This is Battle.snp for Starcraft and Warcraft II: Battle.net Edition, D2Client.snp for Diablo II, and Game.dll for Warcraft III.")]
        [Editor(typeof(DllFileBrowserTypeEditor), typeof(UITypeEditor))]
        [XmlElement("BattleSnpPath")]
        public string GameFile3
        {
            get;
            set; 
        }

        [Browsable(true)]
        [Name("Account Name")]
        [Category("Authentication")]
        [Description("The name of the account with which you should log into Battle.net.")]
        [XmlElement("Username")]
        public string Username
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Lockdown Screen Dump")]
        [Category("Versioning")]
        [Description("For clients that need to support Lockdown (presently Starcraft and Warcraft II: Battle.net Edition), clients must include an additional screen dump file.  This is typically called \"STAR.bin\" or \"W2BN.bin\".")]
        [Editor(typeof(BinFileBrowserTypeEditor), typeof(UITypeEditor))]
        [XmlElement("LockdownImagePath")]
        public string ImageFile
        {
            get;
            set;
        }

        [Browsable(true)]
        [Name("Account Password")]
        [PasswordPropertyText(true)]
        [Category("Authentication")]
        [Description("The password to the account with which you should log into Battle.net.  PLEASE NOTE: This value will be stored in plain text on your computer.")]
        [XmlElement("Password")]
        public string Password
        {
            get;
            set;
        }

        [Name("Server")]
        [Category("Emulation")]
        [Description("Specifies the Battle.net server to which you want to connect.")]
        [Browsable(true)]
        [XmlIgnore]
        [TypeConverter(typeof(BattleNetServerTypeConverter))]
        internal Server BattleNetServer
        {
            get
            {
                return new Server(Server, Port);
            }
            set
            {
                if (!object.ReferenceEquals(null, value))
                {
                    Server = value.Host;
                    Port = value.Port;
                }
                else
                {
                    Server = null;
                    Port = 0;
                }
            }
        }

        [Browsable(false)]
        [XmlElement("ServerUri")]
        public string Server
        {
            get;
            set;
        }

        [Browsable(false)]
        [XmlElement("ServerPort")]
        public int Port
        {
            get;
            set;
        }

        [Browsable(true)]
        [Category("Authentication")]
        [Description("The name of the CD key owner.  This field is used in the event that you attempt to log onto Battle.net more than once with the same CD key.")]
        [Name("CD Key Owner")]
        [XmlElement("CdKeyOwnerName")]
        public string CdKeyOwner
        {
            get;
            set;
        }

        PingType IBattleNetSettings.PingMethod
        {
            get { return (PingType)PingStyle; }
            set { PingStyle = (PingStyle)((int)value); }
        }

        [Browsable(true)]
        [Category("Emulation")]
        [Description("The style of ping used by the client.  This affects the value of your client's latency as it appears to Battle.net.")]
        [Name("Client Ping")]
        [XmlElement("Ping")]
        public PingStyle PingStyle
        {
            get;
            set;
        }

        #endregion


        [Browsable(true)]
        [Name("Profile Name")]
        [Category("JinxBot User Profile")]
        [Description("The name of this profile as JinxBot will use it.")]
        [XmlAttribute("ProfileName")]
        public string ProfileName
        {
            get;
            set;
        }

        [Browsable(false)]
        public PluginsConfiguration PluginsConfiguration
        {
            get;
            set;
        }
    }
}
