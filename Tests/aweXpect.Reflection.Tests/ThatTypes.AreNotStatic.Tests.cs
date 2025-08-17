using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonStaticTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Abstract.Types();

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreNotStatic
					             are all not static,
					             but it contained static types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonStaticTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Abstract.Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that abstract types in assembly containing type ThatTypes.AreNotStatic
					             also contain a static type,
					             but it only contained non-static types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Types()
					.WhichSatisfy(type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, });

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
