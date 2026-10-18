using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotGeneric
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<>);

				async Task Act()
				{
					await That(subject).IsNotGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<>
					             """);
			}

			[Test]
			public async Task WhenTypeIsGeneric_ShouldSucceedWithNegatedAssertion()
			{
				Type subject = typeof(PublicGenericClass<>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotGeneric());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic PublicSealedClass
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
				{
					await That(subject).IsNotGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was <null>
					             """);
			}
		}
	}
}
