using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnException
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnException_ShouldSucceed()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonExceptionType))]
			public async Task WhenTypeIsNotAnException_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an exception,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is an exception,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonExceptionType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicClass), "class PublicClass"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicEnum), "enum PublicEnum"
				},
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAnException_ShouldFail()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnException());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was exception PublicException
					             """);
			}

			[Theory]
			[MemberData(nameof(NonExceptionTypeForNegated))]
			public async Task WhenTypeIsNotAnException_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnException());
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonExceptionTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
