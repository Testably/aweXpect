using aweXpect.Options;

namespace aweXpect.Core.Tests.Core;

public sealed class OptionsProviderTests
{
	[Test]
	public async Task WithOptions_WhenTheResultCopiesOnOption_ShouldReturnACopyWithTheOptions()
	{
		CopyingResult sut = new(StringEqualityOptions.Default);
		StringEqualityOptions options = StringEqualityOptions.Default.IgnoringCase();

		CopyingResult result = OptionsProvider.WithOptions(sut, options);

		await That(result).IsNotSameAs(sut);
		await That(result.Options.Value).IsSameAs(options);
		await That(sut.Options.Value).IsSameAs(StringEqualityOptions.Default)
			.Because("a result that copies on option must stay unchanged");
	}

	[Test]
	public async Task WithOptions_WhenTheResultDoesNotCopyOnOption_ShouldSetTheOptionsInItsHolder()
	{
		InPlaceResult sut = new(StringEqualityOptions.Default);
		OptionsHolder<StringEqualityOptions> holder = sut.Options;
		StringEqualityOptions options = StringEqualityOptions.Default.IgnoringCase();

		InPlaceResult result = OptionsProvider.WithOptions(sut, options);

		await That(result).IsSameAs(sut);
		await That(result.Options).IsSameAs(holder);
		await That(holder.Value).IsSameAs(options);
	}

	private sealed class CopyingResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			ICopyOnOption<StringEqualityOptions, CopyingResult>
	{
		public OptionsHolder<StringEqualityOptions> Options { get; } = new(options);

		public CopyingResult WithOptions(StringEqualityOptions copiedOptions) => new(copiedOptions);
	}

	private sealed class InPlaceResult(StringEqualityOptions options) : IOptionsProvider<StringEqualityOptions>
	{
		public OptionsHolder<StringEqualityOptions> Options { get; } = new(options);
	}
}
