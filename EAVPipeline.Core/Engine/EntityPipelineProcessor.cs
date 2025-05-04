using EAVPipeline.Core.Models;
using System.Data;
using System.Collections.Generic;

namespace EAVPipeline.Core.Engine;

public class EntityPipelineProcessor
{
    private readonly IDbConnection _connection;
    private readonly List<AttributeDefinition> _definitions;
    private readonly List<AttributeEnumOption> _enumOptions;

    public EntityPipelineProcessor(IDbConnection connection, List<AttributeDefinition> definitions, List<AttributeEnumOption> enumOptions)
    {
        _connection = connection;
        _definitions = definitions;
        _enumOptions = enumOptions;
    }

    public void ProcessEntity(int entityId, Dictionary<string, object?> values)
    {
        var engine = new EntityEngine
        {
            EntityId = entityId,
            Definitions = _definitions,
            //EnumOptions = _enumOptions,
            Values = values
        };

        engine.Validate();
        engine.Save(_connection);
    }

    public void ProcessEntities(IEnumerable<(int EntityId, Dictionary<string, object?> Values)> entities)
    {
        foreach (var (entityId, values) in entities)
        {
            try
            {
                ProcessEntity(entityId, values);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[WARN] Entity {entityId} failed: {ex.Message}");
            }
        }
    }
}
