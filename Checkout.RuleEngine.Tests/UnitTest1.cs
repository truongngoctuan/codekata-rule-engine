using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Tests;

public class RuleEngineService_Compute
{
    [Fact]
    public void Compute_2Plus2_Return4()
    {
        //Arrange test
        var engine = new RuleEngineService();
        var root = new BlockNode
        {
            DataType = "BLOCK",
            Children = new BlockBase[] {
                new BlockLeaf { DataType = "FIXED_DATA", Value = new DataPoint { Value = "2", DataType = "INT" } },
                new BlockLeaf { DataType = "OPERATOR_ADD" },
                new BlockLeaf { DataType = "FIXED_DATA", Value = new DataPoint { Value = "2", DataType = "INT" } },
            }
        };

        //Act test
        var result = engine.Compute(root, null);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal("2", result.Value);
        Assert.Equal("INT", result.DataType);
    }
}