namespace EAVPipeline.Core.Models;

public class AttributeValue{
    public int EntityId {get;set;}
    public string AttributeSlug {get;set;} = null;
    public object? Value {get;set;}
}