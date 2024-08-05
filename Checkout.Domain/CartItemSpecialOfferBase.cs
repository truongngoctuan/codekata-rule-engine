using System;
using System.Collections.Generic;

namespace Checkout.Domain
{
  public abstract class CartItemSpecialOfferBase
  {
    public string RuleId { get; set; }
    abstract public decimal GetPriceReduction(CartItem item);
  }
}
