using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine;

public class RuleEngineService : IRuleEngineService
{
    /// <summary>
    /// Given a tree-based nodes, calculate the value of the statement
    /// For Example: Items["A"].Quantity / 3 > 0 should return `DataPoint { BOOL, true}`
    /// (int)(Items["A"].UnitPrice / 3) * SpecialPrice should return `DataPoint {DECIMAL, 3}`
    /// </summary>
    /// <param name="root"></param>
    /// <param name="datas"></param>
    /// <returns></returns>
    public DataPoint Compute(BlockBase root, Dictionary<string, dynamic> datas)
    {
        // stop condition
        if (root is BlockData)
        {
            var blockLeaf = root as BlockData;
            return blockLeaf.Value;
        }

        if (root is BlockDynamicData)
        {
            return toFixedData(root as BlockDynamicData, datas);
        }

        if (root is BlockNode)
        {
            var blockNode = root as BlockNode;
            if (blockNode.Children.Count() == 1)
            {
                return Compute(blockNode.Children[0], datas);
            }

            BlockData left = null;
            BlockLeafOperator opt = null;
            for (int i = 0; i < blockNode.Children.Count(); i++)
            {
                var currentChildNode = blockNode.Children[i];
                //convert dynamic data into fixed data
                if (currentChildNode is BlockDynamicData)
                {
                    currentChildNode = new BlockData
                    {
                        Value = toFixedData(currentChildNode as BlockDynamicData, datas)
                    };
                }

                // simple perform operators from left to right without considering sequencing
                if (currentChildNode is BlockData)
                {
                    if (left == null)
                    {
                        left = currentChildNode as BlockData;
                        continue;
                    }
                    var right = currentChildNode as BlockData;
                    var result = performOperation(left, opt, right);
                    left = new BlockData
                    {
                        Value = result
                    };
                    opt = null;
                    continue;
                }
                if (currentChildNode is BlockLeafOperator)
                {
                    opt = currentChildNode as BlockLeafOperator;
                    continue;
                }
            }

            return left.Value;
        }

        // perform cast after computing value
        return new DataPoint { DataType = "BOOL", Value = "true" };
    }

    DataPoint performOperation(BlockData left, BlockLeafOperator opt, BlockData right)
    {
        switch (opt.Operator)
        {
            case OPERATORS.ADD:
                return DataPoint.NewInt(int.Parse(left.Value.Value) + int.Parse(right.Value.Value));
            case OPERATORS.MINUS:
                return DataPoint.NewInt(int.Parse(left.Value.Value) - int.Parse(right.Value.Value));
            case OPERATORS.MULTIPLY:
                return DataPoint.NewInt(int.Parse(left.Value.Value) * int.Parse(right.Value.Value));
            case OPERATORS.DIVIDE:
                return DataPoint.NewInt(int.Parse(left.Value.Value) / int.Parse(right.Value.Value));

            case OPERATORS.EQUALS:
                return DataPoint.NewBool(int.Parse(left.Value.Value) == int.Parse(right.Value.Value));
            case OPERATORS.GREATER_THAN:
                return DataPoint.NewBool(int.Parse(left.Value.Value) > int.Parse(right.Value.Value));
            case OPERATORS.GREATER_OR_EQUALS:
                return DataPoint.NewBool(int.Parse(left.Value.Value) >= int.Parse(right.Value.Value));
            case OPERATORS.SMALLER_THAN:
                return DataPoint.NewBool(int.Parse(left.Value.Value) < int.Parse(right.Value.Value));
            case OPERATORS.SMALLER_OR_EQUALS:
                return DataPoint.NewBool(int.Parse(left.Value.Value) <= int.Parse(right.Value.Value));

            default:
                throw new NotImplementedException();
        }
    }

    internal DataPoint toFixedData(BlockDynamicData blockDynamicData, Dictionary<string, dynamic> datas)
    {
        var splits = blockDynamicData.Value.Value.Split(".");
        var data = datas[splits[0]];
        dynamic dataValue = null;
        for (var i = 1; i < splits.Length; i++)
        {
            // access object property from string: https://stackoverflow.com/questions/2905187/accessing-object-property-as-string-and-setting-its-value
            PropertyInfo property = data.GetType().GetProperty(splits[1]);
            dataValue = property.GetValue(data, null);
        }

        return new DataPoint { Value = dataValue.ToString(), DataType = blockDynamicData.Value.DataType };
    }

    public bool IsMatchCondition(BlockBase rule, DataPoint datas)
    {
        return true;
    }
}