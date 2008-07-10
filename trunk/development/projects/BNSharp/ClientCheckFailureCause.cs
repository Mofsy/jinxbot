using System;
using System.Collections.Generic;
using System.Text;

namespace BNSharp
{
    /// <summary>
    /// Specifies the causes of client versioning failure reported by Battle.net.
    /// </summary>
    public enum ClientCheckFailureCause
    {
        /// <summary>
        /// Indicates that client checks passed.
        /// </summary>
        Passed = 0,
        /// <summary>
        /// Indicates that the client should upgrade.
        /// </summary>
        OldVersion = 0x100,
        /// <summary>
        /// Indicates that the version checksum was invalid.
        /// </summary>
        InvalidVersion = 0x101,
        /// <summary>
        /// Indicates that the client is using a newer version than is currently supported.
        /// </summary>
        NewerVersion = 0x102,
        /// <summary>
        /// The CD key was invalid.
        /// </summary>
        InvalidCdKey = 0x200,
        /// <summary>
        /// The CD key is already in use.
        /// </summary>
        CdKeyInUse = 0x201,
        /// <summary>
        /// The CD key has been banned.
        /// </summary>
        BannedCdKey = 0x202,
        /// <summary>
        /// The CD key was for the wrong product.
        /// </summary>
        WrongProduct = 0x203,
        /// <summary>
        /// Indicates that the expansion CD key was invalid.
        /// </summary>
        InvalidExpCdKey = 0x210,
        /// <summary>
        /// Indicates that the expansion CD key is already in use.
        /// </summary>
        ExpCdKeyInUse = 0x211,
        /// <summary>
        /// Indicates that the expansion CD key was banned.
        /// </summary>
        BannedExpCdKey = 0x212,
        /// <summary>
        /// Indicates that the expansion CD key was for the wrong product.
        /// </summary>
        WrongExpProduct = 0x213,
    }
}
