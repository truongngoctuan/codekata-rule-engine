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
                BlockData.NewInt(2),
                BlockOperator.NewAdd(),
                BlockData.NewInt(2),
            ]
        };

        //Act test
        var result = engine.Compute(root, null);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal("4", result.Value);
        Assert.Equal(DATA_TYPE.INT, result.DataType);
    }

    [Fact]
    public void Compute_2GreaterThan1_ReturnTrue()
    {
        //Arrange test
        var engine = new RuleEngineService();
        var root = new BlockNode
        {
            Children = [
                BlockData.NewInt(2),
                new BlockOperator { Operator = OPERATORS.GREATER_THAN },
                BlockData.NewInt(1),
            ]
        };

        //Act test
        var result = engine.Compute(root, null);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal(BOOL_DATA.TRUE, result.Value);
        Assert.Equal(DATA_TYPE.BOOL, result.DataType);
    }

    [Fact]
    public void Compute_OperatorPrecedence()
    {
        //Arrange test
        var engine = new RuleEngineService();
        var root = new BlockNode
        {
            Children = [
                BlockData.NewInt(1),
                new BlockOperator { Operator = OPERATORS.SMALLER_THAN },
                BlockData.NewInt(1),
                new BlockOperator { Operator = OPERATORS.ADD },
                BlockData.NewInt(1),
            ]
        };

        //Act test
        var result = engine.Compute(root, null);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal(BOOL_DATA.TRUE, result.Value);
        Assert.Equal(DATA_TYPE.BOOL, result.DataType);
    }
}