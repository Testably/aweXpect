using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotClasses
	{
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenEnumerableContainsClassType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all not classes,
					             but it contained classes [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoClassTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenAssembliesContainOnlyInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Interfaces();

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyClasses_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotClasses>().Types()
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).AreNotClasses();
				}

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
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotClasses());
				}

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
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotClasses());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
