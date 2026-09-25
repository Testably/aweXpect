using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class ContainingMatchTypeTests
	{
		[Fact]
		public async Task Contains_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.Containing();

			await That(result).IsSameAs(sut);
		}
	}
}
