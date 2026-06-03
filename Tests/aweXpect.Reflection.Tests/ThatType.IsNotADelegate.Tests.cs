using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotADelegate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsADelegate_ShouldFail()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was delegate PublicDelegate
					             """);
			}

			[Theory]
			[MemberData(nameof(NonDelegateTypes))]
			public async Task WhenTypeIsNotADelegate_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonDelegateTypes() => new()
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
			public async Task WhenTypeIsADelegate_ShouldSucceed()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotADelegate());
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonDelegateTypes))]
			public async Task WhenTypeIsNotADelegate_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotADelegate());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*is a delegate*but it was*").AsWildcard();
			}

			public static TheoryData<Type> NonDelegateTypes() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
