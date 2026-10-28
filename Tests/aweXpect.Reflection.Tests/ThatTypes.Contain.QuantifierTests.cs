using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class ContainConstructors
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Exactly_ShouldApplyBothQuantifiers()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithMarkedConstructor), typeof(ClassWithTwoMarkedConstructors),];

				async Task Act()
				{
					await That(subject).AtLeast(2)
						.ContainConstructors(constructors => constructors.With<MarkerAttribute>()).Exactly(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains constructors with ThatTypes.ContainConstructors.MarkerAttribute exactly twice for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainConstructors.ClassWithMarkedConstructor
					             ]

					             Collection:
					             [
					               ThatTypes.ContainConstructors.ClassWithMarkedConstructor,
					               ThatTypes.ContainConstructors.ClassWithTwoMarkedConstructors
					             ]
					             """);
			}
		}
	}

	public sealed partial class ContainEvents
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task None_Directly_WhenNoItemDeclaresAMatchingEvent_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithoutMarkedEvent), typeof(DerivedClassWithInheritedMarkedEvent),];

				async Task Act()
				{
					await That(subject).None().ContainEvents(events => events.With<MarkerAttribute>()).Directly();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}

	public sealed partial class ContainFields
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtMost_WhenTooManyItemsContainAMatchingField_ShouldFail()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithMarkedField), typeof(ClassWithTwoMarkedFields), typeof(ClassWithoutMarkedField),];

				async Task Act()
				{
					await That(subject).AtMost(1).ContainFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains fields with ThatTypes.ContainFields.MarkerAttribute at least once for at most one item,
					             but 2 of 3 did

					             Matching items:
					             [
					               ThatTypes.ContainFields.ClassWithMarkedField,
					               ThatTypes.ContainFields.ClassWithTwoMarkedFields
					             ]

					             Collection:
					             [
					               ThatTypes.ContainFields.ClassWithMarkedField,
					               ThatTypes.ContainFields.ClassWithTwoMarkedFields,
					               ThatTypes.ContainFields.ClassWithoutMarkedField
					             ]
					             """);
			}
		}
	}

	public sealed partial class ContainMethods
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Directly_ShouldOnlyCountDeclaringItems()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithMarkedMethod), typeof(DerivedClassWithInheritedMarkedMethod),];

				async Task Act()
				{
					await That(subject).AtLeast(2).ContainMethods(methods => methods.With<MarkerAttribute>())
						.Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             declares methods with ThatTypes.ContainMethods.MarkerAttribute at least once for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainMethods.DerivedClassWithInheritedMarkedMethod
					             ]

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               ThatTypes.ContainMethods.DerivedClassWithInheritedMarkedMethod
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
					[typeof(ClassWithMarkedMethod), typeof(ClassWithoutMarkedMethod),];

				async Task Act()
				{
					await That(subject).Exactly(1).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithMarkedMethod), null,];

				async Task Act()
				{
					await That(subject).AtLeast(2).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(2).ContainMethods(methods => methods.With<MarkerAttribute>()));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatTypes.ContainMethods.MarkerAttribute at least once for at least 2 items,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatTypes.ContainMethods.MarkerAttribute at least once for fewer than 2 items,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task None_WhenAnItemContainsAMatchingMethod_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(ClassWithoutMarkedMethod), typeof(ClassWithTwoMarkedMethods),];

				async Task Act()
				{
					await That(subject).None().ContainMethods(methods => methods.With<MarkerAttribute>()).AtLeast(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatTypes.ContainMethods.MarkerAttribute at least twice for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             ]

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod,
					               ThatTypes.ContainMethods.ClassWithTwoMarkedMethods
					             ]
					             """);
			}
		}

		public sealed class QuantifiedAsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsContainAMatchingMethod_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithMarkedMethod), typeof(ClassWithoutMarkedMethod),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).ContainMethods(methods => methods.With<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains methods with ThatTypes.ContainMethods.MarkerAttribute at least once for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             ]

					             Collection:
					             [
					               ThatTypes.ContainMethods.ClassWithMarkedMethod,
					               ThatTypes.ContainMethods.ClassWithoutMarkedMethod
					             ]
					             """);
			}
		}
#endif
	}

	public sealed partial class ContainProperties
	{
#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_WhenEnoughItemsContainAMatchingProperty_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(ClassWithMarkedProperty), typeof(ClassWithoutMarkedProperty),];

				async Task Act()
				{
					await That(subject).AtLeast(1).ContainProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
