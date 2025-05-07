using System.Data;
using Dapper;

public static class EntitySimulator{
    public static void SeedEntitiesAndRawProperties(IDbConnection connection,int totalentities = 1_000_000,int propertiesPerEntity = 5){
        var rnd = new Random();

        Console.WriteLine("[INFO] Seedint entities and raw properties...");
        using var transaction = connection.BeginTransaction();

        for (int i = 1; i <= totalentities;i++){
            connection.Execute("INSERT OR IGNORE INTO MasterEntities (EntityId,Name) VALUES (@EntityId,@Name)",
            new {EntityId  = i, Name = $"Entity_{i}"},transaction);
        

            // for (int j = 0;j < propertiesPerEntity; j++){
            //     string key = $"property_{j+1}";
            //     string value = $"val_{rnd.Next(1,100)}";

            //     connection.Execute("INSERT INTO RawInputData (EntityId,PropertyKey,PropertyValue) VALUES (@EntityId,@PropertyKey,@PropertyValue)",
            //     new {EntityId = i, PropertyKey = key, PropertyValue  = value},transaction
            //     );
            // }
    }
    transaction.Commit();
    Console.WriteLine("[INFO] Seeding complete.");
    }
}