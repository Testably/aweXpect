using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotInstantiable
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(InstantiableTypes))]
			public async Task WhenTypeIsInstantiable_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not instantiable,
					              but it was instantiable {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[MemberData(nameof(InstantiableTypes))]
			public async Task WhenTypeIsInstantiable_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInstantiable());
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonInstantiableTypesWithReason))]
			public async Task WhenTypeIsNotInstantiable_ShouldFailWithNegatedAssertion(Type subject, string reason)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotInstantiable());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is instantiable,
					              but it was {reason} {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[MemberData(nameof(NonInstantiableTypes))]
			public async Task WhenTypeIsNotInstantiable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotInstantiable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not instantiable,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> InstantiableTypes()
				=> new()
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(Container.PublicNestedClass),
					typeof(ClassWithoutDefaultConstructor),
				};

			public static TheoryData<Type> NonInstantiableTypes()
				=> new()
				{
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicGenericClass<>),
				};

			public static TheoryData<Type, string> NonInstantiableTypesWithReason()
				=> new()
				{
					{
						typeof(PublicAbstractClass), "abstract"
					},
					{
						typeof(PublicStaticClass), "static"
					},
					{
						typeof(IPublicInterface), "an interface"
					},
					{
						typeof(PublicGenericClass<>), "a generic type definition"
					},
				};
		}
	}
}
