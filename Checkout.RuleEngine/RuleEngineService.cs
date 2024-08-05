using System;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine
{
    public class RuleEngineService
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
            return new DataPoint { DataType = "BOOL", Value = "true" };
        }

        public bool IsMatchCondition(BlockBase rule, DataPoint datas)
        {
            return true;
        }
    }
}
