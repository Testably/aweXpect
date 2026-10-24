using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotClasses
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsClassType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are classes,
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
			public async Task WhenEnumerableContainsNoClassTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Interfaces();

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyClasses_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Types()
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsClass && !type.IsRecordClass() in assembly containing type ThatTypes.AreNotClasses
					             none are classes,
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
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Interfaces();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotClasses());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotClasses
					             at least one is a class,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyClasses_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Types()
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotClasses());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
