﻿using Algo.Runtime.ComponentModel;

namespace Algo.Runtime.Build.AlgorithmDOM.DOM
{
    /// <summary>
    /// Defines identifiers for supported binary operators
    /// </summary>
    public enum AlgorithmBinaryOperatorType
    {
        /// <summary>
        /// Addition operator
        /// </summary>
        [Description("+")]
        Addition = 0,
        /// <summary>
        /// Subtraction operator
        /// </summary>
        [Description("-")]
        Subtraction = 1,
        /// <summary>
        /// Multiplication operator
        /// </summary>
        [Description("*")]
        Multiply = 2,
        /// <summary>
        /// Division operator
        /// </summary>
        [Description("/")]
        Division = 3,
        /// <summary>
        /// Modulus operator
        /// </summary>
        [Description("%")]
        Modulus = 4,
        /// <summary>
        /// Identity not equal operator
        /// </summary>
        [Description("!=")]
        Inequality = 5,
        /// <summary>
        /// Identity equal operator
        /// </summary>
        [Description("==")]
        Equality = 6,
        /// <summary>
        /// Value equal operator
        /// </summary>
        [Description("Equals")]
        Equals = 7,
        /// <summary>
        /// Bitwise or operator
        /// </summary>
        [Description("|")]
        BitwiseOr = 8,
        /// <summary>
        /// Bitwise and operator
        /// </summary>
        [Description("&")]
        BitwiseAnd = 9,
        /// <summary>
        /// Boolean or operator. This represents a short circuiting operator. A short circuiting
        /// operator will evaluate only as many expressions as necessary before returning
        /// a correct value.
        /// </summary>
        [Description("||")]
        LogicalOr = 10,
        /// <summary>
        /// Boolean and operator. This represents a short circuiting operator. A short circuiting
        /// operator will evaluate only as many expressions as necessary before returning
        /// a correct value.
        /// </summary>
        [Description("&&")]
        LogicalAnd = 11,
        /// <summary>
        /// Less than operator
        /// </summary>
        [Description("<")]
        LessThan = 12,
        /// <summary>
        /// Less than or equal operator
        /// </summary>
        [Description("<=")]
        LessThanOrEqual = 13,
        /// <summary>
        /// Greater than operator
        /// </summary>
        [Description(">")]
        GreaterThan = 14,
        /// <summary>
        /// Greater than or equal operator
        /// </summary>
        [Description(">=")]
        GreaterThanOrEqual = 15
    }
}
