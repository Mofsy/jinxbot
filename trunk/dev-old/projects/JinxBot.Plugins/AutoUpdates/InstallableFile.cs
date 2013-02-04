using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace JinxBot.Plugins.AutoUpdates
{
    [DataContract]
    public class InstallableFile
    {
        private const int HASH_LENGTH = 20;
        private const string INSTALL_DIR_TOKEN = "{INSTALLDIR}";
        private const string PLUGINS_DIR_TOKEN = "{PLUGINSDIR}";

        [DataMember(Name = "FileHash")]
        private byte[] m_hash;
        [DataMember(Name = "Filename")]
        private string m_name;

        public bool CompareHash(byte[] calculatedFileHash)
        {
            Debug.Assert(calculatedFileHash != null);
            Debug.Assert(calculatedFileHash.Length == HASH_LENGTH);

            for (int i = 0; i < HASH_LENGTH; i++)
            {
                if (m_hash[i] != calculatedFileHash[i])
                    return false;
            }
            return true;
        }

        public string Filename
        {
            get { return m_name; }
        }

        public string GetAbsolutePath(string applicationInstallationPath, string pluginsPath)
        {
            return m_name.Replace(INSTALL_DIR_TOKEN, applicationInstallationPath).Replace(PLUGINS_DIR_TOKEN, pluginsPath);
        }

        public static byte[] CalculateFileHash(string filePath)
        {
            byte[] result = null;
            using (FileStream fs = File.OpenRead(filePath))
            using (SHA1 hasher = new SHA1Managed())
            {
                result = hasher.ComputeHash(fs);
            }

            return result;
        }
    }
}
