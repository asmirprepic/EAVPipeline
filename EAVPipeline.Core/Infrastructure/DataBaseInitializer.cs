using System.Data.SQLite;
namespace EAVPipeline.Core.Infrastructure;
using Dapper;


public static class DataBaseInitializer {
    public static void EnsureSchema(SQLiteConnection connection){
        
        var command = connection.CreateCommand();
        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS AttributeDefinitions (
        AttributeId INTEGER PRIMARY KEY AUTOINCREMENT,
        Slug TEXT NOT NULL UNIQUE,
        DisplayName TEXT NOT NULL,
        DataType TEXT NOT NULL, 
        Required BOOLEAN NOT NULL,
        IsEnum BOOLEAN NOT NULL
        
        );

        CREATE TABLE IF NOT EXISTS AttributeEnumOptions (
            OptionId INTEGER PRIMARY KEY AUTOINCREMENT,
            AttributeId INTEGER NOT NULL,
            Value TEXT NOT NULL,
            FOREIGN KEY (AttributeId) REFERENCES AttributeDefinitions(AttributeId)
        );

        CREATE TABLE IF NOT EXISTS AttributeValues (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        AttributeId INTEGER NOT NULL,
        EntityId INTEGER NOT NULL,
        Value TEXT, 
        ArchivedAt TEXT,
        FOREIGN KEY (AttributeId) References AttributeDefinitions (AttributeId)
        );
        ";
        command.ExecuteNonQuery();
    }
}