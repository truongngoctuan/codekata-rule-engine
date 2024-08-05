using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Tests;

public class RuleEngineService_Compute_Tests
{
    [Fact]
    public void Compute_2Plus2_Return4()
    {
        //Arrange test
        var engine = new RuleEngineService();
        var root = new BlockNode
        {
            Children = [
                new BlockLeaf { Value = new DataPoint { Value = "2", DataType = "INT" } },
                new BlockLeafOperator { Operator = "ADD" },
                new BlockLeaf { Value = new DataPoint { Value = "2", DataType = "INT" } },
            ]
        };

        //Act test
        var result = engine.Compute(root, null);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal("4", result.Value);
        Assert.Equal("INT", result.DataType);
    }
}