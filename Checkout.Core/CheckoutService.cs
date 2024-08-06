using System;
using System.Collections.Generic;
using System.Linq;
using Checkout.Core.Contracts.Persistences;
using Checkout.Domain;
using Checkout.RuleEngine;

namespace Checkout.Core
{
    public class CheckoutService : ICheckoutService
    {
        public IEnumerable<RuleEngine.Entities.Rule> Rules { get; set; }
        internal Dictionary<string, CartItem> CartItems { get; set; }
        IProductRepository productRepository { get; }
        IRuleEngineService ruleEngineService { get; }

        public CheckoutService(IProductRepository productRepository, IRuleEngineService ruleEngineService)
        {
            Rules = new List<RuleEngine.Entities.Rule>();
            CartItems = new Dictionary<string, CartItem>();
            this.productRepository = productRepository;
            this.ruleEngineService = ruleEngineService;
        }

        public async void Scan(string sku)
        {
            var product = await productRepository.GetBySkuAsync(sku);
            if (product != null)
            {
                if (CartItems.ContainsKey(sku))
                {
                    var currentItem = CartItems[sku];
                    currentItem.Quantity += 1;
                    applyRules(currentItem, Rules);

                    CartItems[sku] = currentItem;
                }
                else
                {
                    var currentItem = new CartItem
                    {
                        SKU = sku,
                        Quantity = 1,
                        UnitPrice = product.UnitPrice,
                        Modifiers = new List<CartItemSpecialOfferBase>()
                    };
                    CartItems[sku] = currentItem;
                    applyRules(currentItem, Rules);
                }
            }
        }

        public decimal Total()
        {
            decimal total = 0;
            foreach (var item in CartItems)
            {
                total += item.Value.Sum();
            }
            return total;
        }

        void applyRules(CartItem item, IEnumerable<RuleEngine.Entities.Rule> rules)
        {
            item.Modifiers = new List<CartItemSpecialOfferBase>();


            foreach (var rule in rules)
            {
                // extract data from its definition
                var datas = new Dictionary<string, dynamic>();
                foreach (var dataPointDefinition in rule.Data)
                {
                    var extractor = new DataPointCartItemExtractor(CartItems.Values.ToArray());
                    var data = extractor.Extract(dataPointDefinition);
                    if (data != null)
                    {
                        datas[dataPointDefinition.Name] = data;
                    }
                }


                var conditionResult = ruleEngineService.Compute(rule.Condition, datas);
                if (conditionResult.DataType == DATA_TYPE.BOOL &&
                    conditionResult.Value == BOOL_DATA.TRUE)
                {
                    var actionResult = ruleEngineService.Compute(rule.Action.ComputedValue, datas);
                    if (actionResult.DataType == DATA_TYPE.DECIMAL)
                    {
                        item.Modifiers = item.Modifiers.Append(new CartItemPriceReduction(decimal.Parse(actionResult.Value)));
                        return;
                    }
                }
            }
        }
    }
}
