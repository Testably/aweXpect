using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNested
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsNonNestedType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Container.PublicNestedClass), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNested();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all nested,
					             but it contained non-nested types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsOnlyNestedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Container.PublicNestedClass),
				};

				async Task Act()
				{
					await That(subject).AreNested();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types();

				async Task Act()
				{
					await That(subject).AreNested();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreNested
					             are all nested,
					             but it contained non-nested types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types()
					.Which(type => type.IsNested);

				async Task Act()
				{
					await That(subject).AreNested();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNested());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types()
					.Which(type => type.IsNested);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNested());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsNested in assembly containing type ThatTypes.AreNested
					             are not all nested,
					             but it only contained nested types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
