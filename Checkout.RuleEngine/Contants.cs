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
