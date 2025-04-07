using Container = aweXpect.Reflection.Tests.TestHelpers.Types.Container;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNested_ShouldFail()
			{
				Type subject = typeof(Container.PublicNestedClass);

				async Task Act()
					=> await That(subject).IsNotNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not nested,
					             but it was nested PublicNestedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotNested_ShouldSucceed()
			{
				Type subject = typeof(Container);

				async Task Act()
					=> await That(subject).IsNotNested();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not nested,
					             but it was <null>
					             """);
			}
		}
	}
}
