using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class DoNotInheritFrom
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenAllTypesInherit_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.DerivedClass2]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllTypesInheritIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.GrandChildClass]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenAtLeastOneTypeInherits_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.UnrelatedClass]
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), null,];

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
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
					             at least one inherits from ThatTypes.BaseClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(GenericTests));

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAllTypesInherit_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllTypesInheritIndirectly_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(GrandChildClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenAtLeastOneTypeInherits_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not inherit from ThatTypes.BaseClass,
					             but it contained types that inherit from ThatTypes.BaseClass

					             Not matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface1), typeof(ClassWithInterface2),
				};
				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoNotInheritFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenNoTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass), typeof(GenericTests));
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoNotInheritFrom(baseType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesDoNotInherit_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(NegatedTests), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DoNotInheritFrom.NegatedTests, ThatTypes.UnrelatedClass]
					             at least one inherits from ThatTypes.BaseClass,
					             but it only contained types that do not inherit from ThatTypes.BaseClass

					             Collection:
					             [
					               ThatTypes.DoNotInheritFrom.NegatedTests,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllTypesInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllTypesInheritIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Filtered.Types subject = In.Types(typeof(ClassWithInterface1), typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotInheritFrom<ITestInterface>());
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}
		}
	}
}
