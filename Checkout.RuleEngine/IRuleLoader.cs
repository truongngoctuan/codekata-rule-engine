using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine
{
  public interface IRuleLoader
  {
    Task<IEnumerable<Rule>> LoadAsync(string source);
  }
}
