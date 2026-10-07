using System.Collections.Generic;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class DictionaryKeyComparerTests
{
	[Test]
	public async Task Read_WhenDictionaryHasOtherTypeArguments_ShouldReturnNull()
	{
		DictionaryKeyComparer sut = DictionaryKeyComparer.Create<string, int>();

		object? result = sut.Read(new Dictionary<int, int>());

		await That(result).IsNull();
	}

	[Test]
	public async Task Read_WhenKeyIsNotOfTheKeyType_ShouldCompareItWithItsOwnEquals()
	{
		DictionaryKeyComparer sut = DictionaryKeyComparer.Create<string, int>();
		IEqualityComparer<object> comparer = (IEqualityComparer<object>)sut.Read(
			new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase))!;

		bool isSameAsString = comparer.Equals(1, "1");
		bool isSameAsInt = comparer.Equals("1", 1);
		bool isSameAsEqualInt = comparer.Equals(1, 1);
		int hashCode = comparer.GetHashCode(1);

		await That(isSameAsString).IsFalse();
		await That(isSameAsInt).IsFalse();
		await That(isSameAsEqualInt).IsTrue()
			.Because("a key that is not of the key type is never handed to the comparer, which would reject it");
		await That(hashCode).IsEqualTo(1.GetHashCode());
	}

	[Test]
	public async Task Read_WhenNoKeyIsOfTheKeyType_ShouldUseTheirOwnEquality()
	{
		IEqualityComparer<object> sut = Read(new Dictionary<MyKey, int>(new MyKeyComparer()));

		await That(sut.Equals(new OtherKey(), new OtherKey())).IsTrue();
		await That(sut.Equals(new OtherKey(), "foo")).IsFalse();
	}

	[Test]
	public async Task Read_WhenOnlyOneKeyIsOfTheKeyType_ShouldNotBeEqual()
	{
		IEqualityComparer<object> sut = Read(new Dictionary<MyKey, int>(new MyKeyComparer()));
		MyKey key = new();
		OtherKey other = new();

		await That(key.Equals(other)).IsTrue();
		await That(sut.GetHashCode(key)).IsNotEqualTo(sut.GetHashCode(other));
		await That(sut.Equals(key, other)).IsFalse();
		await That(sut.Equals(other, key)).IsFalse();
	}

	private static IEqualityComparer<object> Read(Dictionary<MyKey, int> dictionary)
		=> (IEqualityComparer<object>)DictionaryKeyComparer.Create<MyKey, int>().Read(dictionary)!;

	private sealed class MyKey
	{
		public override bool Equals(object? obj) => obj is MyKey or OtherKey;
		public override int GetHashCode() => 1;
	}

	private sealed class OtherKey
	{
		public override bool Equals(object? obj) => obj is MyKey or OtherKey;
		public override int GetHashCode() => 1;
	}

	private sealed class MyKeyComparer : IEqualityComparer<MyKey>
	{
		public bool Equals(MyKey? x, MyKey? y) => true;
		public int GetHashCode(MyKey obj) => 2;
	}
}
