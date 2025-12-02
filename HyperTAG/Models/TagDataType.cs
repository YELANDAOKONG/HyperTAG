namespace HyperTAG.Models;

[Serializable]
public enum TagDataType : int
{
    // Supported Data Types: 
    // - empty
    // - null
    // - data
    // - bool
    // - char
    // - byte
    // - short
    // - int
    // - long
    // - float
    // - double
    // - string
    // - decimal
    // - ushort
    // - uint
    // - ulong
    // - sbyte
    // - bool[]
    // - char[]
    // - byte[]
    // - short[]
    // - int[]
    // - long[]
    // - float[]
    // - double[]
    // - string[]
    // - decimal[]
    // - ushort[]
    // - uint[]
    // - ulong[]
    // - sbyte[]
    // Unsupported Data Types:
    // - list
    // - dictionary
    
    Empty = 0,
    Null = 1,
    Data = 2,
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