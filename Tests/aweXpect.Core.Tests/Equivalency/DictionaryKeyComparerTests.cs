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
}
