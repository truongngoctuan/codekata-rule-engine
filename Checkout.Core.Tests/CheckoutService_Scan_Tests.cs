using Checkout.Core.Contracts.Persistences;
using Checkout.Domain.Entities;
using Checkout.RuleEngine;
using Moq;

namespace Checkout.Core.Tests;

public class CheckoutService_Scan_Tests
{
    [Theory]
    [InlineData("A", 50)]
    public void Scan_A_ReturnUnitPrice(string sku, decimal unitPrice)
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new Mock<IRuleEngineService>();
        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine.Object);

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
        Assert.Equal(unitPrice, result);
        Assert.Single(checkoutService.CartItems);
    }

    [Fact]
    public void Scan_AB_ReturnTotal()
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new Mock<IRuleEngineService>();
        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine.Object);

        productRepository.Setup(s => s.GetBySkuAsync("A"))
            .Returns(Task.FromResult(new ProductEntity
            {
                SKU = "A",
                UnitPrice = 50
            }));

        productRepository.Setup(s => s.GetBySkuAsync("B"))
            .Returns(Task.FromResult(new ProductEntity
            {
                SKU = "B",
                UnitPrice = 30
            }));

        //Act test
        checkoutService.Scan("A");
        checkoutService.Scan("B");
        var result = checkoutService.Total();

        //Assert test
        Assert.Equal(80, result);
        Assert.Equal(2, checkoutService.CartItems.Count());
    }
}