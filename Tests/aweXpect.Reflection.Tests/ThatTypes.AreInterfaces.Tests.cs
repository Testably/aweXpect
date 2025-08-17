using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreInterfaces
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonInterfaceTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types();

				async Task Act()
					=> await That(subject).AreInterfaces();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreInterfaces
					             are all interfaces,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyInterfaces_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types()
					.WhichSatisfy(type => type.IsInterface);

				async Task Act()
					=> await That(subject).AreInterfaces();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyInterfaces_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types()
					.WhichSatisfy(type => type.IsInterface);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInterfaces());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsInterface in assembly containing type ThatTypes.AreInterfaces
					             are not all interfaces,
					             but it only contained interfaces [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAssembliesContainNonInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreInterfaces());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
