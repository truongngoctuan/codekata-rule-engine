using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Checkout.RuleEngine.Entities;

namespace Checkout.RuleEngine.Persistence
{
  /// <summary>
  /// Configure polymorphism in resolver to avoid using attribute in the model
  /// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0
  /// </summary>
  public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
  {
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
      JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

      Type baseType = typeof(BlockBase);
      if (jsonTypeInfo.Type == baseType)
      {
        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
          TypeDiscriminatorPropertyName = "DataType",
          IgnoreUnrecognizedTypeDiscriminators = true,
          UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
          DerivedTypes =
                {
                    new JsonDerivedType(typeof(BlockNode), "BLOCK"),
                    new JsonDerivedType(typeof(BlockData), "FIXED_DATA"),
                    new JsonDerivedType(typeof(BlockDynamicData), "DYNAMIC_DATA")
                }
        };
      }

      return jsonTypeInfo;
    }
  }
}