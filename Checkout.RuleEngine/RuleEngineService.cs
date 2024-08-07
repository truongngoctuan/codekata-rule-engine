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
        if (root is BlockData)
        {
            return handleBlockData(root as BlockData);
        }

        if (root is BlockDynamicData)
        {
            return handleBlockDynamicData(root as BlockDynamicData, datas);
        }

        if (root is BlockNode)
        {
            return handleBlockNode(root as BlockNode, datas);
        }

        throw new NotImplementedException();
    }

    DataPoint handleBlockData(BlockData blockLeaf)
    {
        return blockLeaf.Value;
    }

    DataPoint handleBlockDynamicData(BlockDynamicData blockLeaf, Dictionary<string, dynamic> datas)
    {
        return toFixedData(blockLeaf, datas);
    }

    DataPoint handleBlockNode(BlockNode blockNode, Dictionary<string, dynamic> datas)
    {
        if (blockNode.Children.Count() == 1)
        {
            return Compute(blockNode.Children[0], datas);
        }

        var childrenNodes = blockNode.Children.ToList();
        var optsPrecedencesDict = new Dictionary<ushort, bool>();
        var optPrecedences = childrenNodes.Where(node => node is BlockOperator)
            .Select(node => OPERATORS.Operators_Precedence[(node as BlockOperator).Operator])
            .Distinct().ToArray();
        Array.Sort(optPrecedences);

        foreach (var optPrecedence in optPrecedences)
        {
            var newChildrenNode = new List<BlockBase>();

            BlockData left = null;
            BlockOperator blockOpt = null;
            for (int i = 0; i < childrenNodes.Count(); i++)
            {
                var currentChildNode = childrenNodes[i];
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
                    var result = performOperation(left, blockOpt, right);
                    left = new BlockData
                    {
                        Value = result
                    };
                    blockOpt = null;
                    continue;
                }
                if (currentChildNode is BlockOperator)
                {
                    if (OPERATORS.Operators_Precedence[(currentChildNode as BlockOperator).Operator] == optPrecedence)
                    {
                        blockOpt = currentChildNode as BlockOperator;
                        continue;
                    }
                    newChildrenNode.Add(left);
                    newChildrenNode.Add(currentChildNode);
                    left = null;
                }
            }

            if (left != null)
            {
                newChildrenNode.Add(left);
            }
            childrenNodes = newChildrenNode;
        }

        if (childrenNodes.Count == 1)
        {
            return (childrenNodes[0] as BlockData).Value;
        }

        return null;
    }

    DataPoint performOperation(BlockData left, BlockOperator opt, BlockData right)
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
        if (!datas.ContainsKey(splits[0])) return null;

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
}