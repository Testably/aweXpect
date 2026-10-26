using System.Collections.Generic;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Internal.Tests.Options;

public sealed class GenericArgumentsFilterOptionsTests
{
	[Test]
	public async Task Matches_WhenAPredicateThrows_ShouldThrowTheExceptionOfTheCaller()
	{
		GenericArgumentsFilterOptions sut = new();
		sut.AddPredicate(_ => throw new InvalidOperationException("boom"), () => "matching predicate");

		async Task Act()
			=> await sut.Matches(typeof(List<int>));

		await That(Act).Throws<Exception>()
			.WithInner<InvalidOperationException>(inner => inner.HasMessage("boom"))
			.And.WithMessage("The code of the caller threw an exception while the expectation was evaluated.")
			.Because("the expectation reports an exception of the predicate as a failure of the caller's code");
	}
}
