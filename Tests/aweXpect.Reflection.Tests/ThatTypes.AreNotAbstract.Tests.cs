using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, } in assembly containing type ThatTypes.AreNotAbstract
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that sealed types in assembly containing type ThatTypes.AreNotAbstract
					             also contain an abstract type,
					             but it only contained non-abstract types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
