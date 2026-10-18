using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotInstantiable
	{
		public sealed class Tests
		{
			[Test]
			[MethodDataSource(nameof(InstantiableTypes))]
			public async Task WhenTypeIsInstantiable_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is not instantiable,
					              but it was instantiable {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[MethodDataSource(nameof(InstantiableTypes))]
			public async Task WhenTypeIsInstantiable_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInstantiable());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonInstantiableTypesWithReason))]
			public async Task WhenTypeIsNotInstantiable_ShouldFailWithNegatedAssertion(Type subject, string reason)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInstantiable());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is instantiable,
					              but it was {reason} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[MethodDataSource(nameof(NonInstantiableTypes))]
			public async Task WhenTypeIsNotInstantiable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not instantiable,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> InstantiableTypes()
				=> new List<Type>
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(Container.PublicNestedClass),
					typeof(ClassWithoutDefaultConstructor),
				};

			public static IEnumerable<Type> NonInstantiableTypes()
				=> new List<Type>
				{
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicGenericClass<>),
				};

			public static IEnumerable<(Type, string)> NonInstantiableTypesWithReason()
				=> new List<(Type, string)>
				{
					(typeof(PublicAbstractClass), "abstract"),
					(typeof(PublicStaticClass), "static"),
					(typeof(IPublicInterface), "an interface"),
					(typeof(PublicGenericClass<>), "a generic type definition"),
				};
		}
	}
}
