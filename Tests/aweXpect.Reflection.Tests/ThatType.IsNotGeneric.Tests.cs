using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<>);

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsGeneric_ShouldSucceedWithNegatedAssertion()
			{
				Type subject = typeof(PublicGenericClass<>);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotGeneric());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotGeneric());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic PublicSealedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was <null>
					             """);
			}
		}
	}
}
