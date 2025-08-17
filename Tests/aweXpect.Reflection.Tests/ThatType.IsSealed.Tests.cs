using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsSealed
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldFail(Type subject)
			{
				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is sealed,
					              but it was non-sealed {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsSealed_ShouldSucceed()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonSealedTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}

		public sealed class NegatedTests
		{
			[Theory]
			[MemberData(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsSealed());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsSealed_ShouldFail()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsSealed());

				await That(Act).Throws<XunitException>()
					.WithMessage("*is not sealed*but it was*").AsWildcard();
			}

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
