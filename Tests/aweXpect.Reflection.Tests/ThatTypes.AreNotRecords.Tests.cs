using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotRecords
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoRecordTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotRecords();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsRecordType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface), typeof(PublicRecord),
				};

				async Task Act()
				{
					await That(subject).AreNotRecords();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are records,
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

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Interfaces();

				async Task Act()
				{
					await That(subject).AreNotRecords();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyRecords_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Types()
					.Which(type => type.IsRecordClass());

				async Task Act()
				{
					await That(subject).AreNotRecords();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordClass() in assembly containing type ThatTypes.AreNotRecords
					             none are records,
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
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Interfaces();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRecords());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotRecords
					             at least one is a record,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyRecords_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Types()
					.Which(type => type.IsRecordClass());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRecords());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
