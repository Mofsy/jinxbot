using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public sealed class ClassicProduct : IEquatable<ClassicProduct>, IEquatable<string>
    {
        private static Dictionary<string, ClassicProduct> s_products;

        #region fields
        private string _prodCode;
        private string _descriptiveTitle;

        private bool _canConnect, _needs2Keys, _needsLockdown, _usesUdp;
        #endregion

        #region Constructors
        private ClassicProduct(string productCode, string descriptiveTitle)
        {
            _prodCode = productCode;
            _descriptiveTitle = descriptiveTitle;

            if (s_products == null)
                s_products = new Dictionary<string, ClassicProduct>();

            s_products.Add(productCode, this);
        }

        private ClassicProduct(string productCode, string descriptiveTitle, bool canConnect, bool needs2Keys)
            : this(productCode, descriptiveTitle)
        {
            _canConnect = canConnect;
            _needs2Keys = needs2Keys;
        }

        private ClassicProduct(string productCode, string descriptiveTitle, bool canConnect, bool needs2Keys, bool needsLockdown)
            : this(productCode, descriptiveTitle, canConnect, needs2Keys)
        {
            _needsLockdown = needsLockdown;
        }

        private ClassicProduct(string productCode, string descriptiveTitle, bool canConnect, bool needs2Keys, bool needsLockdown, bool needsUdp)
            : this(productCode, descriptiveTitle, canConnect, needs2Keys, needsLockdown)
        {
            _usesUdp = needsUdp;
        }

        #endregion

        #region Known products
        /// <summary>
        /// The <see>Product</see> object for a telnet chat client.
        /// </summary>
        public static readonly ClassicProduct ChatClient = new ClassicProduct("CHAT", "Chat client");

        /// <summary>
        /// The <see>Product</see> object for Starcraft (Retail).
        /// </summary>
        public static readonly ClassicProduct StarcraftRetail = new ClassicProduct("STAR", "Starcraft (Retail)", true, false, true, true);
        /// <summary>
        /// The <see>Product</see> object for Starcraft Shareware.
        /// </summary>
        public static readonly ClassicProduct StarcraftShareware = new ClassicProduct("SSHR", "Starcraft: Shareware");
        /// <summary>
        /// The <see>Product</see> object for Starcraft: Brood War.
        /// </summary>
        public static readonly ClassicProduct StarcraftBroodWar = new ClassicProduct("SEXP", "Starcraft: Brood War", true, false, true, true);
        /// <summary>
        /// The <see>Product</see> object for Japan Starcraft.
        /// </summary>
        public static readonly ClassicProduct JapanStarcraft = new ClassicProduct("JSTR", "Japan Starcraft", true, false, false, true);

        /// <summary>
        /// The <see>Product</see> object for Warcraft II: Battle.net Edition.
        /// </summary>
        public static readonly ClassicProduct Warcraft2BNE = new ClassicProduct("W2BN", "Warcraft II: Battle.net Edition", true, false, true, true);

        /// <summary>
        /// The <see>Product</see> object for Diablo (Retail).
        /// </summary>
        public static readonly ClassicProduct DiabloRetail = new ClassicProduct("DRTL", "Diablo Classic (Retail)");
        /// <summary>
        /// The <see>Product</see> object for Diablo (Shareware).
        /// </summary>
        public static readonly ClassicProduct DiabloShareware = new ClassicProduct("DSHR", "Diablo Classic (Shareware)");

        /// <summary>
        /// The <see>Product</see> object for Diablo 2 Shareware.
        /// </summary>
        public static readonly ClassicProduct Diablo2Shareware = new ClassicProduct("D2SH", "Diablo II: Shareware");
        /// <summary>
        /// The <see>Product</see> object for Diablo 2 (Retail).
        /// </summary>
        public static readonly ClassicProduct Diablo2Retail = new ClassicProduct("D2DV", "Diablo II (Retail)", true, false);
        /// <summary>
        /// The <see>Product</see> object for Diablo 2: The Lord of Destruction.
        /// </summary>
        public static readonly ClassicProduct Diablo2Expansion = new ClassicProduct("D2XP", "Diablo II: The Lord of Destruction", true, true);

        /// <summary>
        /// The <see>Product</see> object for Warcraft 3: The Reign of Chaos.
        /// </summary>
        public static readonly ClassicProduct Warcraft3Retail = new ClassicProduct("WAR3", "Warcraft III: The Reign of Chaos", true, false);

        /// <summary>
        /// The <see>Product</see> object for Warcraft 3: The Frozen Throne.
        /// </summary>
        public static readonly ClassicProduct Warcraft3Expansion = new ClassicProduct("W3XP", "Warcraft III: The Frozen Throne", true, true);

        /// <summary>
        /// The <see>Product</see> object that represents any product unrecognized by BN#.
        /// </summary>
        public static readonly ClassicProduct UnknownProduct = new ClassicProduct("UNKN", "An Unknown Product or Client");

        #endregion

        /// <summary>
        /// Gets the <see>Product</see> associated with the specified product code.
        /// </summary>
        /// <param name="productCode">The four-character product code to check.</param>
        /// <returns>A <see>Product</see> object associated with the product code if it is found; otherwise <see langword="null" />.</returns>
        public static ClassicProduct GetByProductCode(string productCode)
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
            get { return _prodCode; }
        }

        /// <summary>
        /// Gets the name of this product.
        /// </summary>
        /// <remarks>
        /// <para>If localized resources exist for the current language, they are retrieved.</para>
        /// </remarks>
        public string Name
        {
            get { return _descriptiveTitle; }
        }

        /// <summary>
        /// Gets an array of all supported products.
        /// </summary>
        /// <returns>An array of recognized products.</returns>
        public static IEnumerable<ClassicProduct> GetAllProducts()
        {
            return s_products.Values.AsEnumerable();
        }

        /// <summary>
        /// Gets whether this product can be used to connect via BN#.
        /// </summary>
        public bool CanConnect
        {
            get { return _canConnect; }
        }

        internal bool NeedsTwoKeys
        {
            get { return _needs2Keys; }
        }

        internal bool NeedsLockdown
        {
            get { return _needsLockdown; }
        }

        internal bool UsesUdpPing
        {
            get { return _usesUdp; }
        }

        #region IEquatable<string> Members

        /// <summary>
        /// Determines whether the specified product's product code matches the specified product code.
        /// </summary>
        /// <param name="other">The product code to test.</param>
        /// <returns><see langword="true" /> if this product matches the tested product code; otherwise <see langword="false" />.</returns>
        public bool Equals(string other)
        {
            return _prodCode.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region IEquatable<Product> Members
        /// <summary>
        /// Determines whether the specified product and this product represent the same Battle.net client.
        /// </summary>
        /// <param name="other">The client to test.</param>
        /// <returns><see langword="true" /> if the products match; otherwise <see langword="false" />.</returns>
        public bool Equals(ClassicProduct other)
        {
            return _prodCode.Equals(other._prodCode, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
