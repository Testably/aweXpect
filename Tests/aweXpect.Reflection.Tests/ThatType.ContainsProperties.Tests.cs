using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Exactly_WhenCountMatches_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedProperties);

				async Task Act()
				{
					await That(subject).ContainsProperties(properties => properties.With<MarkerAttribute>())
						.Exactly(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInheritedPropertyMatches_WithDeclaredOnly_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedProperty);

				async Task Act()
				{
					await That(subject).ContainsProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains properties with ThatType.ContainsProperties.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsProperties.DerivedClassWithInheritedMarkedProperty
					             """);
			}

			[Fact]
			public async Task WhenInheritedPropertyMatches_WithIncludingInherited_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithInheritedMarkedProperty);

				async Task Act()
				{
					await That(subject).ContainsProperties(properties => properties.With<MarkerAttribute>(),
						MemberScope.IncludingInherited);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsMatchingProperty_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedProperty);

				async Task Act()
				{
					await That(subject).ContainsProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingProperty_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedProperty);

				async Task Act()
				{
					await That(subject).ContainsProperties(properties => properties.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains properties with ThatType.ContainsProperties.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsProperties.ClassWithoutMarkedProperty
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeContainsMatchingProperty_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedProperty);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsProperties(properties => properties.With<MarkerAttribute>()));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain properties with ThatType.ContainsProperties.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsProperties.ClassWithMarkedProperty
					             """);
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
