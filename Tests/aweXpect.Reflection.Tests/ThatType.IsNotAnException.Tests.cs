using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnException
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnException_ShouldFail()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was exception PublicException
					             """);
			}

			[Theory]
			[MemberData(nameof(NonExceptionTypes))]
			public async Task WhenTypeIsNotAnException_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonExceptionTypes() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAnException_ShouldSucceed()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnException());
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonExceptionTypes))]
			public async Task WhenTypeIsNotAnException_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnException());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*is an exception*but it was*").AsWildcard();
			}

			public static TheoryData<Type> NonExceptionTypes() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
