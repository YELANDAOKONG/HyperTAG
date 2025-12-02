using HyperTAG.Attributes;
using HyperTAG.Converters;
using HyperTAG.Core;
using HyperTAG.Exceptions;
using HyperTAG.Models;

namespace TagTests.Converters;

public class DictionaryConverterTests
{
    [TagStruct]
    public class TestData
    {
        [TagData]
        public string Info = string.Empty;
        
        [TagData]
        public int Value;

        public override bool Equals(object? obj)
        {
            if (obj is not TestData other) return false;
            return Info == other.Info && Value == other.Value;
        }

        public override int GetHashCode() => HashCode.Combine(Info, Value);
    }

    [Fact]
    public void Constructor_WithNonBasicKeyType_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<TagAutoSerializeException>(() => 
            new DictionaryConverter<TestData, int>());
    }

    [Fact]
    public void ToTag_WithStringIntDictionary_ReturnsValidTag()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var dict = new Dictionary<string, int>
        {
            ["one"] = 1,
            ["two"] = 2,
            ["three"] = 3
        };

        // Act
        var tag = converter.ToTag(dict);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(3, tag.Entities.Count);
        
        // Each entity should be a pair container (Empty) with 2 children (key, value)
        foreach (var pairTag in tag.Entities)
        {
            Assert.Equal(TagDataType.Empty, pairTag.Type);
            Assert.Equal(2, pairTag.Entities.Count);
            Assert.Equal(TagDataType.String, pairTag.Entities[0].Type); // Key
            Assert.Equal(TagDataType.Int, pairTag.Entities[1].Type);    // Value
        }
    }

    [Fact]
    public void ToTag_WithIntStringDictionary_ReturnsValidTag()
    {
        // Arrange
        var converter = new DictionaryConverter<int, string>();
        var dict = new Dictionary<int, string>
        {
            [1] = "apple",
            [2] = "banana",
            [3] = "cherry"
        };

        // Act
        var tag = converter.ToTag(dict);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(3, tag.Entities.Count);
        
        foreach (var pairTag in tag.Entities)
        {
            Assert.Equal(TagDataType.Empty, pairTag.Type);
            Assert.Equal(2, pairTag.Entities.Count);
            Assert.Equal(TagDataType.Int, pairTag.Entities[0].Type);    // Key
            Assert.Equal(TagDataType.String, pairTag.Entities[1].Type); // Value
        }
    }

    [Fact]
    public void ToTag_WithEmptyDictionary_ReturnsEmptyTag()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var dict = new Dictionary<string, int>();

        // Act
        var tag = converter.ToTag(dict);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Empty(tag.Entities);
    }

    [Fact]
    public void ToTag_WithNullValues_HandlesNullsCorrectly()
    {
        // Arrange
        var converter = new DictionaryConverter<string, string>();
        var dict = new Dictionary<string, string?>
        {
            ["key1"] = "value1",
            ["key2"] = null,
            ["key3"] = "value3"
        };

        // Act
        var tag = converter.ToTag(dict!);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(3, tag.Entities.Count);
        
        // Find the pair with key2
        var key2Pair = tag.Entities.FirstOrDefault(p => 
            p.Entities[0].Value?.ToString() == "key2");
        Assert.NotNull(key2Pair);
        Assert.Equal(TagDataType.Null, key2Pair.Entities[1].Type);
    }

    [Fact]
    public void ToTag_WithComplexValues_ReturnsValidTag()
    {
        // Arrange
        var converter = new DictionaryConverter<int, TestData>();
        var dict = new Dictionary<int, TestData>
        {
            [1] = new TestData { Info = "first", Value = 100 },
            [2] = new TestData { Info = "second", Value = 200 }
        };

        // Act
        var tag = converter.ToTag(dict);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Empty, tag.Type);
        Assert.Equal(2, tag.Entities.Count);
        
        foreach (var pairTag in tag.Entities)
        {
            Assert.Equal(2, pairTag.Entities.Count);
            Assert.Equal(TagDataType.Int, pairTag.Entities[0].Type); // Key
            // Value should be a complex nested structure
            Assert.Equal(TagDataType.Empty, pairTag.Entities[1].Type);
        }
    }

    [Fact]
    public void ToTag_WithNullInput_ReturnsNullTag()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();

        // Act
        var tag = converter.ToTag(null);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(TagDataType.Null, tag.Type);
    }

    [Fact]
    public void FromTag_WithStringIntDictionary_ReturnsValidDictionary()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var original = new Dictionary<string, int>
        {
            ["alpha"] = 10,
            ["beta"] = 20,
            ["gamma"] = 30
        };
        var tag = converter.ToTag(original);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(10, result["alpha"]);
        Assert.Equal(20, result["beta"]);
        Assert.Equal(30, result["gamma"]);
    }

    [Fact]
    public void FromTag_WithEmptyTag_ReturnsEmptyDictionary()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var tag = new HyperTag(TagDataType.Empty);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void FromTag_WithNullTag_ReturnsNull()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();

        // Act
        var result = converter.FromTag(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithNullTypeTag_ReturnsNull()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var tag = new HyperTag(TagDataType.Null);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromTag_WithInvalidType_ThrowsException()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var invalidTag = new HyperTag(TagDataType.String, "invalid");

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => 
            converter.FromTag(invalidTag));
    }

    [Fact]
    public void FromTag_WithInvalidPairStructure_ThrowsException()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var tag = new HyperTag(TagDataType.Empty);
        
        // Add invalid pair (should have 2 entities, but we add 3)
        var invalidPair = new HyperTag(TagDataType.Empty);
        invalidPair.Entities.Add(new HyperTag(TagDataType.String, "key"));
        invalidPair.Entities.Add(new HyperTag(TagDataType.Int, 1));
        invalidPair.Entities.Add(new HyperTag(TagDataType.Int, 2)); // Extra entity
        tag.Entities.Add(invalidPair);

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => 
            converter.FromTag(tag));
    }

    [Fact]
    public void FromTag_WithKeyTypeMismatch_ThrowsException()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var tag = new HyperTag(TagDataType.Empty);
        
        var pair = new HyperTag(TagDataType.Empty);
        pair.Entities.Add(new HyperTag(TagDataType.Int, 123)); // Wrong key type
        pair.Entities.Add(new HyperTag(TagDataType.Int, 456));
        tag.Entities.Add(pair);

        // Act & Assert
        Assert.Throws<TagAutoDeserializeException>(() => 
            converter.FromTag(tag));
    }

    [Fact]
    public void FromTag_WithComplexValues_ReturnsValidDictionary()
    {
        // Arrange
        var converter = new DictionaryConverter<int, TestData>();
        var original = new Dictionary<int, TestData>
        {
            [1] = new TestData { Info = "data1", Value = 111 },
            [2] = new TestData { Info = "data2", Value = 222 }
        };
        var tag = converter.ToTag(original);

        // Act
        var result = converter.FromTag(tag);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("data1", result[1].Info);
        Assert.Equal(111, result[1].Value);
        Assert.Equal("data2", result[2].Info);
        Assert.Equal(222, result[2].Value);
    }

    [Fact]
    public void RoundTrip_WithStringIntDictionary_PreservesData()
    {
        // Arrange
        var converter = new DictionaryConverter<string, int>();
        var original = new Dictionary<string, int>
        {
            ["red"] = 1,
            ["green"] = 2,
            ["blue"] = 3
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);
        foreach (var kvp in original)
        {
            Assert.True(restored.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, restored[kvp.Key]);
        }
    }

    [Fact]
    public void RoundTrip_WithIntStringDictionary_PreservesData()
    {
        // Arrange
        var converter = new DictionaryConverter<int, string>();
        var original = new Dictionary<int, string>
        {
            [100] = "hundred",
            [200] = "two hundred",
            [300] = "three hundred"
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithNullValues_PreservesNulls()
    {
        // Arrange
        var converter = new DictionaryConverter<string, string>();
        var original = new Dictionary<string, string?>
        {
            ["a"] = "value_a",
            ["b"] = null,
            ["c"] = "value_c"
        };

        // Act
        var tag = converter.ToTag(original!);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(3, restored.Count);
        Assert.Equal("value_a", restored["a"]);
        Assert.Null(restored["b"]);
        Assert.Equal("value_c", restored["c"]);
    }

    [Fact]
    public void RoundTrip_WithComplexValues_PreservesData()
    {
        // Arrange
        var converter = new DictionaryConverter<int, TestData>();
        var original = new Dictionary<int, TestData>
        {
            [10] = new TestData { Info = "ten", Value = 10 },
            [20] = new TestData { Info = "twenty", Value = 20 },
            [30] = new TestData { Info = "thirty", Value = 30 }
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);
        foreach (var kvp in original)
        {
            Assert.True(restored.ContainsKey(kvp.Key));
            Assert.Equal(kvp.Value, restored[kvp.Key]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(20)]
    public void RoundTrip_WithVariousSizes_PreservesCount(int count)
    {
        // Arrange
        var converter = new DictionaryConverter<int, string>();
        var original = new Dictionary<int, string>();
        for (int i = 0; i < count; i++)
        {
            original[i] = $"value_{i}";
        }

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(count, restored.Count);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithBoolKeys_PreservesData()
    {
        // Arrange
        var converter = new DictionaryConverter<bool, string>();
        var original = new Dictionary<bool, string>
        {
            [true] = "yes",
            [false] = "no"
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_WithDoubleValues_PreservesData()
    {
        // Arrange
        var converter = new DictionaryConverter<string, double>();
        var original = new Dictionary<string, double>
        {
            ["pi"] = 3.14159,
            ["e"] = 2.71828,
            ["sqrt2"] = 1.41421
        };

        // Act
        var tag = converter.ToTag(original);
        var restored = converter.FromTag(tag);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(original.Count, restored.Count);
        foreach (var kvp in original)
        {
            Assert.Equal(kvp.Value, restored[kvp.Key], 5);
        }
    }
}
