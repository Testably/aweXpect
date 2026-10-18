using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsADelegate
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsADelegate_ShouldSucceed()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonDelegateType))]
			public async Task WhenTypeIsNotADelegate_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a delegate,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsADelegate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a delegate,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonDelegateType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsADelegate_ShouldFail()
			{
				Type subject = typeof(PublicDelegate);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsADelegate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a delegate,
					             but it was delegate PublicDelegate
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonDelegateTypeForNegated))]
			public async Task WhenTypeIsNotADelegate_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsADelegate());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonDelegateTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
