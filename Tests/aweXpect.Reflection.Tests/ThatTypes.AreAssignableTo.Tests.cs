using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreAssignableTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreAssignable_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableTypesAreAssignable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), null,];

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableTo<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are assignable to ThatTypes.BaseClass,
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
					             not all are assignable to ThatTypes.BaseClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenSomeTypeIsNotAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.UnrelatedClass]
					             all are assignable to ThatTypes.BaseClass,
					             but only 1 of 2 were

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
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1),
				};
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).AreAssignableTo(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			[Test]
			public async Task WhenTypeIsAnOpenGenericAndThereAreNoTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = Array.Empty<Type?>();
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).AreAssignableTo(openGeneric);
				}

				await That(Act).DoesNotThrow()
					.Because("the generated expectation rejects an open generic type only when it compares an item with it");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesAreAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableTo<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.DerivedClass2]
					             not all are assignable to ThatTypes.BaseClass,
					             but all 2 were

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}
		}
	}

	public sealed class AreNotAssignableTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableTypesAreNotAssignable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), null,];

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableTo<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are assignable to ThatTypes.BaseClass,
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
					             at least one is assignable to ThatTypes.BaseClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeIsAssignable_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeIsAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1]
					             none are assignable to ThatTypes.BaseClass,
					             but 1 of 1 were

					             Matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeIsAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableTo<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             at least one is assignable to ThatTypes.BaseClass,
					             but none of 1 were

					             Collection:
					             [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}

	public sealed partial class AreAssignableFrom
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreAssignableFromTarget_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).AreAssignableFrom<GrandChildClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableTypesAreAssignableFromTarget_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(BaseClass), typeof(DerivedClass1),
				};

				async Task Act()
				{
					await That(subject).AreAssignableFrom<GrandChildClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(BaseClass), null,];

				async Task Act()
				{
					await That(subject).AreAssignableFrom<GrandChildClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableFrom<GrandChildClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are assignable from ThatTypes.GrandChildClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are assignable from ThatTypes.GrandChildClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenSomeTypeIsNotAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass, ThatTypes.UnrelatedClass]
					             all are assignable from ThatTypes.DerivedClass1,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllTypesAreAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableFrom<GrandChildClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass, ThatTypes.DerivedClass1]
					             not all are assignable from ThatTypes.GrandChildClass,
					             but all 2 were

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}
		}
	}

	public sealed class AreNotAssignableFrom
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEnumerableTypesAreNotAssignableFromTarget_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), null,];

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<GrandChildClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableFrom<GrandChildClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are assignable from ThatTypes.GrandChildClass,
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
					             at least one is assignable from ThatTypes.GrandChildClass,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeIsAssignableFromTarget_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeTypeIsAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass]
					             none are assignable from ThatTypes.DerivedClass1,
					             but 1 of 1 were

					             Matching items:
					             [
					               ThatTypes.BaseClass
					             ]

					             Collection:
					             [
					               ThatTypes.BaseClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoTypeIsAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableFrom<DerivedClass1>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             at least one is assignable from ThatTypes.DerivedClass1,
					             but none of 1 were

					             Collection:
					             [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}
}
