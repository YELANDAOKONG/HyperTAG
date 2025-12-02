namespace HyperTAG.Attributes;

/// <summary>
/// Marks a class or struct as supporting automatic Tag serialization
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TagStructAttribute : Attribute
{
    
}