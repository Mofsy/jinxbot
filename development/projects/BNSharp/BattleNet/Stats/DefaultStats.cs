using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// Gets information about the user's product when the product is otherwise unrecognized.
    /// </summary>
#if !NET_2_ONLY
    [DataContract]
#endif
    public class DefaultStats : UserStats
    {
        #region fields
        [DataMember(Name = "Product")]
        private Product m_prod; 
        #endregion

        internal DefaultStats(string productID)
        {
            m_prod = Product.GetByProductCode(productID);
            if (m_prod == null)
                m_prod = Product.UnknownProduct;

        }

        /// <summary>
        /// Gets the <see cref="BNSharp.BattleNet.Product">Product</see> with which the user is currently logged on.
        /// </summary>
        public override Product Product
        {
            get
            {
                return m_prod;
            } 
        }
    }
}
