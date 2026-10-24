using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreEnums
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonEnumTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types();

				async Task Act()
				{
					await That(subject).AreEnums();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreEnums
					             all are enums,
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
			public async Task WhenFilteringOnlyEnums_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types()
					.Which(type => type.IsEnum);

				async Task Act()
				{
					await That(subject).AreEnums();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonEnumType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicEnum), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreEnums();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are enums,
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
			public async Task WhenEnumerableContainsOnlyEnumTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicEnum),
				};

				async Task Act()
				{
					await That(subject).AreEnums();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonEnumTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreEnums());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyEnums_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types()
					.Which(type => type.IsEnum);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreEnums());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsEnum in assembly containing type ThatTypes.AreEnums
					             not all are enums,
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
