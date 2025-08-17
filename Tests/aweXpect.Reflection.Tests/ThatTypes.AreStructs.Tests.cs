using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreStructs
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonStructTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types();

				async Task Act()
					=> await That(subject).AreStructs();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreStructs
					             are all structs,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).AreStructs();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyStructs_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types()
					.WhichSatisfy(type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStructs());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsValueType && !type.IsRecordStruct() && !type.IsEnum in assembly containing type ThatTypes.AreStructs
					             are not all structs,
					             but it only contained structs [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNonStructTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreStructs>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStructs());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
