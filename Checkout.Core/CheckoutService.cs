using System;
using System.Collections.Generic;
using Checkout.Core.Contracts.Persistences;
using Checkout.Domain;

namespace Checkout.Core
{
    public class CheckoutService : ICheckoutService
    {
        public IEnumerable<RuleEngine.Entities.Rule> Rules { get; set; }
        internal Dictionary<string, CartItem> CartItems { get; set; }
        IProductRepository productRepository { get; }

        public CheckoutService(IProductRepository productRepository)
        {
            Rules = new List<RuleEngine.Entities.Rule>();
            CartItems = new Dictionary<string, CartItem>();
            this.productRepository = productRepository;
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
                    // TODO: check rule

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
                    // TODO: check rule

                    CartItems[sku] = currentItem;
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
    }
}
