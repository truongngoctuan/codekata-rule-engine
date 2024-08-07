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
                    BlockData.NewInt(2),
                    new BlockOperator { Operator = OPERATORS.GREATER_THAN },
                    BlockData.NewInt(1),
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = BlockData.NewDecimal(priceReduction)
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

    [Theory]
    [InlineData("A", 50, 10)]
    public void Scan_DynamicData_ReturnDataAccessFromDynamicData(string sku, decimal unitPrice, decimal priceReduction)
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new RuleEngineService();

        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine);

        var r = new Rule
        {
            Data = [
                getCartItemDefinition("A")
            ],
            Condition = new BlockNode
            {
                Children = [
                    BlockDynamicData.NewInt("CartItem.Quantity"),
                    new BlockOperator { Operator = OPERATORS.EQUALS },
                    BlockData.NewInt(1)
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = BlockData.NewDecimal(priceReduction)
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

    [Theory]
    [InlineData("A", 50, 10)]
    public void Scan_ScanARuleB_ReturnNoDiscount(string sku, decimal unitPrice, decimal priceReduction)
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new RuleEngineService();

        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine);

        var r = new Rule
        {
            Data = [
                getCartItemDefinition("B")
            ],
            Condition = new BlockNode
            {
                Children = [
                    BlockDynamicData.NewInt("CartItem.Quantity"),
                    new BlockOperator { Operator = OPERATORS.EQUALS },
                    BlockData.NewInt(1)
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = BlockData.NewDecimal(priceReduction)
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
        Assert.Empty(checkoutService.CartItems["A"].Modifiers);
        Assert.Equal(unitPrice, result);
    }

    [Fact]
    public void Scan_SpecialOffers_ReturnItemAWithSpecialOffers()
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new RuleEngineService();

        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine);

        var r = new Rule
        {
            Data = [
                getCartItemDefinition("A")
            ],
            Condition = new BlockNode
            {
                Children = [
                    // Items["A"].Quantity / 3 > 0
                    BlockDynamicData.NewInt("CartItem.Quantity"),
                    new BlockOperator { Operator = OPERATORS.DIVIDE },
                    BlockData.NewInt(3),
                    new BlockOperator { Operator = OPERATORS.GREATER_THAN },
                    BlockData.NewInt(0)
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = new BlockNode
                {
                    Children = [
                        //Items["A].Quantity / 3 * SpecialPrice
                        BlockDynamicData.NewInt("CartItem.Quantity"),
                        new BlockOperator { Operator = OPERATORS.DIVIDE },
                        BlockData.NewInt(3),
                        new BlockOperator { Operator = OPERATORS.MULTIPLY },
                        BlockData.NewInt(20),
                    ]
                }
            }
        };
        checkoutService.Rules = [r];

        productRepository.Setup(s => s.GetBySkuAsync("A"))
            .Returns(Task.FromResult(new ProductEntity
            {
                SKU = "A",
                UnitPrice = 50
            }));

        //Act test
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        var result = checkoutService.Total();

        //Assert test
        Assert.Single(checkoutService.CartItems);
        Assert.Single(checkoutService.CartItems["A"].Modifiers);
        Assert.Equal(130, result);
    }
    [Fact]
    public void Scan_SpecialOffers2Times_ReturnItemAWith2XSpecialOffers()
    {
        //Arrange test
        var productRepository = new Mock<IProductRepository>();
        var ruleEngine = new RuleEngineService();

        var checkoutService = new CheckoutService(productRepository.Object, ruleEngine);

        var r = new Rule
        {
            Data = [
                getCartItemDefinition("A")
            ],
            Condition = new BlockNode
            {
                Children = [
                    // Items["A"].Quantity / 3 > 0
                    BlockDynamicData.NewInt("CartItem.Quantity"),
                    new BlockOperator { Operator = OPERATORS.DIVIDE },
                    BlockData.NewInt(3),
                    new BlockOperator { Operator = OPERATORS.GREATER_THAN },
                    BlockData.NewInt(0)
                ]
            },
            Action = new RuleAction
            {
                ActionType = "PRICE_REDUCTION",
                ComputedValue = new BlockNode
                {
                    Children = [
                        //Items["A].Quantity / 3 * SpecialPrice
                        BlockDynamicData.NewInt("CartItem.Quantity"),
                        new BlockOperator { Operator = OPERATORS.DIVIDE },
                        BlockData.NewInt(3),
                        new BlockOperator { Operator = OPERATORS.MULTIPLY },
                        BlockData.NewInt(20),
                    ]
                }
            }
        };
        checkoutService.Rules = [r];

        productRepository.Setup(s => s.GetBySkuAsync("A"))
            .Returns(Task.FromResult(new ProductEntity
            {
                SKU = "A",
                UnitPrice = 50
            }));

        //Act test
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        checkoutService.Scan("A");
        var result = checkoutService.Total();

        //Assert test
        Assert.Single(checkoutService.CartItems);
        Assert.Single(checkoutService.CartItems["A"].Modifiers);
        Assert.Equal(260, result);
    }

    #region data setup
    DataPointDefinition getCartItemDefinition(string sku)
    {
        return new DataPointDefinition
        {
            Name = "CartItem",
            DataType = "CART_ITEM",
            IsRequired = true,
            Params = [
                        new DataPointParam {
                            Key = "SKU",
                            DataType = "STRING",
                            Value = sku
                        }
                    ]
        };
    }
    #endregion
}