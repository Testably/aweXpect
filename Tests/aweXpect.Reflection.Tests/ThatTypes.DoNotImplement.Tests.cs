using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DoNotImplement
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenAllTypesImplementInterface_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
					             all do not implement ThatTypes.ITestInterface,
					             but it contained types that implement ThatTypes.ITestInterface

					             Not matching items:
					             [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableTypesDoNotImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(BaseClass),
				};

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), null,];

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotImplement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not implement ThatTypes.ITestInterface,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one implements ThatTypes.ITestInterface,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeImplementsInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(BaseClass));

				async Task Act()
				{
					await That(subject).DoNotImplement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotImplement(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			[Test]
			public async Task WhenNoTypeImplementsInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(BaseClass),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoNotImplement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeImplements_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotImplement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             at least one implements ThatTypes.ITestInterface,
					             but it only contained types that do not implement ThatTypes.ITestInterface

					             Collection:
					             [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}
}
