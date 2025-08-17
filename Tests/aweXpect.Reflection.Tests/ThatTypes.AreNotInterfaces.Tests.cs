using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotInterfaces
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyEnumTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Enums();

				async Task Act()
					=> await That(subject).AreNotInterfaces();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyInterfaces_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Types()
					.WhichSatisfy(type => type.IsInterface);

				async Task Act()
					=> await That(subject).AreNotInterfaces();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsInterface in assembly containing type ThatTypes.AreNotInterfaces
					             are all not interfaces,
					             but it contained interfaces [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyEnumTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Enums();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInterfaces());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that enums in assembly containing type ThatTypes.AreNotInterfaces
					             also contain an interface,
					             but it only contained not interfaces [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyInterfaces_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Types()
					.WhichSatisfy(type => type.IsInterface);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotInterfaces());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
