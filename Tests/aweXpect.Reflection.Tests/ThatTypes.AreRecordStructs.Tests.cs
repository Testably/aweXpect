using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreRecordStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonRecordStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types();

				async Task Act()
					=> await That(subject).AreRecordStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreRecordStructs
					             are all record structs,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyRecordStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types()
					.WhichSatisfy(type => type.IsRecordStruct());

				async Task Act()
					=> await That(subject).AreRecordStructs();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyRecordStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types()
					.WhichSatisfy(type => type.IsRecordStruct());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreRecordStructs());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsRecordStruct() in assembly containing type ThatTypes.AreRecordStructs
					             are not all record structs,
					             but it only contained record structs [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNonRecordStructTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreRecordStructs>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreRecordStructs());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
