using System.Text.Json;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Persistence
{
  public class RuleJsonFileLoader : IRuleLoader
  {
    public async Task<IEnumerable<Rule>> LoadAsync(string source)
    {

      using FileStream stream = File.OpenRead(source);
      var root = await JsonSerializer.DeserializeAsync<BlockBase>(stream, new JsonSerializerOptions
      {
        TypeInfoResolver = new PolymorphicTypeResolver()
      });

      if (root == null) throw new JsonFileDeserializingError();

      return
      [
        new Rule {
          Condition = root
        }
      ];
    }
  }

  [Serializable]
  internal class JsonFileDeserializingError : Exception
  {
    public JsonFileDeserializingError()
    {
    }

    public JsonFileDeserializingError(string? message) : base(message)
    {
    }

    public JsonFileDeserializingError(string? message, Exception? innerException) : base(message, innerException)
    {
    }
  }
}


