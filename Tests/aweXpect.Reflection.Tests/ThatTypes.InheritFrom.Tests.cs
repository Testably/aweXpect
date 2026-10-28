using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class InheritFrom
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).InheritFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(ClassWithInterface1) || type == typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).InheritFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), null,];

				async Task Act()
				{
					await That(subject).InheritFrom<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all inherit from ThatTypes.BaseClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all inherit from ThatTypes.BaseClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenSomeTypesDoNotInherit_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).InheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass) in assembly containing type ThatTypes.InheritFrom.GenericTests
					             all inherit from ThatTypes.BaseClass,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesInheritIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<GenericTests>()
					.Types()
					.Which(type => type == typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).InheritFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritFrom(baseType);
				}

				await That(Act).DoesNotThrow();
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
					await That(subject).InheritFrom(interfaceType);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenSomeTypesDoNotInherit_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				};
				Type baseType = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).InheritFrom(baseType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all inherit from ThatTypes.BaseClass,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesInheritFromBaseClass_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(DerivedClass1) || type == typeof(DerivedClass2) in assembly containing type ThatTypes.InheritFrom.NegatedTests
					             not all inherit from ThatTypes.BaseClass,
					             but all 2 did

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(ClassWithInterface1) || type == typeof(ClassWithInterface2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.InheritFrom<ITestInterface>());
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenSomeTypesDoNotInherit_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(DerivedClass1) || type == typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesInheritIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<NegatedTests>()
					.Types()
					.Which(type => type == typeof(GrandChildClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(GrandChildClass) in assembly containing type ThatTypes.InheritFrom.NegatedTests
					             not all inherit from ThatTypes.BaseClass,
					             but all 1 did

					             Collection:
					             [
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}
		}
	}
}
