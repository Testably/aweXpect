using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreClasses
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainNonClassTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types();

				async Task Act()
					=> await That(subject).AreClasses();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreClasses
					             are all classes,
					             but it contained other types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types()
					.WhichSatisfy(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
					=> await That(subject).AreClasses();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesContainNonClassTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreClasses());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types()
					.WhichSatisfy(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreClasses());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types matching type => type.IsClass && !type.IsRecordClass() in assembly containing type ThatTypes.AreClasses
					             are not all classes,
					             but it only contained classes [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
