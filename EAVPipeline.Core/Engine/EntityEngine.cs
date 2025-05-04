using EAVPipeline.Core.Models;
using System.Data;
using Dapper;

namespace EAVPipeline.Core.Engine;


public class EntityEngine
{
    public int EntityId {get;set;}
    public string EntityType {get;set;} = string.Empty;
    //public List<AttributeEnumOption> EnumOptions { get; set; } = new();

    public Dictionary<string,object?> Values {get;set;} = new();
    public List<AttributeDefinition> Definitions {get;set;} = new();

    public static Type GetClrType(AttributeDataType type) =>
    type switch {
        AttributeDataType.String => typeof(string),
        AttributeDataType.Double => typeof(double),
        AttributeDataType.Integer => typeof(int),
        AttributeDataType.Boolean => typeof(bool),
        AttributeDataType.DateTime => typeof(DateTime),
        _ => typeof(string)
    };


    public void Validate(){
        foreach (var def in Definitions){
            Values.TryGetValue(def.Slug, out var val);

            if (def.Required && val == null)
                throw new Exception($"Attribute '{def.Slug}' is required.");

            if (val != null && val.GetType() != GetClrType(def.DataType))
                throw new Exception($"Attribute '{def.Slug}' has invalid type. Expected {GetClrType(def.DataType).Name}, got {val.GetType().Name}.");
            
            

            // if (def.IsEnum && def.EnumOptions != null && !def.EnumOptions.Contains(val?.ToString()?? string.Empty))
            //     throw new Exception($"'{val}' is not a valid enum value for '{def.Slug}'.");
        

        }
    }

    public void Save(IDbConnection connection){
        foreach(var pair in Values){
            var definition = Definitions.FirstOrDefault(d => d.Slug == pair.Key);
            if (definition == null)
                throw new Exception($"Definition for new attribute '{pair.Key} not found.'");
            
            int attributeId = connection.QueryFirst<int>(
                "SELECT AttributeId FROM AttributeDefinitions WHERE Slug = @Slug",
                new {Slug = pair.Key});
            
            var existing = connection.QueryFirstOrDefault<string?>(
                "SELECT Value FROM AttributeValues WHERE EntityId = @EntityId AND AttributeId = @AttributeId AND ArchivedAt IS NULL",
            new {EntityId,AttributeId = attributeId});

            string? newVal = pair.Value?.ToString();

            if (existing != null){
                if (existing != newVal){
                    connection.Execute(
                        "UPDATE AttributeValues SET ArchivedAt = CURRENT_TIMESTAMP WHERE EntityId = @EntityId AND AttributeID = @AttributeId AND ArchivedAt IS NULL",
                    new{EntityId, AttributeId = attributeId});

                    connection.Execute(
                        "INSERT INTO AttributeValues (EntityId, AttributeId, Value, ArchivedAt) VALUES (@EntityId, @AttributeId, @Value, NULL)",
                        new { EntityId, AttributeId = attributeId, Value = newVal });
                }
            }

            else {

                connection.Execute(
                    "INSERT INTO AttributeValues (EntityId, AttributeId, Value, ArchivedAt) VALUES (@EntityId,@AttributeId,@Value,NULL)",
                new {EntityId, AttributeId = attributeId,Value = newVal});
            }

            

            
        }
    }



}