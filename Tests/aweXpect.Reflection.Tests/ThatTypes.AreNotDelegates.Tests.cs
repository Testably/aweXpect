using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotDelegates
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsDelegateType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicDelegate),
				};

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all not delegates,
					             but it contained delegates [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoDelegateTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNoDelegateTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreNotDelegates();

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyDelegates_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which are delegates in assembly containing type ThatTypes.AreNotDelegates
					             are all not delegates,
					             but it contained delegates [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNoDelegateTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreNotDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotDelegates());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are not delegates in assembly containing type ThatTypes.AreNotDelegates
					             also contain a delegate,
					             but it only contained not delegates [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyDelegates_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotDelegates());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
