using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotInheritDirectlyFrom
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenNoTypeInheritsDirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypesInheritDirectly_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.GrandChildClass]
					             all do not inherit directly from ThatTypes.BaseClass,
					             but it contained types that inherit directly from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<Type?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit directly from ThatTypes.BaseClass,
					             but it was <null>
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenTypesInheritDirectly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit directly from ThatTypes.BaseClass,
					             but it contained types that inherit directly from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesInheritIndirectly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(GrandChildClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritDirectlyFrom(baseType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeInheritsDirectly_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritDirectlyFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.GrandChildClass]
					             at least one inherits directly from ThatTypes.BaseClass,
					             but it only contained types that do not inherit directly from ThatTypes.BaseClass

					             Collection:
					             [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeTypesInheritDirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritDirectlyFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
