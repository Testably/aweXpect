using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnInterface
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnInterface_ShouldFail()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
				{
					await That(subject).IsNotAnInterface();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was interface IPublicInterface
					             """);
			}

			[Test]
			[MethodDataSource(nameof(InterfaceTypesForNegated))]
			public async Task WhenTypeIsAnInterface_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnInterface());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotAnInterface_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnInterface());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an interface,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonInterfaceTypes))]
			public async Task WhenTypeIsNotAnInterface_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnInterface();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnInterface();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> InterfaceTypesForNegated() => new List<Type>
			{
				typeof(IPublicInterface),
			};

			public static IEnumerable<Type> NonInterfaceTypes()
				=>
				[
					typeof(PublicClass),
					typeof(PublicEnum),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
