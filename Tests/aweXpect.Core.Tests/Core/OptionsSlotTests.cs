using aweXpect.Options;

namespace aweXpect.Core.Tests.Core;

public sealed class OptionsSlotTests
{
	[Test]
	public async Task SetMatch_ShouldSetTheMatchOnTheValue()
	{
		OptionsSlot<CollectionIndexOptions> sut = new(new CollectionIndexOptions());

		OptionsSlot<CollectionIndexOptions> result = sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(result).IsSameAs(sut);
		await That(sut.Value.Match).Is<MyMatch>();
	}

	[Test]
	public async Task ToString_ShouldReturnTheTextOfTheValue()
	{
		OptionsSlot<StringEqualityOptions> sut = new(new StringEqualityOptions("expected"));
		sut.IgnoringCase();

		string result = sut.ToString();

		await That(result).IsEqualTo(" ignoring case")
			.Because("an expectation text can append the slot like the options it holds");
	}

	[Test]
	public async Task ToString_WhenValueIsNull_ShouldReturnEmptyString()
	{
		OptionsSlot<string?> sut = new(null);

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task Value_ShouldBeReadableAsReadOnlySlotOfABaseType()
	{
		StringEqualityOptions options = new("expected");
		OptionsSlot<StringEqualityOptions> sut = new(options);

		IReadOnlyOptionsSlot<object> result = sut;

		await That(result.Value).IsSameAs(options);
	}

	[Test]
	public async Task Value_WhenReplaced_ShouldReturnTheNewValue()
	{
		OptionsSlot<Quantifier> sut = new(new Quantifier());
		Quantifier replacement = new();

		sut.Value = replacement;

		await That(sut.Value).IsSameAs(replacement);
	}

	private sealed class MyMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " at my index";

		public bool OnlySingleIndex() => false;
	}
}
