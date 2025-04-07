using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAbstract_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was abstract PublicAbstractClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonAbstractTypes))]
			public async Task WhenTypeIsNotAbstract_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonAbstractTypes()
				=>
				[
					typeof(PublicStaticClass),
					typeof(PublicSealedClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
