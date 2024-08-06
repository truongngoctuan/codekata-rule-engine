using Checkout.Core.Contracts.Persistences;
using Checkout.Domain.Entities;
using Checkout.RuleEngine;
using Checkout.RuleEngine.Entities;
using Moq;

namespace Checkout.Core.Tests;

public class CheckoutService_ScanWithRules_Tests
{
    [Theory]
    [InlineData("A", 50, 10)]
    public void Scan_A_ReturnWithRuleReduction(string sku, decimal unitPrice, decimal priceReduction)
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new RuleEngineService();

        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine);

        var r = new Rule
        {
            Data = [],
            Condition = new BlockNode
            {
                Children = [
                    BlockLeaf.NewInt(2),
                    new BlockLeafOperator { Operator = OPERATORS.GREATER_THAN },
                    BlockLeaf.NewInt(1),
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = BlockLeaf.NewDecimal(priceReduction)
            }
        };
        checkoutService.Rules = [r];

        productRepository.Setup(s => s.GetBySkuAsync("A"))
            .Returns(Task.FromResult(new ProductEntity
            {
                SKU = sku,
                UnitPrice = unitPrice
            }));

        //Act test
        checkoutService.Scan(sku);
        var result = checkoutService.Total();

        //Assert test
        Assert.Single(checkoutService.CartItems);
        Assert.Single(checkoutService.CartItems["A"].Modifiers);
        Assert.Equal(unitPrice - priceReduction, result);
    }
}