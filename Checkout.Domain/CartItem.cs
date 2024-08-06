using System;
using System.Collections.Generic;

namespace Checkout.Domain
{
    public class CartItem
    {
        public string SKU { get; set; }
        public decimal UnitPrice { get; set; }
        public uint Quantity { get; set; }

        public IEnumerable<CartItemSpecialOfferBase> Modifiers { get; set; }

        public decimal Sum()
        {
            decimal totalReduction = 0;
            foreach (var item in Modifiers)
            {
                totalReduction -= item.GetPriceReduction(this);
            }
            return UnitPrice * Quantity + totalReduction;
        }
    }
}
