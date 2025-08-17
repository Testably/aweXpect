using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types();

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreSealed
					             are all sealed,
					             but it contained non-sealed types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types()
					.WhichSatisfy(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlySealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types()
					.WhichSatisfy(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreSealed
					             are not all sealed,
					             but it only contained sealed types [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
