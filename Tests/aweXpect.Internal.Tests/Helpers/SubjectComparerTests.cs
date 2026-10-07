using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class SubjectComparerTests
{
	[Test]
	[Arguments("a", "A", true)]
	[Arguments("a", "b", false)]
	[Arguments(null, null, true)]
	[Arguments("a", null, false)]
	[Arguments(null, "a", false)]
	[Arguments("a", 1, false)]
	[Arguments(1, "a", false)]
	[Arguments(1, 1, false)]
	public async Task AreEqual_ShouldOnlyCompareTwoValuesOfTheItemType(object? actual, object? expected,
		bool expectedResult)
	{
		SubjectComparer<string> comparer = new(
			(a, e) => string.Equals(a, e, StringComparison.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

		bool result = comparer.AreEqual(actual, expected);

		await That(result).IsEqualTo(expectedResult);
	}
}
