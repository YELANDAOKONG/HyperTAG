# HyperTAG

---

## What is HyperTAG?

HyperTAG was originally the data storage component of my personal simple game engine. It has now been extracted and open-sourced as a standalone library.

HyperTAG supports direct storage of most fundamental C# data types:

```csharp
public enum TagDataType : int
{
    Empty = 0,
    Null = 1,
    Data = 2, // Equivalent to ByteArray, but specifically designed for raw data storage
    Bool = 3,
    Char = 4,
    Byte = 5,
    Short = 6,
    Int = 7,
    Long = 8,
    Float = 9,
    Double = 10,
    String = 11,
    Decimal = 12,
    UShort = 13,
    UInt = 14,
    ULong = 15,
    SByte = 16,
    BoolArray = 17,
    CharArray = 18,
    ByteArray = 19,
    ShortArray = 20,
    IntArray = 21,
    LongArray = 22,
    FloatArray = 23,
    DoubleArray = 24,
    StringArray = 25,
    DecimalArray = 26,
    UShortArray = 27,
    UIntArray = 28,
    ULongArray = 29,
    SByteArray = 30,
    // List = ?,
    // Dictionary = ?,
}
```

> For Lists and Dictionaries, they can be implemented via nested HyperTAG structures, hence they are not natively supported.

A HyperTAG consists of the following components:
-   **Type**: The data type of the tag.
-   **Value**: The value of the current tag.
-   **Entities**: A list of child tags.

## Automatic Serialization

For classes or structs decorated with the `[TagStruct]` attribute or those implementing the `ITagStruct` interface:
The automatic serializer will convert fields and properties marked with the `[TagData]` attribute.

The type of a field/property with the `[TagData]` attribute must either:
1.  Also be decorated with `[TagStruct]` or implement `ITagStruct`, or
2.  Have a registered type converter.

```csharp
[TagStruct]
public class Example
{
    [TagData]
    public string? Title;

    [TagData]
    public int Status = 0;

    [TagData]
    public int[] Data = [0, 1, 2, 3];

    [TagData("ItemData")] // Custom name used for storage
    public ExampleItem Item = new();
}

[TagStruct]
public class ExampleItem
{
    [TagData]
    public string? Name;

    [TagData]
    public int Count;
}
```

## About this project

The project code may contain imperfections or areas for improvement. Contributions are welcome.

---
