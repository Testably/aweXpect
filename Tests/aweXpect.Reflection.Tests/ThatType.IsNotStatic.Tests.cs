using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotStatic
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(NonStaticTypes))]
			public async Task WhenTypeIsNotStatic_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not static,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsStatic_ShouldFail()
			{
				Type subject = typeof(PublicStaticClass);

				async Task Act()
					=> await That(subject).IsNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not static,
					             but it was static PublicStaticClass
					             """);
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

		public sealed class NegatedTests
		{
			[Theory]
			[MemberData(nameof(NonStaticTypes))]
			public async Task WhenTypeIsNotStatic_ShouldFail(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotStatic());

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is static,
					              but it was non-static {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsStatic_ShouldSucceed()
			{
				Type subject = typeof(PublicStaticClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotStatic());

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
