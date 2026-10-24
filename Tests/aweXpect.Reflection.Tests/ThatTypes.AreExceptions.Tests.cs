using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreExceptions
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonExceptionTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types();

				async Task Act()
				{
					await That(subject).AreExceptions();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreExceptions
					             all are exceptions,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenFilteringOnlyExceptions_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).AreExceptions();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonExceptionType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicException), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreExceptions();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are exceptions,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenEnumerableContainsOnlyExceptionTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicException),
				};

				async Task Act()
				{
					await That(subject).AreExceptions();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonExceptionTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExceptions());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyExceptions_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExceptions());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are exceptions in assembly containing type ThatTypes.AreExceptions
					             not all are exceptions,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
