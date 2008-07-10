using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp.BattleNet
{
    /// <summary>
    /// Represents a Battle.net chat product.  This class cannot be instantiated.
    /// </summary>
    /// <remarks>
    /// <para>This class is primarily designed to provide information about products supported on Battle.net.  In order to obtain an instance of it,
    /// access one of the static fields.  Equality can also be tested by comparing a user's product to an instance retrieved from the fields exposed
    /// by this class.</para>
    /// </remarks>
    public sealed class Product
    {
        private static Dictionary<string, Product> s_products;

        private string m_prodCode;
        private string m_descriptiveTitle;

        private Product(string productCode, string descriptiveTitle)
        {
            m_prodCode = productCode;
            m_descriptiveTitle = descriptiveTitle;

            if (s_products == null)
                s_products = new Dictionary<string, Product>();

            s_products.Add(productCode, this);
        }

        /// <summary>
        /// The <see>Product</see> object for a telnet chat client.
        /// </summary>
        public static readonly Product ChatClient = new Product("CHAT", Strings.ProdCHAT);

        /// <summary>
        /// The <see>Product</see> object for Starcraft (Retail).
        /// </summary>
        public static readonly Product StarcraftRetail = new Product("STAR", Strings.ProdSTAR);
        /// <summary>
        /// The <see>Product</see> object for Starcraft Shareware.
        /// </summary>
        public static readonly Product StarcraftShareware = new Product("SSHR", Strings.ProdSSHR);
        /// <summary>
        /// The <see>Product</see> object for Starcraft: Brood War.
        /// </summary>
        public static readonly Product StarcraftBroodWar = new Product("SEXP", Strings.ProdSEXP);
        /// <summary>
        /// The <see>Product</see> object for Japan Starcraft.
        /// </summary>
        public static readonly Product JapanStarcraft = new Product("JSTR", Strings.ProdJSTR);

        /// <summary>
        /// The <see>Product</see> object for Warcraft II: Battle.net Edition.
        /// </summary>
        public static readonly Product Warcraft2BNE = new Product("W2BN", Strings.ProdW2BN);

        /// <summary>
        /// The <see>Product</see> object for Diablo 2 Shareware.
        /// </summary>
        public static readonly Product Diablo2Shareware = new Product("D2SH", Strings.ProdD2SH);
        /// <summary>
        /// The <see>Product</see> object for Diablo 2 (Retail).
        /// </summary>
        public static readonly Product Diablo2Retail = new Product("D2DV", Strings.ProdD2DV);
        /// <summary>
        /// The <see>Product</see> object for Diablo 2: The Lord of Destruction.
        /// </summary>
        public static readonly Product Diablo2Expansion = new Product("D2XP", Strings.ProdD2XP);
        
        /// <summary>
        /// The <see>Product</see> object for Warcraft 3: The Reign of Chaos.
        /// </summary>
        public static readonly Product Warcraft3Retail = new Product("WAR3", Strings.ProdWAR3);

        /// <summary>
        /// The <see>Product</see> object for Warcraft 3: The Frozen Throne.
        /// </summary>
        public static readonly Product Warcraft3Expansion = new Product("W3XP", Strings.ProdW3XP);

        /// <summary>
        /// The <see>Product</see> object that represents any product unrecognized by BN#.
        /// </summary>
        public static readonly Product UnknownProduct = new Product("UNKN", Strings.ProdUNKN);

        /// <summary>
        /// Gets the <see>Product</see> associated with the specified product code.
        /// </summary>
        /// <param name="productCode">The four-character product code to check.</param>
        /// <returns>A <see>Product</see> object associated with the product code if it is found; otherwise <see langword="null" />.</returns>
        public static Product GetByProductCode(string productCode)
        {
            if (s_products.ContainsKey(productCode))
                return s_products[productCode];
            return null;
        }

        /// <summary>
        /// Gets the product code for this Product.
        /// </summary>
        public string ProductCode
        {
            get { return m_prodCode; }
        }

        /// <summary>
        /// Gets the name of this product.
        /// </summary>
        /// <remarks>
        /// <para>If localized resources exist for the current language, they are retrieved.</para>
        /// </remarks>
        public string Name
        {
            get { return m_descriptiveTitle; }
        }

        /// <summary>
        /// Gets an array of all supported products.
        /// </summary>
        /// <returns>An array of recognized products.</returns>
        public static Product[] GetAllProducts()
        {
            Product[] result = new Product[s_products.Count];
            s_products.Values.CopyTo(result, 0);
            return result;
        }
    }
}
