using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Xml.Tests;

public sealed class XmlExtensionsTests
{
	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldSetAsXmlOnACopy()
	{
		CopyingResult sut = new(StringEqualityOptions.Default);
		string unchangedOptions = sut.Options.Value.ToString();

		CopyingResult result = sut.AsXml();

		await That(result).IsNotSameAs(sut);
		await That(result.Options.Value.ToString()).IsNotEqualTo(unchangedOptions);
		await That(sut.Options.Value.ToString()).IsEqualTo(unchangedOptions)
			.Because("a stored result must not be changed by an option on its copy");
		await That(() => sut.IgnoringCase()).DoesNotThrow()
			.Because("the stored result has no XML match type that the option could compete with");
	}

	private sealed class CopyingResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, CopyingResult>
	{
		public OptionsSlot<StringEqualityOptions> Options { get; } = new(options);

		public CopyingResult WithOptions(StringEqualityOptions copiedOptions) => new(copiedOptions);
	}
}
