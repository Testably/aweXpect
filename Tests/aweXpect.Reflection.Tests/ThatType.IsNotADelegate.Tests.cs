using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotADelegate
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsADelegate_ShouldFail()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was delegate PublicDelegate
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonDelegateTypes))]
			public async Task WhenTypeIsNotADelegate_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotADelegate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonDelegateTypes() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsADelegate_ShouldSucceed()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotADelegate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonDelegateTypes))]
			public async Task WhenTypeIsNotADelegate_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotADelegate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is a delegate*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonDelegateTypes() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
