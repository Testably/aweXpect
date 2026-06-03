using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsADelegate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsADelegate_ShouldSucceed()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonDelegateType))]
			public async Task WhenTypeIsNotADelegate_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a delegate,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a delegate,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonDelegateType() => new()
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
			public async Task WhenTypeIsADelegate_ShouldFail()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsADelegate());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was delegate PublicDelegate
					             """);
			}

			[Theory]
			[MemberData(nameof(NonDelegateTypeForNegated))]
			public async Task WhenTypeIsNotADelegate_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsADelegate());
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonDelegateTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
