using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreEnums
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonEnumTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types();

				async Task Act()
					=> await That(subject).AreEnums();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreEnums
					             are all enums,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).AreEnums();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreEnums());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsEnum in assembly containing type ThatTypes.AreEnums
					             are not all enums,
					             but it only contained enums [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNonEnumTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreEnums>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreEnums());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
