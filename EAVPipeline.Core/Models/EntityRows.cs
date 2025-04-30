namespace EAVPipeline.Core.Models;

public class EntityRow {
    public int EntityId {get;set;};
    public string EntityType {get;set;} = null;
    public Dictionary<string,object?> Attributes {get;} = new();
    
}