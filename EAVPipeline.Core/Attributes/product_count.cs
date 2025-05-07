using System.Data;
using Dapper;
using EAVPipeline.Core.Models;
using EAVPipeline.Core.Repositories;
using System.Collections.Generic;

namespace EAVPipeline.Core.Attributes;


public class product_count{
    public static void Compute(IDbConnection connection){
        Console.WriteLine("[START] product_count attribute computation started");
        
        var attr = new AttributeDefinition {
            Slug = "product_count",
            DisplayName = "Product Count",
            DataType = AttributeDataType.Integer,
            Required = false,
            IsEnum = false
        };
    

    DefinitionRepository.InsertDefinitionIfMissing(connection,attr);
    int attrId = DefinitionRepository.GetAttributeIdBySlug(connection,attr.Slug);

    var counts = connection.Query<(int EntityId,int Count)>(
        @"SELECT CustomerId,COUNT(*) as Count FROM  CustomerPurchases GROUP BY CustomerId"
    ).ToDictionary(x => x.EntityId, x => x.Count);

    var allEntities = connection.Query<int>("SELECT EntityId FROM MasterEntities");

    using var tx = connection.BeginTransaction();

    connection.Execute("DROP TABLE IF EXISTS TempProductCounts;",transaction: tx);
    connection.Execute(@"
    CREATE TEMP TABLE TempProductCounts (
        EntityId INTEGER PRIMARY KEY,
        Value TEXT NOT NULL
    );
    ",transaction: tx);

    var tempRows = allEntities.Select(id => new {
        EntityId = id,
        Value = counts.TryGetValue(id,out var c) ? c.ToString(): "0"
    });

    connection.Execute("INSERT INTO TempProductCounts (EntityId, Value) VALUES (@EntityId,@Value);",tempRows,transaction: tx);
    Console.WriteLine("Executed Here");
    connection.Execute(@"
    UPDATE AttributeValues
    SET ArchivedAt = CURRENT_TIMESTAMP
    WHERE AttributeId = @AttrId AND ArchivedAt IS NULL
    AND EntityId IN (SELECT EntityId FROM TempProductCounts);",new {AttrId = attrId},transaction: tx);

    connection.Execute(
        @"INSERT INTO AttributeValues (EntityId, AttributeId, Value, ArchivedAt)
        SELECT EntityId,@AttrId,Value,NULL
        FROM TempProductCounts;
        ",new {AttrId = attrId},transaction: tx);

    tx.Commit();

    Console.WriteLine("[DONE] product_count attribute populated.");

    }
}


// public class product_count{
//     public static void Compute(IDbConnection connection){
        
//         Console.WriteLine("[START] product_count attribute computation started."); // Log start

//         var attr = new AttributeDefinition {
//             Slug = "product_count",
//             DisplayName = "Product Count",
//             DataType = AttributeDataType.Integer,
//             Required = false,
//             IsEnum = false
//         };
//         Console.WriteLine($"[INFO] Attribute definition for '{attr.Slug}' ensured.");

//         DefinitionRepository.InsertDefinitionIfMissing(connection,attr);
//         int attrId = DefinitionRepository.GetAttributeIdBySlug(connection,attr.Slug);
//         Console.WriteLine($"[INFO] Retrieved attribute ID for '{attr.Slug}': {attrId}.");

//         var counts = connection.Query<(int EntityId,int Count)>(
//             @"SELECT CustomerId, COUNT(*) as Count
//             FROM CustomerPurchases
//             GROUP BY CustomerId").ToDictionary(x => x.EntityId,x=>x.Count);
//         var allEntities = connection.Query<int>("SELECT EntityId FROM MasterEntities");

//         using var tx = connection.BeginTransaction();
//         int processedCount = 0;
//         foreach(var entityId in allEntities){
//             connection.Execute(
//                 @"UPDATE AttributeValues
//                 SET ArchivedAt  = CURRENT_TIMESTAMP
//                 WHERE EntityId = @EntityId and AttributeID = @AttrId AND ArchivedAt is NULL",
//                 new {EntityId = entityId, AttrId = attrId},tx);

//             var count = counts.TryGetValue(entityId, out var c) ? c: 0; 

//             connection.Execute(
//                 @"INSERT INTO AttributeValues (EntityId,AttributeId,Value,ArchivedAt)
//                 Values  (@EntityId, @AttrId,@Val,NULL)",
//             new {EntityId = entityId, AttrId = attrId, Val = count.ToString()},tx);

//             processedCount++;
//             if (processedCount % 100_000==0){
//                 Console.WriteLine($"[PROGRESS] processed {processedCount} entities ...");
//             }
//         }
//         tx.Commit();
//         Console.WriteLine("[INTO] product_count attribute populated");

//     }
// }

