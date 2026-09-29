using System.Threading;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyExpectationBuilderTests
{
	[Fact]
	public async Task IsMet_ShouldThrowNotSupportedException()
	{
		EquivalencyExpectationBuilder<int> sut = new();

		async Task Act() => await sut.IsMet(
			new ExpectationNode(), null!, new TimeSystemMock(), null, CancellationToken.None);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Use IsMetBy for EquivalencyExpectationBuilder.");
	}
}
