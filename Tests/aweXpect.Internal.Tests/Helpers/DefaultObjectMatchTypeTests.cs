using aweXpect.Core;

namespace aweXpect.Internal.Tests.Helpers;

public class DefaultObjectMatchTypeTests
{
	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(bool isEqual)
	{
		IEqualityEvaluator<object?> sut = ObjectMatchType<object?>.Default.ForEvaluation(default);
		EqualsObject a = new(isEqual);
		EqualsObject b = new(isEqual);

		await That(sut.AreConsideredEqual(a, b)).IsEqualTo(isEqual);
	}

	private sealed class EqualsObject(bool isEqual)
	{
		public override bool Equals(object? obj) => isEqual;

		public override int GetHashCode() => throw new NotSupportedException();
	}
}
