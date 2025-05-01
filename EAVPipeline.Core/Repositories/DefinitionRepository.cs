using System.Data.SQLite;
using Dapper;
using EAVPipeline.Core.Models;

namespace EAVPipeline.Core.Repositories;
public static class DefititionRepository{
  public static void InsertDefinitionIfMissing(SQLiteConnection connection,AttributeDefinition def){
    var exists = connection.ExecuteScalar<int>(
        "SELECT COUNT(*) FROM AttributeDefinition WHERE Slug = @Slug",
        new {def.Slug} )>0;
    
    if(!exists){
        connection.Execute(@"
        INSERT INTO AttibuteDefinitions (Slug, DisplayName,DataType,Required,IsEnum)
        Values (@Slug,@DisplayName,@DataType,@Required,@IsEnum)
        ",def);
    }
  }

  public static int GetAttributeIdBySlug(SQLiteConnection connection, string slug) =>
  connection.QueryFirst<int>("SELECT AttributeId FROM AttributeDefinitions WHERE Slug = @Slug",new {slug}  );

  public static void InsertEnumOptionIfMissing(SQLiteConnection connection,int attributeId,string value){
    var exists = connection.ExecuteScalar<int>(
        "SELECT COUNT(*) FROM AttributeEnumOptions WHERE AttributeId = @AttributeId AND Value = @Value",
        new {AttributeId = attributeId,Value= value}) >0 ;

    if (!exists){
        connection.Execute(
            "INSERT INTO AttributeEnumOptions (AttributeId,Value) VALUES (@AttributeId,@Value)",
            new {AttributeId = attributeId,Value = value})>0;
    }
  }

  public static List<AttributeDefinition> LoadDefinitions(SQLiteConnection connection) =>
    connection.Query<AttributeDefinition>("SELECT * FROM AttributeDefinitions").ToList();

  public static List<AttributeEnumOption> LoadEnumOptions(SQLiteConnection connection) =>
    connection.Query<AttributeEnumOption>("SELECT * FROM AttributeEnumOptions").ToList();

}