using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotExceptions
	{
		public sealed class TypeTests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all not exceptions,
					             but it contained exceptions [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenFilteringOnlyExceptions_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreExceptions();

				async Task Act()
				{
					await That(subject).AreNotExceptions();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which are exceptions in assembly containing type ThatTypes.AreNotExceptions
					             are all not exceptions,
					             but it contained exceptions [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNoExceptionTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotExceptions>().Types()
					.WhichAreNotExceptions();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExceptions());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are not exceptions in assembly containing type ThatTypes.AreNotExceptions
					             also contain an exception,
					             but it only contained not exceptions [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
