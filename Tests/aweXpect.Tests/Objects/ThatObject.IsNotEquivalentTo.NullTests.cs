namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsNotEquivalentTo
	{
		public sealed class NullTests
		{
			[Fact]
			public async Task WhenSubjectIsNotNull_ShouldSucceed()
			{
				OuterClass? subject = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBothAreNull_ShouldFail()
			{
				OuterClass? subject = null;

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to <null>,
					             but it was <null>

					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}
	}
}
