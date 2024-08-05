using System;
using System.Collections.Generic;

namespace Checkout.Domain
{
  public class CartItemSpecialPrice : CartItemSpecialOfferBase
  {
    public uint Quantity { get; set; }
    public decimal SpecialOffer { get; set; }
    public override decimal GetPriceReduction(CartItem item)
    {
      // there is no limit to the number of items
      uint fold = item.Quantity / Quantity;
      return fold * (Quantity * item.UnitPrice - SpecialOffer);
      throw new NotImplementedException();
    }
  }
}
