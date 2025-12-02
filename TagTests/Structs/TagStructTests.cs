using HyperTAG.Models;
using HyperTAG.Structs;

namespace TagTests.Structs;

public class TagStructTests
{
    [Fact]
    public void Constructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        var tagStruct = new TagStruct();

        // Assert
        Assert.Equal(TagDataType.Empty, tagStruct.Type);
        Assert.Null(tagStruct.Value);
        Assert.NotNull(tagStruct.Entities);
        Assert.Empty(tagStruct.Entities);
    }

    [Fact]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange & Act
        var tagStruct = new TagStruct(TagDataType.String, "test value");

        // Assert
        Assert.Equal(TagDataType.String, tagStruct.Type);
        Assert.Equal("test value", tagStruct.Value);
        Assert.NotNull(tagStruct.Entities);
        Assert.Empty(tagStruct.Entities);
    }

    [Fact]
    public void Entities_ShouldBeMutable()
    {
        // Arrange
        var tagStruct = new TagStruct();
        var subTag = new TagStruct(TagDataType.Int, 42);

        // Act
        tagStruct.Entities.Add(subTag);

        // Assert
        Assert.Single(tagStruct.Entities);
        Assert.Equal(subTag, tagStruct.Entities[0]);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var tagStruct = new TagStruct();

        // Act
        tagStruct.Type = TagDataType.Bool;
        tagStruct.Value = true;

        // Assert
        Assert.Equal(TagDataType.Bool, tagStruct.Type);
        Assert.True((bool)tagStruct.Value!);
    }

    [Fact]
    public void MultipleEntities_ShouldBeSupported()
    {
        // Arrange
        var tagStruct = new TagStruct();
        var subTag1 = new TagStruct(TagDataType.String, "first");
        var subTag2 = new TagStruct(TagDataType.Int, 2);
        var subTag3 = new TagStruct(TagDataType.Bool, true);

        // Act
        tagStruct.Entities.Add(subTag1);
        tagStruct.Entities.Add(subTag2);
        tagStruct.Entities.Add(subTag3);

        // Assert
        Assert.Equal(3, tagStruct.Entities.Count);
        Assert.Equal("first", tagStruct.Entities[0].Value);
        Assert.Equal(2, tagStruct.Entities[1].Value);
        Assert.True((bool)tagStruct.Entities[2].Value!);
    }

    [Fact]
    public void NestedEntities_ShouldBeSupported()
    {
        // Arrange
        var root = new TagStruct(TagDataType.String, "root");
        var child = new TagStruct(TagDataType.Int, 1);
        var grandchild = new TagStruct(TagDataType.Bool, true);

        // Act
        child.Entities.Add(grandchild);
        root.Entities.Add(child);

        // Assert
        Assert.Single(root.Entities);
        Assert.Single(root.Entities[0].Entities);
        Assert.True((bool)root.Entities[0].Entities[0].Value!);
    }
}
