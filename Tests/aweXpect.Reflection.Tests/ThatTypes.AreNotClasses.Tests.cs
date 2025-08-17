using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotClasses
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Interfaces();

				async Task Act()
					=> await That(subject).AreNotClasses();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Types()
					.WhichSatisfy(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreNotClasses();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types matching type => type.IsClass && !type.IsRecordClass() in assembly containing type ThatTypes.AreNotClasses
					             are all not classes,
					             but it contained classes [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Interfaces();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotClasses());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that interfaces in assembly containing type ThatTypes.AreNotClasses
					             also contain a class,
					             but it only contained not classes [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Types()
					.WhichSatisfy(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotClasses());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
