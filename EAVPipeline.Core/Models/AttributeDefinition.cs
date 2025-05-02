namespace EAVPipeline.Core.Models;

public enum AttributeDataType{
    String, 
    Double, 
    Integer, 
    Boolean,
    DateTime
}

public class AttributeDefinition{
    public string Slug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AttributeDataType DataType { get; set; } = AttributeDataType.String;
    public bool Required { get; set; }
    public bool IsEnum { get; set; }
    public List<string>? EnumOptions { get; set; }
}



