using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotStructs();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).AreNotStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum in assembly containing type ThatTypes.AreNotStructs
					             are all not structs,
					             but it contained structs [
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
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Interfaces();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStructs());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotStructs
					             also contain a struct,
					             but it only contained not structs [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStructs());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
