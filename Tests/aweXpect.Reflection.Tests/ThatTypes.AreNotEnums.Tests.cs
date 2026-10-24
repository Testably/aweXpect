using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotEnums
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsEnumType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicEnum),
				};

				async Task Act()
				{
					await That(subject).AreNotEnums();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are enums,
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
			public async Task WhenEnumerableContainsNoEnumTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotEnums();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainOnlyClassTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Classes();

				async Task Act()
				{
					await That(subject).AreNotEnums();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyEnums_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Types()
					.Which(type => type.IsEnum);

				async Task Act()
				{
					await That(subject).AreNotEnums();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsEnum in assembly containing type ThatTypes.AreNotEnums
					             none are enums,
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
			public async Task WhenAssembliesContainOnlyClassTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Classes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotEnums());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that classes in assembly containing type ThatTypes.AreNotEnums
					             at least one is an enum,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyEnums_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Types()
					.Which(type => type.IsEnum);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotEnums());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
