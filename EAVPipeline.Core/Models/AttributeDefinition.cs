namespace EAVPipeline.Core.Models;

public class AttributeDefinition{
    public string Slug {get; set;} = null;
    public string DisplayName {get; set;} = null;
    public Type DataType {get; set;} = typeof(String);
    public bool Required {get; set;} 
    public bool IsEnum {get;set;}
    public List<string> ? EnumOptions {get;set;}
}



