using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class Implement
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableTypesImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithInterface1), null,];

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Implement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all implement ThatTypes.ITestInterface,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.ClassWithInterface1,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all implement ThatTypes.ITestInterface,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.ClassWithInterface1,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenSomeTypeDoesNotImplement_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).Implement<ITestInterface>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.UnrelatedClass]
					             all implement ThatTypes.ITestInterface,
					             but it contained types that do not implement ThatTypes.ITestInterface

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAllTypesImplementInterface_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).Implement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1),
				};
				Type classType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).Implement(classType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesImplement_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Implement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.ClassWithInterface1, ThatTypes.ClassWithInterface2]
					             not all implement ThatTypes.ITestInterface,
					             but it only contained types that implement ThatTypes.ITestInterface

					             Collection:
					             [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.ClassWithInterface2
					             ]
					             """);
			}
		}
	}
}
