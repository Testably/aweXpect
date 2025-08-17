using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNotSealed_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotSealed());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed PublicAbstractClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsSealed_ShouldFail()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was sealed PublicSealedClass
					             """);
			}

			[Theory]
			[MemberData(nameof(SealedTypesForNegated))]
			public async Task WhenTypeIsSealed_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotSealed());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> SealedTypesForNegated() => new()
			{
				typeof(PublicSealedClass),
			};

			public static TheoryData<Type> NonSealedTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
