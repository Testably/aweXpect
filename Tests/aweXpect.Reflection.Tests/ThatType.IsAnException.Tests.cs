using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnException
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnException_ShouldSucceed()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonExceptionType))]
			public async Task WhenTypeIsNotAnException_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is an exception,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnException();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an exception,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonExceptionType() => new List<(Type?, string)>
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
			public async Task WhenTypeIsAnException_ShouldFail()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnException());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was exception PublicException
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonExceptionTypeForNegated))]
			public async Task WhenTypeIsNotAnException_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnException());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonExceptionTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
