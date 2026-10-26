using aweXpect.Options;

namespace aweXpect.Core.Tests.Core;

public sealed class OptionsHolderTests
{
	[Test]
	public async Task SetMatch_ShouldSetTheMatchOnTheValue()
	{
		OptionsHolder<CollectionIndexOptions> sut = new(CollectionIndexOptions.Default);

		OptionsHolder<CollectionIndexOptions> result = sut.SetMatch(new MyMatch(), "AtMyIndex");

		await That(result).IsSameAs(sut);
		await That(sut.Value.Match).Is<MyMatch>();
	}

	[Test]
	public async Task ToString_ShouldReturnTheTextOfTheValue()
	{
		OptionsHolder<StringEqualityOptions> sut = new(StringEqualityOptions.Default);
		sut.IgnoringCase();

		string result = sut.ToString();

		await That(result).IsEqualTo(" ignoring case")
			.Because("an expectation text can append the holder like the options it holds");
	}

	[Test]
	public async Task ToString_WhenValueIsNull_ShouldReturnEmptyString()
	{
		OptionsHolder<string?> sut = new(null);

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task Value_ShouldBeReadableAsReadOnlyHolderOfABaseType()
	{
		StringEqualityOptions options = StringEqualityOptions.Default;
		OptionsHolder<StringEqualityOptions> sut = new(options);

		object? result = ReadAsBaseType(sut);

		await That(result).IsSameAs(options);
	}

	[Test]
	public async Task Value_WhenReplaced_ShouldReturnTheNewValue()
	{
		OptionsHolder<Quantifier> sut = new(Quantifier.Default);
		Quantifier replacement = Quantifier.Default.AtLeast(3);

		sut.Value = replacement;

		await That(sut.Value.ToString()).IsEqualTo("at least 3 times");
	}

	[Test]
	public async Task WhenACollectionMatchOptionsPresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		CollectionMatchOptions preset = new CollectionMatchOptions().IgnoringDuplicates();
		OptionsHolder<CollectionMatchOptions> first = new(preset);
		OptionsHolder<CollectionMatchOptions> second = new(preset);

		first.InAnyOrder();

		await That(second.Value.GetExpectation("[1]", ExpectationGrammars.None))
			.IsEqualTo("is equal to collection [1] in order ignoring duplicates");
		await That(first.Value.GetExpectation("[1]", ExpectationGrammars.None))
			.IsEqualTo("is equal to collection [1] in any order ignoring duplicates");
	}

	[Test]
	public async Task WhenANumberTolerancePresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		NumberTolerance<int> preset = new NumberTolerance<int>((a, b) => Math.Abs(a - b)).Within(2);
		OptionsHolder<NumberTolerance<int>> first = new(preset);
		OptionsHolder<NumberTolerance<int>> second = new(preset);

		first.Within(3);

		await That(second.Value.Tolerance).IsEqualTo(2);
		await That(preset.Tolerance).IsEqualTo(2);
		await That(first.Value.Tolerance).IsEqualTo(3);
	}

	[Test]
	public async Task WhenAQuantifierPresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		Quantifier preset = Quantifier.Default.AtLeast(2);
		OptionsHolder<Quantifier> first = new(preset);
		OptionsHolder<Quantifier> second = new(preset);

		first.AtLeast(3);

		await That(second.Value.ToString()).IsEqualTo("at least twice");
		await That(preset.ToString()).IsEqualTo("at least twice");
		await That(first.Value.ToString()).IsEqualTo("at least 3 times");
	}

	[Test]
	public async Task WhenAStringOptionsPresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		StringEqualityOptions preset = StringEqualityOptions.Default.IgnoringCase();
		OptionsHolder<StringEqualityOptions> first = new(preset);
		OptionsHolder<StringEqualityOptions> second = new(preset);

		first.AsPrefix();

		await That(second.Value.ToString()).IsEqualTo(" ignoring case");
		await That(preset.ToString()).IsEqualTo(" ignoring case");
		await That(first.Value.ToString()).IsEqualTo(" as prefix ignoring case");
	}

	[Test]
	public async Task WhenATimeTolerancePresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		TimeTolerance preset = TimeTolerance.Days.Within(TimeSpan.FromDays(2));
		OptionsHolder<TimeTolerance> first = new(preset);
		OptionsHolder<TimeTolerance> second = new(preset);

		first.Within(TimeSpan.FromDays(3));

		await That(second.Value.Tolerance).IsEqualTo(TimeSpan.FromDays(2));
		await That(preset.Tolerance).IsEqualTo(TimeSpan.FromDays(2));
		await That(first.Value.ToString()).IsEqualTo(" ± 3 days");
	}

	[Test]
	public async Task WhenAnObjectMatchTypePresetIsShared_ShouldNotBeChangedByAnotherHolder()
	{
		ObjectMatchType<int> preset = ObjectMatchType<int>.WithTolerance<int>((a, e, t) => Math.Abs(a - e) <= t)
			.Within(2);
		OptionsHolder<ObjectMatchType<int>> first = new(preset);
		OptionsHolder<ObjectMatchType<int>> second = new(preset);

		first.Within(3);

		await That(second.Value).IsSameAs(preset);
		await That(preset.ToString()).IsEqualTo(" ± 2");
		await That(first.Value.ToString()).IsEqualTo(" ± 3");
	}
	private static object? ReadAsBaseType(IReadOnlyOptionsHolder<object> holder) => holder.Value;

	private sealed class MyMatch : CollectionIndexOptions.IMatch
	{
		public string GetDescription() => " at my index";

		public bool OnlySingleIndex() => false;
	}
}
