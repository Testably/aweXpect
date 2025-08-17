using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotNested
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => !type.IsNested);

				async Task Act()
					=> await That(subject).AreNotNested();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => type.IsNested);

				async Task Act()
					=> await That(subject).AreNotNested();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsNested in assembly containing type ThatTypes.AreNotNested
					             are all not nested,
					             but it contained nested types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => !type.IsNested);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotNested());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => !type.IsNested in assembly containing type ThatTypes.AreNotNested
					             also contain a nested type,
					             but it only contained non-nested types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.WhichSatisfy(type => type.IsNested);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotNested());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
