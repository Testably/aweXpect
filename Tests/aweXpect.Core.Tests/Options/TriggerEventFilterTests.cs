using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class TriggerEventFilterTests
{
	[Test]
	public async Task AddPredicate_ShouldNotChangeTheOriginalFilter()
	{
		TriggerEventFilter sut = TriggerEventFilter.Default;

		TriggerEventFilter result = sut.AddPredicate(p => p.Length > 0, " with a parameter");

		await That(result.IsMatch([])).IsFalse();
		await That(sut.IsMatch([])).IsTrue();
		await That(sut.ToString()).IsEmpty();
	}

	[Test]
	public async Task AddPredicate_Twice_ShouldRequireBothPredicatesAndNameBoth()
	{
		TriggerEventFilter sut = TriggerEventFilter.Default;

		sut = sut.AddPredicate(p => p.Length > 0, " with a parameter");
		sut = sut.AddPredicate(p => p[0] is int, " with an int");

		await That(sut.ToString()).IsEqualTo(" with a parameter and with an int");
		await That(sut.IsMatch([1,])).IsTrue();
		await That(sut.IsMatch(["foo",])).IsFalse();
	}

	[Test]
	public async Task AddPredicate_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		TriggerEventFilter sut = TriggerEventFilter.Default;

		void Act() => sut.AddPredicate(null!, " with nothing");

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
		await That(sut.ToString()).IsEmpty();
	}

	[Test]
	public async Task IsMatch_WithoutPredicate_ShouldMatchAnyParameters()
	{
		TriggerEventFilter sut = TriggerEventFilter.Default;

		bool result = sut.IsMatch([]);

		await That(result).IsTrue();
		await That(sut.ToString()).IsEmpty();
	}
}
