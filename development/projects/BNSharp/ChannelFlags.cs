using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp
{
    /// <summary>
    /// Specifies the flags that can be applied to channel-related <see cref="ServerChatEventArgs.Flags">chat events</see>.
    /// </summary>
    [Flags]
    public enum ChannelFlags
    {
        /// <summary>
        /// Specifies that the channel is a normal private channel.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the channel is public.
        /// </summary>
        PublicChannel = 1,
        /// <summary>
        /// Specifies that the channel is moderated by a Blizzard representative.
        /// </summary>
        ModeratedChannel = 2,
        /// <summary>
        /// Specifies that the channel is restricted.
        /// </summary>
        RestrictedChannel = 4,
        /// <summary>
        /// Specifies that the channel is silent.
        /// </summary>
        SilentChannel = 8,
        /// <summary>
        /// Specifies that the channel is provided by the system.
        /// </summary>
        SystemChannel = 0x10,
        /// <summary>
        /// Specifies that the channel is specific to a product.
        /// </summary>
        ProductSpecificChannel = 0x20,
        /// <summary>
        /// Specifies that the channel is globally-accessible.
        /// </summary>
        GloballyAccessibleChannel = 0x1000,
    }
}
