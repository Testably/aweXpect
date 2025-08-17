using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => !type.IsGenericType);

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => type.IsGenericType);

				async Task Act()
					=> await That(subject).AreNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsGenericType in assembly containing type ThatTypes.AreNotGeneric
					             are all not generic,
					             but it contained generic types [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => !type.IsGenericType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => !type.IsGenericType in assembly containing type ThatTypes.AreNotGeneric
					             also contain a generic type,
					             but it only contained non-generic types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.WhichSatisfy(type => type.IsGenericType);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
