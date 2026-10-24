using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotInterfaces
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsInterfaceType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreNotInterfaces();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are interfaces,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsNoInterfaceTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotInterfaces();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainOnlyEnumTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Enums();

				async Task Act()
				{
					await That(subject).AreNotInterfaces();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyInterfaces_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Types()
					.Which(type => type.IsInterface);

				async Task Act()
				{
					await That(subject).AreNotInterfaces();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsInterface in assembly containing type ThatTypes.AreNotInterfaces
					             none are interfaces,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainOnlyEnumTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Enums();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInterfaces());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that enums in assembly containing type ThatTypes.AreNotInterfaces
					             at least one is an interface,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyInterfaces_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotInterfaces>().Types()
					.Which(type => type.IsInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotInterfaces());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
