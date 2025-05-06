using System.Data;
using Dapper;
using EAVPipeline.Core.Models;

namespace EAVPipeline.Core.Repositories;

public static class DefinitionRepository{
  public static void InsertDefinitionIfMissing(IDbConnection connection,AttributeDefinition def){
    int exists = connection.ExecuteScalar<int>(
        "SELECT COUNT(*) FROM AttributeDefinitions WHERE Slug = @Slug",
        new {def.Slug} );
    
    if(exists == 0){
        connection.Execute(@"
        INSERT INTO AttributeDefinitions (Slug, DisplayName,DataType,Required,IsEnum)
        Values (@Slug,@DisplayName,@DataType,@Required,@IsEnum)
        ",def);
    }
  }

  public static int GetAttributeIdBySlug(IDbConnection connection, string slug) =>
  connection.QueryFirst<int>("SELECT AttributeId FROM AttributeDefinitions WHERE Slug = @slug",new {slug}  );

  public static void InsertEnumOptionIfMissing(IDbConnection connection,int attributeId,string value){
    int exists = connection.ExecuteScalar<int>(
        "SELECT COUNT(*) FROM AttributeEnumOptions WHERE AttributeId = @AttributeId AND Value = @Value",
        new {AttributeId = attributeId,Value= value})  ;

    if (exists == 0){
        connection.Execute(
            "INSERT INTO AttributeEnumOptions (AttributeId,Value) VALUES (@AttributeId,@Value)",
            new {AttributeId = attributeId,Value = value});
    }
  }

  public static List<AttributeDefinition> LoadDefinitions(IDbConnection connection) =>
    connection.Query<AttributeDefinition>("SELECT * FROM AttributeDefinitions").ToList();

  public static List<AttributeEnumOption> LoadEnumOptions(IDbConnection connection) =>
    connection.Query<AttributeEnumOption>("SELECT * FROM AttributeEnumOptions").ToList();

}