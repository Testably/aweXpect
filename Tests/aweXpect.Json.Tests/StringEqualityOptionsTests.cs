using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed class StringEqualityOptionsTests
{
	[Fact]
	public async Task EqualJson_ShouldReturnEmptyFailure()
	{
		string actual = "{}";
		string expected = "{  }";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That(actual).IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		var result = optionsProvider.Options.AreConsideredEqual(actual, expected);
		var failure = optionsProvider.Options.GetExtendedFailure("it", actual, expected);

		await That(result).IsTrue();
		await That(failure).IsEmpty();
	}
}
