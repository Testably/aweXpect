using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class InheritFrom
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task All_WhenAllItemsInherit_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).All().InheritFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtLeast_Directly_ShouldOnlyCountDirectlyInheritingItems()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2).InheritFrom<BaseClass>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatTypes.BaseClass for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task AtLeast_WhenEnoughItemsInherit_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass), typeof(DerivedClass2),];

				async Task Act()
				{
					await That(subject).AtLeast(2).InheritFrom(typeof(BaseClass));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtMost_WhenTooManyItemsInherit_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass), typeof(DerivedClass2),];

				async Task Act()
				{
					await That(subject).AtMost(1).InheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for at most one item,
					             but 2 of 3 did

					             Matching items:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}

			[Test]
			public async Task Between_WhenTheNumberOfInheritingItemsIsOutside_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).Between(2).And(3).InheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for between 2 and 3 items,
					             but only 1 of 2 did

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfInheritingItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).Exactly(1).InheritFrom<BaseClass>().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemInherits_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).None().InheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoItemInheritsDirectly_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).None().InheritFrom<BaseClass>().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithInterface1),];

				async Task Act()
				{
					await That(subject).AtLeast(1).InheritFrom<ITestInterface>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface ThatTypes.ITestInterface. Use 'Implements' to check for interface implementations.");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(DerivedClass1),];

				async Task Act()
				{
					await That(subject).AtLeast(1).InheritFrom<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               ThatTypes.DerivedClass1
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               ThatTypes.DerivedClass1
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNegated_ShouldFailWhenTheQuantifierIsMet()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AtLeast(1).InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.DerivedClass1
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_Directly_WhenTooFewItemsInheritDirectly_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(DerivedClass1), typeof(GrandChildClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2).InheritFrom(typeof(BaseClass)).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits directly from ThatTypes.BaseClass for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoItemInherits_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(UnrelatedClass), typeof(ClassWithInterface1),];

				async Task Act()
				{
					await That(subject).None().InheritFrom(typeof(BaseClass));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsInherit_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).InheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for at least 2 items,
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
			public async Task DoNotInheritFrom_WhenAnItemInherits_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(GrandChildClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotInheritFrom<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none inherit from ThatTypes.BaseClass,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.GrandChildClass
					             ]

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               ThatTypes.GrandChildClass
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllItemsInheritDirectly_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).InheritFrom(typeof(BaseClass)).Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), null,
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).Exactly(1).InheritFrom<BaseClass>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Exactly(1).InheritFrom<BaseClass>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for exactly one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               <null>,
					               (… and maybe more)
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             inherits from ThatTypes.BaseClass for not exactly one item,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               <null>,
					               (… and maybe more)
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}
#endif
	}
}
