using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnException
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnException_ShouldFail()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was exception PublicException
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonExceptionTypes))]
			public async Task WhenTypeIsNotAnException_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnException();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an exception,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonExceptionTypes() => new List<Type>
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
			public async Task WhenTypeIsAnException_ShouldSucceed()
			{
				Type subject = typeof(PublicException);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnException());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonExceptionTypes))]
			public async Task WhenTypeIsNotAnException_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnException());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is an exception*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonExceptionTypes() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
