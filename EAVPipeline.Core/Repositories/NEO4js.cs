
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EAVPipeline.Data.Repositories
{
    public class Neo4jRepository : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jRepository(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task CreateEntityNode(string entityName)
        {
            await using var session = _driver.AsyncSession();
            var query = "CREATE (e:Entity {name: $name}) RETURN e";
            var parameters = new { name = entityName };
            await session.RunAsync(query, parameters);
        }

        public async Task<List<string>> GetAllEntities()
        {
            await using var session = _driver.AsyncSession();
            var query = "MATCH (e:Entity) RETURN e.name";
            var result = await session.RunAsync(query);
            var entities = new List<string>();
            await result.ForEachAsync(record => entities.Add(record[0].As<string>()));
            return entities;
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
