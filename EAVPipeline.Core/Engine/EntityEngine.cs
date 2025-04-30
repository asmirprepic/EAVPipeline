using EAVPipeline.Core.Models;
using System.Data;
using Dapper;

namespace EAVPipeline.Core.Engine;


public class EntityEngine
{
    public int EntityId {get;set;}
    public string EntityType {get;set;} = null;
    public Dictionary<string,object?> Values {get;set;} = new();
    public List<AttributeDefinition> Definitions {get;set;} = new();

    public void Validate(){
        foreach (var def in Definitions){
            Values.TryGetValue(def.Slug,out var val);

            if (def.Required && val == null)
                throw new Exception($"Attribute {def.Slug} is required");
            
            if(val != null && val.GetType() != def.DataType)
            throw new Exception($"Attribute {def.Slug} has invalid type");

            if(def.IsEnum && def.EnumOptions != null && !def.EnumOptions.Contains(val?.ToString()))
            throw new Exception($"'{val}' is not a valid enum value for '{def.Slug}'");

        }
    }

    public void Save(IDbConnection connection){
        foreach (var pair in Values){
            connection.Execute("INSERT INTO AttributeValues (EntityId, AttributeSlug, Value) VALUES (@EntityId, @Slug, @Value)",
            new {EntityId, Slug = pair.Key,Value = pair.Value?.ToString()});

        }
    }


}