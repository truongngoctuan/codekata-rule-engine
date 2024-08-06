using System;
using System.Reflection.Metadata.Ecma335;
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
        Data = [
          new DataPointDefinition {
            Name = "CartItem",
            DataType = "CART_ITEM",
            Params = [
              new DataPointParam {
                Key = "SKU",
                DataType = "STRING",
                Value = "A"
              }

            ]
          }
        ],
        Condition = new BlockNode
        {
          Children = [
            new BlockData { Value = new DataPoint { Value = "CartItem.Quantity", DataType = "INT" } },
            new BlockLeafOperator { Operator = "EQUALS" },
            new BlockData { Value = new DataPoint { Value = "3", DataType = "INT" } },
          ]
        },
        Action = new RuleAction
        {
          ActionType = "PRICE_REDUCTION",
          // (int)(Items["A].UnitPrice / 3) * SpecialPrice
          ComputedValue = new BlockNode
          {
            Children = [
              // (int)(Items["A].UnitPrice / 3)
              new BlockCast {
                CastDataType = "INT",
                Children = [
                  // Items["A].UnitPrice / 3
                  new BlockData { Value = new DataPoint { Value = "CartItem.UnitPrice", DataType = "INT" } },
                  new BlockLeafOperator { Operator = "DIVIDE" },
                  new BlockData { Value = new DataPoint { Value = "3", DataType = "INT" } },
                ]
              },
              new BlockLeafOperator { Operator = "MULTIPLY" },
              new BlockData { Value = new DataPoint { Value = "3", DataType = "INT" } },
            ]
          }
        }
      };

    }
  }


  // //CartItem
  // public class CartItem
  // {
  //   public string SKU { get; set; }
  //   public decimal UnitPrice { get; set; }
  //   public uint Quantity { get; set; }
  //   public virtual decimal Price
  //   {
  //     get
  //     {
  //       return UnitPrice * Quantity;
  //     }
  //   }
  // }

  // public class CartItemWithPriceReduction : CartItem
  // {
  //   public decimal PriceReduction { get; set; }
  //   public override decimal Price => base.Price - PriceReduction;
  // }

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

  public class BlockLeafOperator : BlockBase
  {
    public string Operator { get; set; }
    public static BlockLeafOperator NewAdd() => new BlockLeafOperator { Operator = OPERATORS.ADD };
  }

  public class BlockData : BlockBase
  {
    public DataPoint Value { get; set; }
    public static BlockData NewInt(int value) => new BlockData { Value = DataPoint.NewInt(value) };
    public static BlockData NewBool(bool value) => new BlockData { Value = DataPoint.NewBool(value) };
    public static BlockData NewDecimal(decimal value) => new BlockData { Value = DataPoint.NewDecimal(value) };
  }

  public class BlockDynamicData : BlockBase
  {
    public DataPoint Value { get; set; }
    public static BlockDynamicData NewInt(string accesstor) => new BlockDynamicData { Value = new DataPoint { Value = accesstor, DataType = DATA_TYPE.INT } };
  }

  public class DataPoint
  {
    public string Value { get; set; }
    public string DataType { get; set; }
    public static DataPoint NewInt(int value) => new DataPoint { DataType = DATA_TYPE.INT, Value = value.ToString() };
    public static DataPoint NewBool(bool value) => new DataPoint { DataType = DATA_TYPE.BOOL, Value = value.ToString() };
    public static DataPoint NewDecimal(decimal value) => new DataPoint { DataType = DATA_TYPE.DECIMAL, Value = value.ToString() };
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
