using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsStatic
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(NonStaticTypes))]
			public async Task WhenTypeIsNotStatic_ShouldFail(Type subject)
			{
				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is static,
					              but it was non-static {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is static,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsStatic_ShouldSucceed()
			{
				Type subject = typeof(PublicStaticClass);

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonStaticTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicSealedClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
