using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyOptionsTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task WhenMaxRecursionDepthIsNotPositive_ShouldThrowArgumentOutOfRangeException(int maxRecursionDepth)
	{
		void Act()
		{
			_ = new EquivalencyOptions
			{
				MaxRecursionDepth = maxRecursionDepth,
			};
		}

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The maximum recursion depth must be greater than zero*").AsWildcard()
			.Because("a limit below one would fail even the root comparison");
	}
}
