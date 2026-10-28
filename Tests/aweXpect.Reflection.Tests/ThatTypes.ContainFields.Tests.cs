using System.Collections.Generic;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class ContainFields
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesContainMatchingField_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedField, ClassWithTwoMarkedFields>();

				async Task Act()
				{
					await That(subject).ContainFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenAsyncEnumerableTypesOnlyContainInheritedField_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithInheritedMarkedField),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).ContainFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task WhenSomeTypeContainsNoMatchingField_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedField, ClassWithoutMarkedField>();

				async Task Act()
				{
					await That(subject).ContainFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainFields.ClassWithMarkedField and ThatTypes.ContainFields.ClassWithoutMarkedField
					             all contain fields with ThatTypes.ContainFields.MarkerAttribute at least once,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainFields.ClassWithoutMarkedField
					             ]

					             Collection:
					             [
					               ThatTypes.ContainFields.ClassWithMarkedField,
					               ThatTypes.ContainFields.ClassWithoutMarkedField
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypeOnlyContainsInheritedField_Directly_ShouldFail()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedField, BaseClassWithMarkedField>();

				async Task Act()
				{
					await That(subject).ContainFields(fields => fields.With<MarkerAttribute>()).Directly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainFields.DerivedClassWithInheritedMarkedField and ThatTypes.ContainFields.BaseClassWithMarkedField
					             all declare fields with ThatTypes.ContainFields.MarkerAttribute at least once,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatTypes.ContainFields.DerivedClassWithInheritedMarkedField
					             ]

					             Collection:
					             [
					               ThatTypes.ContainFields.DerivedClassWithInheritedMarkedField,
					               ThatTypes.ContainFields.BaseClassWithMarkedField
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesOnlyContainInheritedField_ShouldSucceed()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedField, BaseClassWithMarkedField>();

				async Task Act()
				{
					await That(subject).ContainFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSomeTypeContainsNoMatchingField_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedField, ClassWithoutMarkedField>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.ContainFields(fields => fields.With<MarkerAttribute>()));
				}

				await That(Act).DoesNotThrow();
			}
		}

		[AttributeUsage(AttributeTargets.Field)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedField
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			[Marker] public int Value;
#pragma warning restore CS0649
		}

		private class ClassWithTwoMarkedFields
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			[Marker] public int First;

			[Marker] public int Second;
#pragma warning restore CS0649
		}

		private class ClassWithoutMarkedField
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public int Value;
#pragma warning restore CS0649
		}

		private class BaseClassWithMarkedField
		{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			[Marker] public int Inherited;
#pragma warning restore CS0649
		}

		private class DerivedClassWithInheritedMarkedField : BaseClassWithMarkedField
		{
		}
	}
}
