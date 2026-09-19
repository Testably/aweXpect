namespace aweXpect.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class ThatEnum
{
	public enum EnumByte : byte
	{
		Min = byte.MinValue,
		Max = byte.MaxValue,
	}

	public enum EnumInt
	{
		Min = int.MinValue,
		Max = int.MaxValue,
	}

	public enum EnumLong : long
	{
		Int64Min = long.MinValue,
		Int64Max = long.MaxValue,
		Int64LessOne = long.MaxValue - 1,
		Int64LessTwo = long.MaxValue - 2,
	}

	public enum EnumSByte : sbyte
	{
		Min = sbyte.MinValue,
		Max = sbyte.MaxValue,
	}

	public enum EnumShort : short
	{
		Min = short.MinValue,
		Max = short.MaxValue,
	}

	public enum EnumUInt : uint
	{
		Min = uint.MinValue,
		Max = uint.MaxValue,
	}

	public enum EnumULong : ulong
	{
		Int64Max = long.MaxValue,
		Int64MaxPlusOne = (ulong)long.MaxValue + 1,
		UInt64LessOne = ulong.MaxValue - 1,
		UInt64Max = ulong.MaxValue,
	}

	public enum EnumUShort : ushort
	{
		Min = ushort.MinValue,
		Max = ushort.MaxValue,
	}

	[Flags]
	public enum MyColors
	{
		Blue = 1 << 0,
		Green = 1 << 1,
		Yellow = 1 << 2,
		Red = 1 << 3,
	}

	public enum MyNumbers
	{
		One = 1,
		Two = 2,
		Three = 3,
	}
}
