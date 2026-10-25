using aweXpect.Options;

namespace aweXpect.Core.Tests.Core;

public sealed class OptionsSlotTests
{
	[Test]
	public async Task SetMatch_ShouldSetTheMatchOnTheValue()
	{
		OptionsSlot<CollectionIndexOptions> sut = new(CollectionIndexOptions.Default);

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
		OptionsSlot<Quantifier> sut = new(Quantifier.Default);
		Quantifier replacement = Quantifier.Default.AtLeast(3);

		sut.Value = replacement;

		await That(sut.Value.ToString()).IsEqualTo("at least 3 times");
	}

	[Test]
	public async Task WhenACollectionMatchOptionsPresetIsShared_ShouldNotBeChangedByAnotherSlot()
	{
		CollectionMatchOptions preset = new CollectionMatchOptions().IgnoringDuplicates();
		OptionsSlot<CollectionMatchOptions> first = new(preset);
		OptionsSlot<CollectionMatchOptions> second = new(preset);

		first.InAnyOrder();

		await That(second.Value.GetExpectation("[1]", ExpectationGrammars.None))
			.IsEqualTo("is equal to collection [1] in order ignoring duplicates");
		await That(first.Value.GetExpectation("[1]", ExpectationGrammars.None))
			.IsEqualTo("is equal to collection [1] in any order ignoring duplicates");
	}

	[Test]
	public async Task WhenANumberTolerancePresetIsShared_ShouldNotBeChangedByAnotherSlot()
	{
		NumberTolerance<int> preset = new NumberTolerance<int>((a, b) => Math.Abs(a - b)).Within(2);
		OptionsSlot<NumberTolerance<int>> first = new(preset);
		OptionsSlot<NumberTolerance<int>> second = new(preset);

		first.Within(3);

		await That(second.Value.Tolerance).IsEqualTo(2);
		await That(preset.Tolerance).IsEqualTo(2);
		await That(first.Value.Tolerance).IsEqualTo(3);
	}

	[Test]
	public async Task WhenAQuantifierPresetIsShared_ShouldNotBeChangedByAnotherSlot()
	{
		Quantifier preset = Quantifier.Default.AtLeast(2);
		OptionsSlot<Quantifier> first = new(preset);
		OptionsSlot<Quantifier> second = new(preset);

		first.AtLeast(3);

		await That(second.Value.ToString()).IsEqualTo("at least twice");
		await That(preset.ToString()).IsEqualTo("at least twice");
		await That(first.Value.ToString()).IsEqualTo("at least 3 times");
	}

	[Test]
	public async Task WhenATimeTolerancePresetIsShared_ShouldNotBeChangedByAnotherSlot()
	{
		TimeTolerance preset = TimeTolerance.Days.Within(TimeSpan.FromDays(2));
		OptionsSlot<TimeTolerance> first = new(preset);
		OptionsSlot<TimeTolerance> second = new(preset);

		first.Within(TimeSpan.FromDays(3));

		await That(second.Value.Tolerance).IsEqualTo(TimeSpan.FromDays(2));
		await That(preset.Tolerance).IsEqualTo(TimeSpan.FromDays(2));
		await That(first.Value.ToString()).IsEqualTo(" ± 3 days");
	}

	[Test]
	public async Task WhenAnObjectMatchTypePresetIsShared_ShouldNotBeChangedByAnotherSlot()
	{
		ObjectMatchType<int> preset = ObjectMatchType<int>.WithTolerance<int>((a, e, t) => Math.Abs(a - e) <= t)
			.Within(2);
		OptionsSlot<ObjectMatchType<int>> first = new(preset);
		OptionsSlot<ObjectMatchType<int>> second = new(preset);

		first.Within(3);

		await That(second.Value).IsSameAs(preset);
		await That(preset.ToString()).IsEqualTo(" ± 2");
		await That(first.Value.ToString()).IsEqualTo(" ± 3");
	}
	private sealed class MyMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " at my index";

		public bool OnlySingleIndex() => false;
	}
}
