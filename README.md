# codekata-rule-engine

Implementation of a rule engine for [CodeKata09](http://codekata.com/kata/kata09-back-to-the-checkout/) using C#.

## Rule Engine

This repository aims to reimplement a simple rule engine by reading the configuration from a JSON file into a tree and then calculating the final value. If it is an expression, the expected value should be `true` or `false`, determining if the rule is matching or not. If it is to calculate the rewards/discounts/price reduction, the result should be a `decimal` number representing the reduction price.

The rule Item A, Unit Price 50, Special Price: 3 for 130, is explained as:

```
rule:
    if: CartItem.Quantity / 3 > 0
    then: CartItem.Quantity / 3 * SpecialPrice
    dynamic data: CartItem = CartItems["A"]
```

A tree data structure is defined to hold the rule above:

```csharp
var r = new Rule
{
    Data = [
        new DataPointDefinition
        {
            Name = "CartItem",
            DataType = "CART_ITEM",
            IsRequired = true,
            Params = [
                        new DataPointParam {
                            Key = "SKU",
                            DataType = "STRING",
                            Value = sku
                        }
                    ]
        };
    ],
    Condition = new BlockNode
    {
        Children = [
            // Items["A"].Quantity / 3 > 0
            BlockDynamicData.NewInt("CartItem.Quantity"),
            new BlockOperator { Operator = OPERATORS.DIVIDE },
            BlockData.NewInt(3),
            new BlockOperator { Operator = OPERATORS.GREATER_THAN },
            BlockData.NewInt(0)
        ]
    },
    Action = new RuleAction
    {
        ActionType = "PRICE_REDUCTION",
        ComputedValue = new BlockNode
        {
            Children = [
                //Items["A].Quantity / 3 * SpecialPrice
                BlockDynamicData.NewInt("CartItem.Quantity"),
                new BlockOperator { Operator = OPERATORS.DIVIDE },
                BlockData.NewInt(3),
                new BlockOperator { Operator = OPERATORS.MULTIPLY },
                BlockData.NewInt(20),
            ]
        }
    }
};
```

- `DataPointDefinition`: Define how to access certain data, in this case: access the scanned item to get Quantity value for the special offers.
- `BlockDynamicData`: Accessing data from the dynamic data `CartItem.Quantity`. The computing process will convert this data to a constant value `BlockData`
- `BlockData`: A constants.
- `BlockOperator`: Define an operator such as `+`, `-`, or `==`.
- `BlockNode`: Similar to a `()` that can hold other blocks.

How does this rule engine work?

- Query dynamic data from the definitions
- Travel through the expression tree to compute the result
  - The operator with higher precedence should be executed first (for example The `*` operator is computed before the `+` operator and the `==` operator).
  - Convert dynamic data into constant data
  - Perform computation for each operation until there is only one constant left.

## Another approach using LINQ dynamic expression builder

While implementing the rule engine from scratch could work for simple cases, it is impossible to cover more complex cases. As such, the adaptation of tools such as the rule engine from the LINQ dynamic expression builder can help to execute those with more complex requirements. See this [Rule Engine's LINQ dynamic builder](https://microsoft.github.io/RulesEngine/)

## Back End NET Core API

On the backend side, Clean architecture is applied to provide enterprise application experience and leverage the power of Clean architecture to extend and implement new features and functionalities.

### Architecture

| Layer         | Project                          |                                  |
| ------------- | -------------------------------- | -------------------------------- |
| Domain        | Checkout.Domain                  | Entities definitions             |
| Application   | Checkout.Core                    | Business process                 |
| Application   | Checkout.Core.Tests              | Unit tests                       |
| Application   | Checkout.RuleEngine              | Rule Engine implementation       |
| Application   | Checkout.RuleEngine.Tests        | Unit tests                       |
| Infrastruture | Checkout.Persistentce            | Database access                  |
| Infrastruture | Checkout.RuleEngine.Persistentce | Rule Engine configuration loader |
| Presentation  | CheckoutConsoleApplication       | Execute checkout process         |

### Technologies

- [ASP.NET Core 8](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
- xUnit and Moq for unit testings

## References

- The GitHub repository of [a Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- test git
