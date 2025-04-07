using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic PublicClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was <null>
					             """);
			}
		}
	}
}
