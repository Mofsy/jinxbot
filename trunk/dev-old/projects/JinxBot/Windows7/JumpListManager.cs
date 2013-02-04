using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;

namespace JinxBot.Windows7
{
    internal static class JumpListManager
    {
        private static IJumpListManagerImpl m_target;
        public static void RefreshJumpList(IEnumerable<ClientProfile> profiles)
        {
            if (m_target == null)
            {
                if (TaskbarManager.IsPlatformSupported)
                {
                    m_target = new Windows7JumpListPlatformManager();
                }
                else
                {
                    m_target = new UnsupportedJumpListPlatformManager();
                }
            }

            m_target.RebindProfiles(profiles);
        }

        private interface IJumpListManagerImpl
        {
            void RebindProfiles(IEnumerable<ClientProfile> profiles);
        }

        private class Windows7JumpListPlatformManager : IJumpListManagerImpl
        {
            #region IJumpListManagerImpl Members

            public void RebindProfiles(IEnumerable<ClientProfile> profiles)
            {
                JumpListCustomCategory category = new JumpListCustomCategory("Profiles");

                int currentIndex = 0;
                foreach (var profile in profiles)
                {
                    JumpListLink link = new JumpListLink(Application.ExecutablePath, "--load-profile-" + currentIndex);
                    link.Title = profile.ProfileName;
                    link.Arguments = "--load-profile-" + currentIndex.ToString();
                    link.IconReference = JumpListIconManager.CreateIconForClient(profile.Client);
                    currentIndex++;
                    category.AddJumpListItems(link);
                }

                JumpList list = JumpList.CreateJumpList();
                list.ClearAllUserTasks();
                list.AddCustomCategories(category);

                JumpListLink newProfileLink = new JumpListLink(Application.ExecutablePath, "New Profile...");
                newProfileLink.Arguments = "--new-profile";
                newProfileLink.IconReference = new IconReference(Application.ExecutablePath, 0);
                list.AddUserTasks(newProfileLink);

                list.Refresh();
            }

            #endregion
        }

        private class UnsupportedJumpListPlatformManager : IJumpListManagerImpl
        {
            #region IJumpListManagerImpl Members

            public void RebindProfiles(IEnumerable<ClientProfile> profiles)
            {
                
            }

            #endregion
        }
    }
}
