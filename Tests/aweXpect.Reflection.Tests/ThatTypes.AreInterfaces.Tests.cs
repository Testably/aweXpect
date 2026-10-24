using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreInterfaces
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonInterfaceType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreInterfaces();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are interfaces,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenEnumerableContainsOnlyInterfaceTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(IPublicInterface),
				};

				async Task Act()
				{
					await That(subject).AreInterfaces();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonInterfaceTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types();

				async Task Act()
				{
					await That(subject).AreInterfaces();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreInterfaces
					             all are interfaces,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenFilteringOnlyInterfaces_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types()
					.Which(type => type.IsInterface);

				async Task Act()
				{
					await That(subject).AreInterfaces();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonInterfaceTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInterfaces());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyInterfaces_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreInterfaces>().Types()
					.Which(type => type.IsInterface);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreInterfaces());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsInterface in assembly containing type ThatTypes.AreInterfaces
					             not all are interfaces,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
