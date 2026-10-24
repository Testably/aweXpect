using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotExceptions
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsExceptionType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicException),
				};

				async Task Act()
				{
					await That(subject).AreNotExceptions();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are exceptions,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsNoExceptionTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotExceptions();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNoExceptionTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreNotExceptions();

				async Task Act()
				{
					await That(subject).AreNotExceptions();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyExceptions_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).AreNotExceptions();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which are exceptions in assembly containing type ThatTypes.AreNotExceptions
					             none are exceptions,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNoExceptionTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreNotExceptions();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExceptions());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are not exceptions in assembly containing type ThatTypes.AreNotExceptions
					             at least one is an exception,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyExceptions_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExceptions());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
