using System;
using System.Linq;
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
    public DataPoint Compute(BlockBase root, DataPoint datas)
    {
        // stop condition
        if (root is BlockLeaf)
        {
            var blockLeaf = root as BlockLeaf;
            return blockLeaf.Value;
        }

        if (root is BlockNode)
        {
            var blockNode = root as BlockNode;
            if (blockNode.Children.Count() == 1)
            {
                return Compute(blockNode.Children[0], datas);
            }

            BlockLeaf left = null;
            BlockLeafOperator opt = null;
            for (int i = 0; i < blockNode.Children.Count(); i++)
            {
                var currentChildNode = blockNode.Children[i];
                if (currentChildNode is BlockLeaf)
                {
                    if (left == null)
                    {
                        left = currentChildNode as BlockLeaf;
                        continue;
                    }
                    var right = currentChildNode as BlockLeaf;
                    var result = performOperation(left, opt, right);
                    left = new BlockLeaf
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

    DataPoint performOperation(BlockLeaf left, BlockLeafOperator opt, BlockLeaf right)
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

    public bool IsMatchCondition(BlockBase rule, DataPoint datas)
    {
        return true;
    }
}