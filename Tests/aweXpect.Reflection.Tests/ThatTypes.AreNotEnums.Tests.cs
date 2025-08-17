using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotEnums
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyClassTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Classes();

				async Task Act()
					=> await That(subject).AreNotEnums();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).AreNotEnums();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsEnum in assembly containing type ThatTypes.AreNotEnums
					             are all not enums,
					             but it contained enums [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyClassTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Classes();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotEnums());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that classes in assembly containing type ThatTypes.AreNotEnums
					             also contain an enum,
					             but it only contained not enums [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyEnums_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotEnums>().Types()
					.WhichSatisfy(type => type.IsEnum);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotEnums());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
