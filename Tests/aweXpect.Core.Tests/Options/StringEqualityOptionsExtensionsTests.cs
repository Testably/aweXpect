using System.Text.RegularExpressions;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Options;

public sealed class StringEqualityOptionsExtensionsTests
{
	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldKeepTheMatchTypeSetSoFar()
	{
		CopyingResult sut = new(new StringEqualityOptions("expected"));

		void Act() => sut.AsPrefix().AsSuffix();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AsSuffix cannot be combined with AsPrefix.");
	}

	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldKeepTheOptionsSetSoFar()
	{
		CopyingResult sut = new(new StringEqualityOptions("expected"));

		void Act() => sut.IgnoringCase().IgnoringCase();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("IgnoringCase cannot be specified more than once.");
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
		CopyingResult sut = new(new StringEqualityOptions("expected"));
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
		StringEqualityOptions options = new("expected");
		InPlaceResult sut = new(options);
		string unchangedOptions = options.ToString();

		InPlaceResult result = Apply(sut, option);

		await That(result).IsSameAs(sut);
		await That(result.Options).IsSameAs(options);
		await That(options.ToString()).IsNotEqualTo(unchangedOptions);
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
		public StringEqualityOptions Options => options;

		public CopyingResult WithOptions(StringEqualityOptions copiedOptions) => new(copiedOptions);
	}

	private sealed class InPlaceResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions
	{
		public StringEqualityOptions Options => options;
	}
}
