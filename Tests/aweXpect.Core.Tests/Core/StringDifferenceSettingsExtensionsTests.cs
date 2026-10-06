namespace aweXpect.Core.Tests.Core;

public class StringDifferenceSettingsExtensionTests
{
	[Test]
	public async Task ShouldInitializeToEqualityMatchType()
	{
		StringDifferenceSettings settings = new(2, 3);

		await That(settings.MatchType).IsEqualTo(StringDifference.MatchType.Equality);
		await That(settings.IgnoredTrailingLines).IsEqualTo(2);
		await That(settings.IgnoredTrailingColumns).IsEqualTo(3);
	}

	[Test]
	[Arguments(StringDifference.MatchType.Wildcard)]
	[Arguments(StringDifference.MatchType.Regex)]
	public async Task WithMatchType_ShouldSetMatchTypeAndKeepOtherValues(StringDifference.MatchType matchType)
	{
		StringDifferenceSettings settings = new(2, 3);

		settings.WithMatchType(matchType);

		await That(settings.MatchType).IsEqualTo(matchType);
		await That(settings.IgnoredTrailingLines).IsEqualTo(2);
		await That(settings.IgnoredTrailingColumns).IsEqualTo(3);
	}
}
