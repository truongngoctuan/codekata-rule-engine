using System;
using System.Runtime.ConstrainedExecution;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Entities
{
  public class Rule
  {
    public DataPointDefinition[] Data { get; set; }
    public BlockBase Condition { get; set; }
    public RuleAction Action { get; set; }
    // Item A
    // Unit Price 50
    //  Special Price: 3 for 130

    // rule:
    // if: Items["A"].Quantity / 3 > 0
    // simpler version: if: Items["A"].Quantity == 3
    // then: (int)(Items["A].UnitPrice / 3) * SpecialPrice
    // data: SpecialPrice = -20
    // dynamic data:
    //    Items Array of CartItem
    //    Array of Item details

    void SampleRule()
    {
      var r = new Rule
      {
        Data = new DataPointDefinition[] {
          new DataPointDefinition {
            Name = "CartItem",
            DataType = "CART_ITEM",
            Params = new DataPointParam[] {
              new DataPointParam {
                Key = "SKU",
                DataType = "STRING",
                Value = "A"
              }

            }
          }
        },
        Condition = new BlockNode
        {
          DataType = "BLOCK",
          Children = new BlockBase[] {
            new BlockLeaf { DataType = "DYNAMIC_DATA", Value = new DataPoint { Value = "CartItem.Quantity", DataType = "INT" } },
            new BlockLeaf { DataType = "OPERATOR_EQUALS" },
            new BlockLeaf { DataType = "FIXED_DATA", Value = new DataPoint { Value = "3", DataType = "INT" } },
          }
        },
        Action = new RuleAction
        {
          ActionType = "PRICE_REDUCTION",
          // (int)(Items["A].UnitPrice / 3) * SpecialPrice
          ComputedValue = new BlockNode
          {
            Children = new BlockBase[] {
              // (int)(Items["A].UnitPrice / 3)
              new BlockCast {
                DataType = "BLOCK_CAST",
                CastDataType = "INT",
                Children = new BlockBase[] {
                  // Items["A].UnitPrice / 3
                  new BlockLeaf { DataType = "DYNAMIC_DATA", Value = new DataPoint { Value = "CartItem.UnitPrice", DataType = "INT" } },
                  new BlockLeaf { DataType = "OPERATOR_DIVIDE" },
                  new BlockLeaf { DataType = "FIXED_DATA", Value = new DataPoint { Value = "3", DataType = "INT" } },
                }
              },
              new BlockLeaf { DataType = "OPERATOR_MULTIPLY" },
              new BlockLeaf { DataType = "FIXED_DATA", Value = new DataPoint { Value = "3", DataType = "INT" } },
            }
          }
        }
      };

    }
  }


  //CartItem
  public class CartItem
  {
    public string SKU { get; set; }
    public decimal UnitPrice { get; set; }
    public uint Quantity { get; set; }
    public virtual decimal Price
    {
      get
      {
        return UnitPrice * Quantity;
      }
    }
  }

  public class CartItemWithPriceReduction : CartItem
  {
    public decimal PriceReduction { get; set; }
    public override decimal Price => base.Price - PriceReduction;
  }

  public class Item
  {
    public string SKU { get; set; }
    public string Name { get; set; }
  }


  public class DataPointDefinition
  {
    public string Name { get; set; }
    public string DataType { get; set; }
    public DataPointParam[] Params { get; set; }

    // void SampleRule()
    // {
    //   var r = new Rule
    //   {
    //     Data = new DataPoint[] {
    //       DataPoint {
    //         Key = "CART_ITEMS",
    //         DataType = "ARRAY_OF"
    //         SubTypes = new DataPoint[] {
    //           DataPoint {
    //             Key = "CART_ITEM",
    //           }
    //         }
    //       }
    //     }
    //   }
    //   }

  }

  public class DataPointParam
  {
    public string Key { get; set; }
    public string DataType { get; set; }
    public string Value { get; set; }
  }

  public class BlockBase
  {
    public string DataType { get; set; }
  }

  public class BlockNode : BlockBase
  {
    public BlockBase[] Children { get; set; }
  }

  public class BlockLeaf : BlockBase
  {
    public DataPoint Value { get; set; }
  }

  public class DataPoint
  {
    public string Value { get; set; }
    public string DataType { get; set; }
  }

  public class BlockStatement : BlockBase
  {
    public BlockBase Left { get; set; }
    public BlockBase Operator { get; set; }
    public BlockBase Right { get; set; }
  }

  public class BlockCast : BlockNode
  {
    public string CastDataType { get; set; }
  }

  public class RuleAction
  {
    public string ActionType { get; set; }
    public BlockBase ComputedValue { get; set; }
  }

}
