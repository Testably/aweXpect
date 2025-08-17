using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotRecordStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecordStructs>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotRecordStructs();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecordStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecordStructs>().Types()
					.WhichSatisfy(type => type.IsRecordStruct());

				async Task Act()
					=> await That(subject).AreNotRecordStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordStruct() in assembly containing type ThatTypes.AreNotRecordStructs
					             are all not record structs,
					             but it contained record structs [
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
				Filtered.Types subject = In.AssemblyContaining<AreNotRecordStructs>().Interfaces();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotRecordStructs());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotRecordStructs
					             also contain a record struct,
					             but it only contained not record structs [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecordStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotRecordStructs>().Types()
					.WhichSatisfy(type => type.IsRecordStruct());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotRecordStructs());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
