using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNested
	{
		public sealed class TypeTests
		{
			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all nested,
					             but it contained non-nested types

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
			[Test]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types();

				async Task Act()
				{
					await That(subject).AreNested();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreNested
					             are all nested,
					             but it contained non-nested types

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
			[Test]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNested());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNested>().Types()
					.Which(type => type.IsNested);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNested());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsNested in assembly containing type ThatTypes.AreNested
					             are not all nested,
					             but it only contained nested types

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
