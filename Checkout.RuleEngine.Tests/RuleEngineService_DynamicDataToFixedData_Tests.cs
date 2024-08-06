using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Tests;

public class RuleEngineService_DynamicDataToFixedData_Tests
{
    [Fact]
    public void ToFixedData_CartItem_ReturnQuantityValue()
    {
        //Arrange test
        var engine = new RuleEngineService();
        var root = BlockDynamicData.NewInt("Item.Value1");
        var datas = new Dictionary<string, dynamic>() {
            {"Item", new { Value1 = 5 }}
        };

        //Act test
        var result = engine.toFixedData(root, datas);

        //Assert test
        Assert.NotNull(result);
        Assert.Equal("5", result.Value);
        Assert.Equal(DATA_TYPE.INT, result.DataType);
    }

}