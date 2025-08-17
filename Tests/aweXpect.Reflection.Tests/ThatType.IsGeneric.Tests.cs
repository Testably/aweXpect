using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsGeneric());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<int>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsGeneric());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
