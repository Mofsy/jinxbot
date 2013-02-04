using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Globalization;

namespace JinxBot.Controls
{
    /// <summary>
    /// Provides methods to validate arguments and raises exceptions when arguments are not within the valid range.
    /// </summary>
    internal static class Contract
    {
        /// <summary>
        /// Throws an exception if a string object is passed that is null or zero-length.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown if <c>s</c> is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <c>s</c> is a zero-length string.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireStringWithValue(string s, string paramName)
        {
            if (object.ReferenceEquals(null, s))
                throw new ArgumentNullException(paramName, "Parameter cannot be null.");
            if (s.Length == 0)
                throw new ArgumentOutOfRangeException(paramName, "Parameter cannot be zero-length.");
        }

        /// <summary>
        /// Throws an exception if an object is null.
        /// </summary>
        /// <param name="o">The object to test.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown if <c>o</c> is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireInstance(object o, string paramName)
        {
            if (object.ReferenceEquals(null, o))
                throw new ArgumentNullException(paramName, "Parameter cannot be null.");
        }

        /// <summary>
        /// Throws an exception if a database connection is null or not open.
        /// </summary>
        /// <param name="con">The connection object to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown if <c>con</c> is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="InvalidOperationException">Thrown if <c>con.State</c> is not set to <see cref="ConnectionState">ConnectionState</see><b>.Open</b>.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireOpenConnection(IDbConnection con, string paramName)
        {
            if (object.ReferenceEquals(null, con))
                throw new ArgumentNullException(paramName, "Database connection cannot be null.");

            if (con.State != ConnectionState.Open)
                throw new InvalidOperationException("Database must be open.");
        }

        /// <summary>
        /// Throws an exception if a buffer is not exactly a given length or if it is null.
        /// </summary>
        /// <param name="buffer">The buffer to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <param name="exactCount">The exact length the buffer is expected to be.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireBytes(byte[] buffer, string paramName, int exactCount)
        {
            ValidateContractParam(exactCount >= 0, "Specify a non-negative buffer size.");

            RequireInstance(buffer, paramName);

            if (buffer.Length != exactCount)
                throw new ArgumentOutOfRangeException(paramName, string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' must be exactly {1} byte(s) long.", paramName, exactCount));
        }

        /// <summary>
        /// Throws an exception if a buffer is not within a range of lengths or if it is null.
        /// </summary>
        /// <param name="buffer">The buffer to check.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <param name="min">The minimum length of the buffer.</param>
        /// <param name="max">The maximum length of the buffer.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireBytes(byte[] buffer, string paramName, int min, int max)
        {
            ValidateContractParam(min < max && min >= 0, "Specify non-negative values for min and max and a greater value for max.");

            RequireInstance(buffer, paramName);

            if (buffer.Length > max || buffer.Length < min)
                throw new ArgumentOutOfRangeException(paramName,
                    string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' must be between {1} and {2} bytes long.", paramName, min, max));
        }

        /// <summary>
        /// Throws an exception if the <c>test</c> value is not <c>true</c>.
        /// </summary>
        /// <param name="test">The expression to test.</param>
        /// <param name="failureMessage">The message to display if the test fails.</param>
        /// <exception cref="InvalidOperationException">Thrown if <c>test</c> evaluates to <b>false</b>.</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Assert(bool test, string failureMessage)
        {
            if (!test)
                throw new InvalidOperationException(failureMessage);
        }

        /// <summary>
        /// Throws an exception if the the <c>test</c> value is not <c>true</c>.
        /// </summary>
        /// <param name="test">The expression to test.</param>
        /// <param name="failureMessage">The message to display if the test fails.</param>
        /// <param name="paramName">The name of the parameter being tested.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void Assert(bool test, string failureMessage, string paramName)
        {
            if (!test)
                throw new ArgumentException(failureMessage, paramName);
        }

        /// <summary>
        /// Throws an exception if a string is null, is zero-length, or exceeds a specific length.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <param name="paramName">The name of the parameter to check.</param>
        /// <param name="maxLength">The maximum length boundary.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void MaxStringLength(string s, string paramName, int maxLength)
        {
            ValidateContractParam(maxLength > 0, "Cannot specify a max length <= 0.");

            RequireStringWithValue(s, paramName);

            if (s.Length > maxLength)
                throw new ArgumentOutOfRangeException(paramName, s,
                    string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' can be no longer than {1} character(s).", paramName, maxLength));
        }

        /// <summary>
        /// Throws an exception if an object is not of a specified type.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="type">The type to compare against.</param>
        /// <param name="paramName">The name of the parameter to check.</param>
        /// <exception cref="InvalidCastException">Thrown if <c>obj</c> is not assignable without an explicit cast to a variable of type <c>type</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <c>obj</c> is <b>null</b> (<b>Nothing</b> in Visual Basic).</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireType(object obj, Type type, string paramName)
        {
            ValidateContractParam(type != null, "Specify a Type.");

            RequireInstance(obj, paramName);

            if (!type.IsAssignableFrom(obj.GetType()))
                throw new InvalidCastException(
                    string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' must be assignable to type '{1}'.  It is of type '{2}', which is incompatible.", paramName, type.FullName, obj.GetType().FullName));
        }

        /// <summary>
        /// Throws an exception if a value is not part of a given enumeration.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="type">The type to compare against.</param>
        /// <param name="paramName">The name of the parameter to check.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireEnumValue(object value, Type type, string paramName)
        {
            ValidateContractParam(type != null, "Specify a Type.");
            ValidateContractParam(type.IsEnum, "Specify an Enum type.");

            RequireType(value, type, paramName);
            if (!Enum.IsDefined(type, value))
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' must be defined by enum type '{1}'.", paramName, type.FullName));
        }

        /// <summary>
        /// Throws an exception if a value is not part of a given enumeration.
        /// </summary>
        /// <remarks>
        /// <para>This call is moderately slow, because it uses reflection to retrieve the values of the enumeration fields.</para>
        /// </remarks>
        /// <param name="value">The value to check.</param>
        /// <param name="type">The type to compare against.</param>
        /// <param name="paramName">The name of the parameter to check.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void RequireEnumFlagsValue(object value, Type type, string paramName)
        {
            ValidateContractParam(type != null, "Specify a Type.");
            ValidateContractParam(type.IsEnum, "Specify an Enum type.");

            RequireType(value, type, paramName);
            ulong validValues = 0;
            ulong testValue = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            FieldInfo[] enumFields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in enumFields)
            {
                validValues |= Convert.ToUInt64(fi.GetValue(null), CultureInfo.InvariantCulture);
            }
            // Here's the math that makes this work:
            // consider an 8-bit enum:
            //              VALID TEST           INVALID TEST
            // Enum val:    11010110             11010110
            // Test val:  ^ 01010110             01010111 (invalid test value)
            //              ========             ========
            //              10000000             10000001
            // Enum val:  | 11010110             11010110
            //              ========             ========
            // Enum val:    11010110             11010111
            // After XOR and OR we should have the same result:
            // enum value ^ test value | enum value == enum value
            // in a test with an invalid test value, the math would not end up equalling the enum value.

            ulong test = validValues ^ testValue;
            test |= validValues;
            if (test != validValues)
                throw new ArgumentOutOfRangeException(paramName, value,
                    string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' was of type '{1}' but specified bitmask values that were not part of this type.", paramName, type.FullName));
        }

        /// <summary>
        /// Throws an exception if an array is null or has two many items.
        /// </summary>
        /// <param name="array">The array to bounds-check.</param>
        /// <param name="paramName">The name of the array.</param>
        /// <param name="maxItemCount">The maximum number of items.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void MaxArrayItems(Array array, string paramName, int maxItemCount)
        {
            ValidateContractParam(maxItemCount > 0, "Cannot specify a max item count <= 0.");

            RequireInstance(array, "array");

            if (array.Length > maxItemCount)
                throw new ArgumentOutOfRangeException(paramName, array, string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' must be no more than '{1}' items in length.", paramName, maxItemCount));
        }

        private static void ValidateContractParam(bool test, string message)
        {
            if (!test)
                throw new InvalidProgramException(string.Format(CultureInfo.InvariantCulture, "Contract parameter failed; '{0}'.", message));
        }
    }

}
