using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BNSharp.Net;
using JinxBot.Plugins;
using JinxBot.Configuration;
using JinxBot.Views.Chat;
using JinxBot.Plugins.UI;

namespace JinxBot.Views
{
    public class ProfileResourceProvider : IDisposable
    {
        #region IconProviderFactory class
        private static class IconProviderFactory
        {
            private static Dictionary<string, IIconProvider> s_existingProviders = new Dictionary<string, IIconProvider>();
            private static readonly object sync = new object();
            
            public static IIconProvider GetProvider(string providerName)
            {
                lock (sync)
                {
                    if (s_existingProviders.Count == 0)
                    {
                        InitializeDictionary();
                    }

                    if (s_existingProviders.ContainsKey(providerName))
                    {
                        return s_existingProviders[providerName];
                    }

                    return null;
                }
            }

            private static void InitializeDictionary()
            {
                JinxBotConfiguration config = JinxBotConfiguration.Instance;
                foreach (var element in config.Globals.IconProviders)
                {
                    Type type = Type.GetType(element.TypeName);
                    if (type != null && typeof (IIconProvider).IsAssignableFrom(type))
                    {
                        IIconProvider provider = Activator.CreateInstance(type) as IIconProvider;
                        s_existingProviders.Add(element.Name, provider);
                    }
                }
            }
        }
        #endregion
        #region statics
        private static Dictionary<BattleNetClient, ProfileResourceProvider> s_providers = new Dictionary<BattleNetClient, ProfileResourceProvider>();
        /// <summary>
        /// Registers a provider for the specified connection and returns the provider.
        /// </summary>
        /// <param name="client">The client connection to register.</param>
        /// <returns>A <see>ProfileResourceProvider</see> if the profile was newly registered; or <see langword="null" /> if the profile
        /// was already registered.</returns>
        public static ProfileResourceProvider RegisterProvider(BattleNetClient client)
        {
            ProfileResourceProvider provider = null;
            if (!s_providers.ContainsKey(client))
            {
                provider = new ProfileResourceProvider(client);
                s_providers.Add(client, provider);
            }

            return provider;
        }

        /// <summary>
        /// Unregisters a profile and cleans up its resources.
        /// </summary>
        /// <param name="client">The client to unregister.</param>
        public static void UnregisterProvider(BattleNetClient client)
        {
            if (!s_providers.ContainsKey(client))
                return;

            ProfileResourceProvider provider = s_providers[client];
            s_providers.Remove(client);

            provider.Dispose();
        }

        /// <summary>
        /// Gets a <see>ProfileResourceProvider</see> for the specified client.
        /// </summary>
        /// <param name="client">The client for which to get the provider.</param>
        /// <returns>A <see>ProfileResourceProvider</see> instance if one was registered; otherwise <see langword="null" />.</returns>
        public static ProfileResourceProvider GetForClient(BattleNetClient client)
        {
            if (object.ReferenceEquals(null, client))
            {
                KeyValuePair<BattleNetClient, ProfileResourceProvider> prp = s_providers.FirstOrDefault();
                
                return prp.Value;
            }

            if (s_providers.ContainsKey(client))
                return s_providers[client];

            return null;
        }
        #endregion

        private IIconProvider m_iconProvider;

        private ProfileResourceProvider(BattleNetClient client)
        {
            ClientProfile profile = client.Settings as ClientProfile;
            m_iconProvider = IconProviderFactory.GetProvider(profile.IconProviderType);
        }

        /// <summary>
        /// Gets the <see>IIconProvider</see> for this profile.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Raised if this object has been disposed.</exception>
        public IIconProvider Icons
        {
            get
            {
                if (m_iconProvider == null)
                    throw new ObjectDisposedException("ProfileResourceProvider");
                return m_iconProvider;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object, cleaning up unmanaged and managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object, cleaning up unmanaged and optionally managed resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_iconProvider != null)
                {
                    m_iconProvider.Dispose();
                    m_iconProvider = null;
                }
            }
        }

        #endregion
    }
}
