using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// Gets information about the user's product when the product is otherwise unrecognized.
    /// </summary>
    public class DefaultStats : UserStats
    {
        #region fields
        private ClassicProduct _prod;
        private string _lit;
        #endregion

        internal DefaultStats(string productID, byte[] literal)
        {
            Debug.Assert(literal != null);

            _prod = ClassicProduct.GetByProductCode(productID);
            if (_prod == null)
                _prod = ClassicProduct.UnknownProduct;

            _lit = Encoding.ASCII.GetString(literal);
        }

        /// <summary>
        /// Gets the <see cref="BNSharp.BattleNet.Product">Product</see> with which the user is currently logged on.
        /// </summary>
        public override ClassicProduct Product
        {
            get
            {
                return _prod;
            }
        }

        /// <summary>
        /// Gets the literal statstring text.
        /// </summary>
        public override string LiteralText
        {
            get { return _lit; }
        }
    }
}
