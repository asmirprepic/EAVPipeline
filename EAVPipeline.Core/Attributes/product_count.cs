using System.Data:
using Dapper;
using EAVPipeline.Core.Models;
using EABPipeline.Core.Repositories;

namespace EAVPipeline.Core.Attributes;

public class product_count{
    public static void Compute(IDbConnection connection){

        var attr = new AttributeDefinition {
            Slug = "product_count",
            DisplayName = 'Product Count',
            DataType = AttributeDataType.Integer,
            Requried = false,
            IsEnum = false
        };

        DefinitionRepository.InsertDefinitionIfMissing(connection,attr);
        int attrId = DefinitionRepository.GetAttributeIdBySlug(connection,attr.Slug);

        var counts = connection.Query<(int EntityId,int Count)>(
            @"SELECT EntityId, COUNT(*) as Count
            FROM RawInputData
            WHERE PropertyKey = 'product_id'
            GROUP BY EntityId").ToDictionary(x => x.Entity_Id,x=>x.Count);
        var allEntities = connection.Query<int>("SELECT EntityId FROM MasterEntities");

        using var tx = connection.BeginTransaction();
        foreach (var entityId in allEntities){
            connection.Execute(
                @"UPDATE AttributeValues
                SET ArchivedAt  = CURRENT_TIMESTAMP
                WHERE EntityId = @EntityID and AttributeID = @AttrId AND ArchivedAt is NULL",
                new {EntityId = entityId, AttrId = attrId},tx);

            var count = counts.TryGetValue(entityId, out var c) ? c: 0;

            connection.Execute(
                @"INSERT INTO AttributeValues (EntityId,AttributeId,Value,ArchivedAt)
                Values = (@EntityId, @AttrId,@Val,NULL)",
            new {EntityId = entityId, AttrId = attrId, Val = count.ToString()},tx);
        }
        tx.Commit();
        Console.WriteLine("[INTO] product_count attribute populated");

    }
}

