using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRecords
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonRecordTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types();

				async Task Act()
					=> await That(subject).AreRecords();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreRecords
					             are all records,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecords_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types()
					.WhichSatisfy(type => type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreRecords();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyRecords_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types()
					.WhichSatisfy(type => type.IsRecordClass());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreRecords());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordClass() in assembly containing type ThatTypes.AreRecords
					             are not all records,
					             but it only contained records [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNonRecordTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecords>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreRecords());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
