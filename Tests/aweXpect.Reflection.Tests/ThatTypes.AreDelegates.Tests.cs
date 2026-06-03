using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreDelegates
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonDelegateTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types();

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreDelegates
					             are all delegates,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyDelegates_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsNonDelegateType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicDelegate), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all delegates,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsOnlyDelegateTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicDelegate),
				};

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonDelegateTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreDelegates());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyDelegates_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreDelegates());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types which are delegates in assembly containing type ThatTypes.AreDelegates
					             are not all delegates,
					             but it only contained delegates [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
