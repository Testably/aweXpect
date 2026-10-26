using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class SignalerPredicateOptionsHolderExtensionsTests
{
	[Test]
	public async Task AddPredicate_ShouldCombineThePredicatesAndReturnTheHolder()
	{
		OptionsHolder<SignalerPredicateOptions<int>> sut = new(SignalerPredicateOptions<int>.Default);

		OptionsHolder<SignalerPredicateOptions<int>> result = sut
			.AddPredicate(x => x > 1, "x > 1")
			.AddPredicate(x => x < 3, "x < 3");

		await That(result).IsSameAs(sut);
		await That(sut.Value.ToString()).IsEqualTo(" with x > 1 and with x < 3");
		await That(sut.Value.Matches(2)).IsTrue();
		await That(sut.Value.Matches(3)).IsFalse()
			.Because("a signal has to match all predicates");
	}
}
