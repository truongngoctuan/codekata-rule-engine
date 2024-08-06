# codekata-rule-engine

Implementation of a rule engine for [CodeKata09](http://codekata.com/kata/kata09-back-to-the-checkout/) using C#.

This repository aims to reimplement a simple rule engine by reading the configuration from a JSON file into a tree and then calculating the final value. If it is an expression, the expected value should be `true` or `false`, determining if the rule is matching or not. If it is to calculate the rewards/discounts/price reduction, the result should be a `decimal` number representing the reduction price.

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
- [JSON file as a simple database](https://github.com/ttu/json-flatfile-datastore) in the infrastructure/persistence layer
- [AutoMapper](https://automapper.org/)

## References

- The github repository of [a Clean architecture](https://github.com/jasontaylordev/CleanArchitecture)
