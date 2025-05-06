using System.Data.SQLite;
using Dapper;
using EAVPipeline.Core.Utilites;

var connection = new SQLiteConnection("Data source = eav.db");

connection.Open();

SqlUtility.ExecuteNonQuery(connection,"DROP TABLE IF EXISTS RawInputData");