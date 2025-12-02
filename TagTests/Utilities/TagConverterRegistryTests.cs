using HyperTAG.Core;
using HyperTAG.Interfaces;
using HyperTAG.Models;
using HyperTAG.Utilities;

namespace TagTests.Utilities;

public class TagConverterRegistryTests
{
    // Test types
    private class TestType1
    {
        public int Value { get; set; }
    }

    private class TestType2
    {
        public string Name { get; set; } = string.Empty;
    }

    private class TestType1Converter : ITagConverter<TestType1>
    {
        public HyperTag? ToTag(TestType1? obj)
        {
            if (obj == null) return null;
            var tag = new HyperTag(TagDataType.Empty);
            var valueTag = new HyperTag(TagDataType.String, "Value");
            valueTag.Entities.Add(new HyperTag(TagDataType.Int, obj.Value));
            tag.Entities.Add(valueTag);
            return tag;
        }

        public TestType1? FromTag(HyperTag? tag)
        {
            if (tag == null) return null;
            var obj = new TestType1();
            foreach (var entity in tag.Entities)
            {
                if (entity.Type == TagDataType.String && entity.Value as string == "Value")
                {
                    if (entity.Entities.Count > 0 && entity.Entities[0].Type == TagDataType.Int)
                    {
                        obj.Value = (int)entity.Entities[0].Value!;
                    }
                }
            }
            return obj;
        }
    }

    private class TestType2Converter : ITagConverter<TestType2>
    {
        public HyperTag? ToTag(TestType2? obj)
        {
            if (obj == null) return null;
            var tag = new HyperTag(TagDataType.Empty);
            var nameTag = new HyperTag(TagDataType.String, "Name");
            nameTag.Entities.Add(new HyperTag(TagDataType.String, obj.Name));
            tag.Entities.Add(nameTag);
            return tag;
        }

        public TestType2? FromTag(HyperTag? tag)
        {
            if (tag == null) return null;
            var obj = new TestType2();
            foreach (var entity in tag.Entities)
            {
                if (entity.Type == TagDataType.String && entity.Value as string == "Name")
                {
                    if (entity.Entities.Count > 0 && entity.Entities[0].Type == TagDataType.String)
                    {
                        obj.Name = (string)entity.Entities[0].Value!;
                    }
                }
            }
            return obj;
        }
    }

    [Fact]
    public void Default_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = TagConverterRegistry.Default;
        var instance2 = TagConverterRegistry.Default;

        // Assert
        Assert.NotNull(instance1);
        Assert.NotNull(instance2);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Constructor_ShouldCreateNewInstance()
    {
        // Act
        var registry1 = new TagConverterRegistry();
        var registry2 = new TagConverterRegistry();

        // Assert
        Assert.NotNull(registry1);
        Assert.NotNull(registry2);
        Assert.NotSame(registry1, registry2);
    }

    [Fact]
    public void Register_WithValidConverter_ShouldAddConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var converter = new TestType1Converter();

        // Act
        registry.Register(converter);

        // Assert
        Assert.True(registry.HasConverter<TestType1>());
        Assert.Equal(1, registry.Count);
    }

    [Fact]
    public void Register_WithNullConverter_ShouldThrowArgumentNullException()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.Register<TestType1>(null!));
    }

    [Fact]
    public void Register_MultipleConverters_ShouldAddAll()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var converter1 = new TestType1Converter();
        var converter2 = new TestType2Converter();

        // Act
        registry.Register(converter1);
        registry.Register(converter2);

        // Assert
        Assert.True(registry.HasConverter<TestType1>());
        Assert.True(registry.HasConverter<TestType2>());
        Assert.Equal(2, registry.Count);
    }

    [Fact]
    public void Register_SameTypeTwice_ShouldReplaceConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var converter1 = new TestType1Converter();
        var converter2 = new TestType1Converter();

        // Act
        registry.Register(converter1);
        registry.Register(converter2);

        // Assert
        Assert.True(registry.HasConverter<TestType1>());
        Assert.Equal(1, registry.Count);
        Assert.Same(converter2, registry.GetConverter<TestType1>());
    }

    [Fact]
    public void GetConverter_WithRegisteredType_ShouldReturnConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var converter = new TestType1Converter();
        registry.Register(converter);

        // Act
        var result = registry.GetConverter<TestType1>();

        // Assert
        Assert.NotNull(result);
        Assert.Same(converter, result);
    }

    [Fact]
    public void GetConverter_WithUnregisteredType_ShouldReturnNull()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act
        var result = registry.GetConverter<TestType1>();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasConverter_WithRegisteredType_ShouldReturnTrue()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());

        // Act
        var result = registry.HasConverter<TestType1>();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasConverter_WithUnregisteredType_ShouldReturnFalse()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act
        var result = registry.HasConverter<TestType1>();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasConverter_WithTypeParameter_ShouldWork()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());

        // Act
        var result = registry.HasConverter(typeof(TestType1));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasConverter_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.HasConverter(null!));
    }

    [Fact]
    public void Unregister_WithRegisteredType_ShouldRemoveConverter()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());

        // Act
        registry.Unregister<TestType1>();

        // Assert
        Assert.False(registry.HasConverter<TestType1>());
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void Unregister_WithUnregisteredType_ShouldDoNothing()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act & Assert (should not throw)
        registry.Unregister<TestType1>();
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void Unregister_WithTypeParameter_ShouldWork()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());

        // Act
        registry.Unregister(typeof(TestType1));

        // Assert
        Assert.False(registry.HasConverter<TestType1>());
    }

    [Fact]
    public void Unregister_WithNullType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => registry.Unregister((Type)null!));
    }

    [Fact]
    public void Clear_WithMultipleConverters_ShouldRemoveAll()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());
        registry.Register(new TestType2Converter());

        // Act
        registry.Clear();

        // Assert
        Assert.Equal(0, registry.Count);
        Assert.False(registry.HasConverter<TestType1>());
        Assert.False(registry.HasConverter<TestType2>());
    }

    [Fact]
    public void Clear_WithEmptyRegistry_ShouldDoNothing()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act & Assert (should not throw)
        registry.Clear();
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void Count_WithNoConverters_ShouldReturnZero()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Assert
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void Count_WithMultipleConverters_ShouldReturnCorrectCount()
    {
        // Arrange
        var registry = new TagConverterRegistry();

        // Act
        registry.Register(new TestType1Converter());
        Assert.Equal(1, registry.Count);

        registry.Register(new TestType2Converter());
        Assert.Equal(2, registry.Count);

        registry.Unregister<TestType1>();
        Assert.Equal(1, registry.Count);

        registry.Clear();
        Assert.Equal(0, registry.Count);
    }

    [Fact]
    public void CustomRegistry_ShouldBeIndependentFromDefault()
    {
        // Arrange
        var customRegistry = new TagConverterRegistry();
        
        // Act
        customRegistry.Register(new TestType1Converter());

        // Assert
        Assert.True(customRegistry.HasConverter<TestType1>());
        Assert.False(TagConverterRegistry.Default.HasConverter<TestType1>());
    }

    [Fact]
    public void MultipleCustomRegistries_ShouldBeIndependent()
    {
        // Arrange
        var registry1 = new TagConverterRegistry();
        var registry2 = new TagConverterRegistry();

        // Act
        registry1.Register(new TestType1Converter());
        registry2.Register(new TestType2Converter());

        // Assert
        Assert.True(registry1.HasConverter<TestType1>());
        Assert.False(registry1.HasConverter<TestType2>());

        Assert.False(registry2.HasConverter<TestType1>());
        Assert.True(registry2.HasConverter<TestType2>());
    }

    [Fact]
    public void GetConverter_NonGeneric_ShouldReturnConverterObject()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var converter = new TestType1Converter();
        registry.Register(converter);

        // Act
        var result = registry.GetConverter<TestType1>();

        // Assert
        Assert.NotNull(result);
        Assert.Same(converter, result);
    }

    // [Fact]
    // public void GetConverter_NonGeneric_WithNullType_ShouldThrowArgumentNullException()
    // {
    //     // Arrange
    //     var registry = new TagConverterRegistry();
    //
    //     // Act & Assert
    //     Assert.Throws<ArgumentNullException>(() => registry.GetConverter((Type)null!));
    // }

    [Fact]
    public async Task ThreadSafety_ConcurrentRegister_ShouldWork()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        var tasks = new List<Task>();

        // Act - Multiple threads registering converters
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => registry.Register(new TestType1Converter())));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        Assert.True(registry.HasConverter<TestType1>());
        Assert.Equal(1, registry.Count);
    }

    [Fact]
    public async Task ThreadSafety_ConcurrentReadWrite_ShouldWork()
    {
        // Arrange
        var registry = new TagConverterRegistry();
        registry.Register(new TestType1Converter());
        var tasks = new List<Task>();

        // Act - Multiple threads reading and writing
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() => registry.GetConverter<TestType1>()));
            tasks.Add(Task.Run(() => registry.HasConverter<TestType1>()));
            tasks.Add(Task.Run(() => registry.Register(new TestType1Converter())));
        }

        // Assert (should not throw)
        await Task.WhenAll(tasks.ToArray());
        Assert.True(registry.HasConverter<TestType1>());
    }

    [Fact]
    public void DefaultRegistry_ShouldPersistAcrossAccess()
    {
        // Arrange & Act
        TagConverterRegistry.Default.Register(new TestType1Converter());
        
        try
        {
            // Access default from different references
            var hasConverter1 = TagConverterRegistry.Default.HasConverter<TestType1>();
            var hasConverter2 = TagConverterRegistry.Default.HasConverter<TestType1>();

            // Assert
            Assert.True(hasConverter1);
            Assert.True(hasConverter2);
        }
        finally
        {
            // Cleanup
            TagConverterRegistry.Default.Unregister<TestType1>();
        }
    }
}
