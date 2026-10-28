using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class Implement
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Directly_ShouldOnlyCountDirectlyImplementingItems()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithInterface1), typeof(DerivedFromClassWithInterface1),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Implement<ITestInterface>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             directly implements ThatTypes.ITestInterface for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.DerivedFromClassWithInterface1
					             ]

					             Collection:
					             [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.DerivedFromClassWithInterface1
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfImplementingItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithInterface1), typeof(UnrelatedClass),];

				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).Exactly(1).Implement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_WhenAnItemImplements_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(UnrelatedClass), typeof(DerivedFromClassWithInterface1),];

				async Task Act()
				{
					await That(subject).None().Implement<ITestInterface>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatTypes.ITestInterface for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.DerivedFromClassWithInterface1
					             ]

					             Collection:
					             [
					               ThatTypes.UnrelatedClass,
					               ThatTypes.DerivedFromClassWithInterface1
					             ]
					             """);
			}

			[Test]
			public async Task WhenInterfaceTypeIsNoInterface_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1),];

				async Task Act()
				{
					await That(subject).AtLeast(1).Implement<BaseClass>();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was ThatTypes.BaseClass. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(ClassWithInterface1),];

				async Task Act()
				{
					await That(subject).AtMost(1).Implement<ITestInterface>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AtMost(1).Implement<ITestInterface>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatTypes.ITestInterface for at most one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               ThatTypes.ClassWithInterface1
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatTypes.ITestInterface for more than one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               ThatTypes.ClassWithInterface1
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsImplement_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(ClassWithInterface1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2).Implement(typeof(ITestInterface));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             implements ThatTypes.ITestInterface for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.ClassWithInterface1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_Directly_WhenTooFewItemsImplementDirectly_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithInterface2), typeof(DerivedFromClassWithInterface1),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).Implement<ITestInterface>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             directly implements ThatTypes.ITestInterface for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.DerivedFromClassWithInterface1
					             ]

					             Collection:
					             [
					               ThatTypes.ClassWithInterface2,
					               ThatTypes.DerivedFromClassWithInterface1
					             ]
					             """);
			}

			[Test]
			public async Task DoNotImplement_WhenNoItemImplements_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass), typeof(DerivedClass1),
				}.ToTestAsyncEnumerable<Type?>();

				Type interfaceType = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).DoNotImplement(interfaceType);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
