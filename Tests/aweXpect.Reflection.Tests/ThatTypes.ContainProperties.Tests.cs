using System.Collections.Generic;
using aweXpect.Reflection.Collections;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class ContainProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesContainMatchingProperty_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedProperty, ClassWithTwoMarkedProperties>();

				async Task Act()
				{
					await That(subject).ContainProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenAsyncEnumerableTypesOnlyContainInheritedProperty_WithIncludingInherited_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClassWithInheritedMarkedProperty),
				}.ToTestAsyncEnumerable();

				async Task Act()
				{
					await That(subject).ContainProperties(properties => properties.With<MarkerAttribute>(),
						MemberScope.IncludingInherited);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingProperty_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedProperty, ClassWithoutMarkedProperty>();

				async Task Act()
				{
					await That(subject).ContainProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainProperties.ClassWithMarkedProperty and ThatTypes.ContainProperties.ClassWithoutMarkedProperty
					             all contain properties with ThatTypes.ContainProperties.MarkerAttribute at least once,
					             but it contained not matching types [
					               ThatTypes.ContainProperties.ClassWithoutMarkedProperty
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeOnlyContainsInheritedProperty_WithDeclaredOnly_ShouldFail()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedProperty, BaseClassWithMarkedProperty>();

				async Task Act()
				{
					await That(subject).ContainProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainProperties.DerivedClassWithInheritedMarkedProperty and ThatTypes.ContainProperties.BaseClassWithMarkedProperty
					             all contain properties with ThatTypes.ContainProperties.MarkerAttribute at least once,
					             but it contained not matching types [
					               ThatTypes.ContainProperties.DerivedClassWithInheritedMarkedProperty
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypesOnlyContainInheritedProperty_WithIncludingInherited_ShouldSucceed()
			{
				Filtered.Types subject =
					In.Types<DerivedClassWithInheritedMarkedProperty, BaseClassWithMarkedProperty>();

				async Task Act()
				{
					await That(subject).ContainProperties(properties => properties.With<MarkerAttribute>(),
						MemberScope.IncludingInherited);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingProperty_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedProperty, ClassWithoutMarkedProperty>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.ContainProperties(properties => properties.With<MarkerAttribute>()));
				}

				await That(Act).DoesNotThrow();
			}
		}

		[AttributeUsage(AttributeTargets.Property)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedProperty
		{
			[Marker] public int Id { get; set; }
		}

		private class ClassWithTwoMarkedProperties
		{
			[Marker] public int Id { get; set; }

			[Marker] public string Name { get; set; } = "";
		}

		private class ClassWithoutMarkedProperty
		{
			public int Id { get; set; }
		}

		private class BaseClassWithMarkedProperty
		{
			[Marker] public int Inherited { get; set; }
		}

		private class DerivedClassWithInheritedMarkedProperty : BaseClassWithMarkedProperty
		{
		}
	}
}
