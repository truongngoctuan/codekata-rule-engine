using System;
using System.Collections;
using System.Collections.Generic;
using Checkout.RuleEngine.Entities;

namespace Checkout.Core
{
  public interface ICheckoutService
  {
    IEnumerable<Rule> Rules { get; set; }
    void Scan(string sku);
    decimal Total();
  }
}
