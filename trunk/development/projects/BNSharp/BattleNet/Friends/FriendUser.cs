using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Friends
{
    /// <summary>
    /// Represents a Battle.net friend user.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class FriendUser
    {
        #region fields
#if !NET_2_ONLY
        [DataMember]
#endif
        private int m_index;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_acctName;
#if !NET_2_ONLY
        [DataMember]
#endif
        private FriendStatus m_status;
#if !NET_2_ONLY
        [DataMember]
#endif
        private FriendLocation m_location;
#if !NET_2_ONLY
        [DataMember]
#endif
        private Product m_product;
#if !NET_2_ONLY
        [DataMember]
#endif
        private string m_locationName;
        #endregion

        /// <summary>
        /// Creates a new <see>FriendUser</see>.
        /// </summary>
        /// <param name="index">The 0-based index of the user's location.</param>
        /// <param name="accountName">The account name of the friend.</param>
        /// <param name="status">The friend's current status.</param>
        /// <param name="locationType">The friend's current location information.</param>
        /// <param name="product">The product with which the friend is currently logged on, otherwise <see langword="null" />.</param>
        /// <param name="location">The name of the friend's current location.</param>
        public FriendUser(int index, string accountName, FriendStatus status, FriendLocation locationType, Product product, string location)
        {
            m_index = index;
            m_acctName = accountName;
            m_status = status;
            m_location = locationType;
            m_product = product;
            m_locationName = location;
        }

        /// <summary>
        /// Gets, and in derived classes sets, the index (0-based) of the user on the client's friends list.
        /// </summary>
        public int Index
        {
            get { return m_index; }
            protected internal set { m_index = value; } 
        }

        /// <summary>
        /// Gets, and in derived classes sets, the account name of the friend.
        /// </summary>
        public string AccountName
        {
            get { return m_acctName; }
            protected internal set { m_acctName = value; }
        }

        /// <summary>
        /// Gets, and in derived classes sets, a reference to the product information about the user's current logged on state.
        /// </summary>
        /// <remarks>
        /// <para>This property will return <see langword="null" /> if the user is currently offline.</para>
        /// </remarks>
        public Product Product
        {
            get { return m_product; }
            protected internal set { m_product = value; }
        }

        /// <summary>
        /// Gets, and in derived classes sets, contextual information about the user's status.
        /// </summary>
        public FriendStatus Status
        {
            get { return m_status; }
            protected internal set { m_status = value; }
        }

        /// <summary>
        /// Gets, and in derived classes sets, the type of location information provided by Battle.net.
        /// </summary>
        /// <remarks>
        /// <para>This property provides information indicating how the <see>Location</see> property should be interpreted.</para>
        /// </remarks>
        public FriendLocation LocationType
        {
            get { return m_location; }
            protected internal set { m_location = value; }
        }

        /// <summary>
        /// Gets, and in derived classes sets, the name of the location of the current user.
        /// </summary>
        public string Location
        {
            get { return m_locationName; }
            protected internal set { m_locationName = value; }
        }
    }
}
