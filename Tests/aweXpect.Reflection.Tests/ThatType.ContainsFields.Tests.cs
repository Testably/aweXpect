using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsFields
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Never_WhenTypeContainsNoMatchingField_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMarkedField);

				async Task Act()
				{
					await That(subject).ContainsFields(fields => fields.With<MarkerAttribute>()).Never();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInheritedFieldMatches_WithDeclaredOnly_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedField);

				async Task Act()
				{
					await That(subject).ContainsFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains fields with ThatType.ContainsFields.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsFields.DerivedClassWithInheritedMarkedField
					             """);
			}

			[Fact]
			public async Task WhenInheritedFieldMatches_WithIncludingInherited_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedField);

				async Task Act()
				{
					await That(subject).ContainsFields(fields => fields.With<MarkerAttribute>(),
						MemberScope.IncludingInherited);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsMatchingField_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedField);

				async Task Act()
				{
					await That(subject).ContainsFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingField_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedField);

				async Task Act()
				{
					await That(subject).ContainsFields(fields => fields.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains fields with ThatType.ContainsFields.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsFields.ClassWithoutMarkedField
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeContainsMatchingField_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsFields(fields => fields.With<MarkerAttribute>()));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain fields with ThatType.ContainsFields.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsFields.ClassWithMarkedField
					             """);
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
