using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine;

public interface IRuleEngineService
{
    public DataPoint Compute(BlockBase root, DataPoint datas);
}