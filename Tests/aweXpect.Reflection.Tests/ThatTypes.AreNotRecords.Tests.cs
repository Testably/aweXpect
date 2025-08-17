using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotRecords
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotRecords();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecords_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Types()
					.WhichSatisfy(type => type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreNotRecords();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordClass() in assembly containing type ThatTypes.AreNotRecords
					             are all not records,
					             but it contained records [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Interfaces();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotRecords());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotRecords
					             also contain a record,
					             but it only contained not records [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecords_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecords>().Types()
					.WhichSatisfy(type => type.IsRecordClass());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotRecords());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
