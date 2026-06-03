using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreExceptions
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonExceptionTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types();

				async Task Act()
				{
					await That(subject).AreExceptions();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreExceptions
					             are all exceptions,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all exceptions,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
			public async Task WhenAssembliesContainNonExceptionTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExceptions());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyExceptions_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExceptions());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are exceptions in assembly containing type ThatTypes.AreExceptions
					             are not all exceptions,
					             but it only contained exceptions [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
