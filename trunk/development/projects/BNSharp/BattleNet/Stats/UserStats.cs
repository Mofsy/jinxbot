using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using BNSharp.MBNCSUtil;

namespace BNSharp.BattleNet.Stats
{
    /// <summary>
    /// When implemented in a derived class, provides statistical information about a user.
    /// </summary>
    public abstract class UserStats
    {
        /// <summary>
        /// Parses a user statstring and returns an object representing the stats in a meaningful way.
        /// </summary>
        /// <param name="userName">The name of the user whose stats are being examined.</param>
        /// <param name="statsData">The stats of the user.</param>
        /// <returns>An instance of a class derived from <see>UserStats</see> based on the user's 
        /// <see cref="BNSharp.BattleNet.Product">Product</see>.  To check the product, check the 
        /// <see cref="UserStats.Product">Product property</see>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="userName"/> or <paramref name="statsData"/>
        /// are <see langword="null" />.</exception>
        public static UserStats Parse(string userName, byte[] statsData)
        {
            DataReader dr = new DataReader(statsData);
            string productCode = dr.ReadDwordString(0);
            UserStats result = null;
            switch (productCode)
            {
                case "STAR":
                case "SEXP":
                case "SSHR":
                case "JSTR":
                case "W2BN":
                case "DSHR":
                case "DRTL":
                    result = new StarcraftStats(statsData);
                    break;
                case "D2DV":
                case "D2XP":
                    result = new Diablo2Stats(userName, statsData);
                    break;
                case "WAR3":
                case "W3XP":
                    result = new Warcraft3Stats(statsData);
                    break;
                default:
                    result = new DefaultStats(productCode);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Creates a default statistics object with information only about the product.
        /// </summary>
        /// <param name="product">The product for which to create information.</param>
        /// <returns>An instance of <see>UserStats</see> with only product information.</returns>
        public static UserStats CreateDefault(Product product)
        {
            return new DefaultStats(product.ProductCode);
        }

        /// <summary>
        /// When implemented in a derived class, gets the product with which the user is logged on.
        /// </summary>
        public abstract Product Product
        {
            get;
        }
    }
}
