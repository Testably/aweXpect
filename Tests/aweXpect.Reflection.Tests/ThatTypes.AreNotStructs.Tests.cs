using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotStructs
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoStructTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotStructs();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsStructType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface), typeof(PublicStruct),
				};

				async Task Act()
				{
					await That(subject).AreNotStructs();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not structs,
					             but it contained structs

					             Not matching items:
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
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Interfaces();

				async Task Act()
				{
					await That(subject).AreNotStructs();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Types()
					.Which(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
				{
					await That(subject).AreNotStructs();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum in assembly containing type ThatTypes.AreNotStructs
					             are all not structs,
					             but it contained structs

					             Not matching items:
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
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Interfaces();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStructs());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotStructs
					             also contain a struct,
					             but it only contained not structs

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Types()
					.Which(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStructs());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
