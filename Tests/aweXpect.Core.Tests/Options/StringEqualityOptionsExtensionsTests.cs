using System.Text.RegularExpressions;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Options;

public sealed class StringEqualityOptionsExtensionsTests
{
	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldKeepTheMatchTypeSetSoFar()
	{
		CopyingResult sut = new(StringEqualityOptions.Default);

		void Act() => sut.AsPrefix().AsSuffix();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AsSuffix cannot be combined with AsPrefix.");
	}

	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldReplaceARepeatedOptionOnTheCopy()
	{
		CopyingResult sut = new(StringEqualityOptions.Default);
		CopyingResult ignoringCase = sut.IgnoringCase();

		CopyingResult result = ignoringCase.IgnoringCase(false);

		await That(result.Options.ToString()).IsEqualTo(sut.Options.ToString());
		await That(ignoringCase.Options.ToString()).EndsWith(" ignoring case")
			.Because("the repeated option must be set on a copy of the options");
		await That(() => ignoringCase.Using(StringComparer.Ordinal)).Throws<InvalidOperationException>()
			.WithMessage(
				"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
			.Because("the copy carries the options set so far, so incompatible options still throw");
	}

	[Test]
	[Arguments("AsPrefix")]
	[Arguments("AsRegex")]
	[Arguments("AsRegex(RegexOptions)")]
	[Arguments("AsSuffix")]
	[Arguments("AsWildcard")]
	[Arguments("IgnoringCase")]
	[Arguments("IgnoringIndentation")]
	[Arguments("IgnoringLeadingWhiteSpace")]
	[Arguments("IgnoringNewlineStyle")]
	[Arguments("IgnoringTrailingWhiteSpace")]
	[Arguments("NormalizingWhiteSpace")]
	[Arguments("Using")]
	public async Task WhenTheResultCopiesOnOption_ShouldSetTheOptionOnACopy(string option)
	{
		CopyingResult sut = new(StringEqualityOptions.Default);
		string unchangedOptions = sut.Options.ToString();

		CopyingResult result = Apply(sut, option);

		await That(result).IsNotSameAs(sut);
		await That(result.Options).IsNotSameAs(sut.Options);
		await That(result.Options.ToString()).IsNotEqualTo(unchangedOptions);
		await That(sut.Options.ToString()).IsEqualTo(unchangedOptions)
			.Because("the option must be set on a copy of the options");
	}

	[Test]
	[Arguments("AsPrefix")]
	[Arguments("AsRegex")]
	[Arguments("AsRegex(RegexOptions)")]
	[Arguments("AsSuffix")]
	[Arguments("AsWildcard")]
	[Arguments("IgnoringCase")]
	[Arguments("IgnoringIndentation")]
	[Arguments("IgnoringLeadingWhiteSpace")]
	[Arguments("IgnoringNewlineStyle")]
	[Arguments("IgnoringTrailingWhiteSpace")]
	[Arguments("NormalizingWhiteSpace")]
	[Arguments("Using")]
	public async Task WhenTheResultDoesNotCopyOnOption_ShouldSetTheOptionInPlace(string option)
	{
		InPlaceResult sut = new(StringEqualityOptions.Default);
		OptionsHolder<StringEqualityOptions> holder = sut.Options;
		string unchangedOptions = holder.Value.ToString();

		InPlaceResult result = Apply(sut, option);

		await That(result).IsSameAs(sut);
		await That(result.Options).IsSameAs(holder);
		await That(holder.Value.ToString()).IsNotEqualTo(unchangedOptions);
	}

	private static TResult Apply<TResult>(TResult result, string option)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		=> option switch
		{
			"AsPrefix" => result.AsPrefix(),
			"AsRegex" => result.AsRegex(),
			"AsRegex(RegexOptions)" => result.AsRegex(RegexOptions.Multiline),
			"AsSuffix" => result.AsSuffix(),
			"AsWildcard" => result.AsWildcard(),
			"IgnoringCase" => result.IgnoringCase(),
			"IgnoringIndentation" => result.IgnoringIndentation(),
			"IgnoringLeadingWhiteSpace" => result.IgnoringLeadingWhiteSpace(),
			"IgnoringNewlineStyle" => result.IgnoringNewlineStyle(),
			"IgnoringTrailingWhiteSpace" => result.IgnoringTrailingWhiteSpace(),
			"NormalizingWhiteSpace" => result.NormalizingWhiteSpace(),
			"Using" => result.Using(StringComparer.OrdinalIgnoreCase),
			_ => throw new ArgumentOutOfRangeException(nameof(option)),
		};

	private sealed class CopyingResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, CopyingResult>
	{
		public OptionsHolder<StringEqualityOptions> Options { get; } = new(options);

		public CopyingResult WithOptions(StringEqualityOptions copiedOptions) => new(copiedOptions);
	}

	private sealed class InPlaceResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions
	{
		public OptionsHolder<StringEqualityOptions> Options { get; } = new(options);
	}
}
