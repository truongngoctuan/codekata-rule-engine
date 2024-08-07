using System.Collections.Generic;

namespace Checkout.RuleEngine;

public static class OPERATORS
{
    // Multiplicative
    public const string MULTIPLY = "MULTIPLY";
    public const string DIVIDE = "DIVIDE";

    // Additive
    public const string ADD = "ADD";
    public const string MINUS = "MINUS";

    // Relational
    public const string EQUALS = "EQUALS";
    public const string GREATER_THAN = "GREATER_THAN";
    public const string GREATER_OR_EQUALS = "GREATER_OR_EQUALS";
    public const string SMALLER_THAN = "SMALLER_THAN";
    public const string SMALLER_OR_EQUALS = "SMALLER_OR_EQUALS";

    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/#operator-precedence
    public static readonly Dictionary<string, ushort> Operators_Precedence = new Dictionary<string, ushort>
    {
        // Multiplicative
        {OPERATORS.MULTIPLY, 5},
        {OPERATORS.DIVIDE, 5},

        // Additive
        {OPERATORS.ADD, 6},
        {OPERATORS.MINUS, 6},

        // Relational
        {OPERATORS.SMALLER_OR_EQUALS, 8},
        {OPERATORS.SMALLER_THAN, 8},
        {OPERATORS.GREATER_OR_EQUALS, 8},
        {OPERATORS.GREATER_THAN, 8},

        // Equality
        {OPERATORS.EQUALS, 9},
    };
}

public static class DATA_TYPE
{
  public const string INT = "INT";
  public const string BOOL = "BOOL";
  public const string DECIMAL = "DECIMAL";
}

public static class BOOL_DATA
{
  public const string TRUE = "True";
  public const string FALSE = "False";
}
