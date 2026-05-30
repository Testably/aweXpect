using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsConstructors
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Exactly_WhenCountMatches_ShouldSucceed()
			{
				Type subject = typeof(ClassWithTwoMarkedConstructors);

				async Task Act()
				{
					await That(subject).ContainsConstructors(constructors => constructors.With<MarkerAttribute>())
						.Exactly(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsMatchingConstructor_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedConstructor);

				async Task Act()
				{
					await That(subject).ContainsConstructors(constructors => constructors.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeContainsNoMatchingConstructor_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedConstructor);

				async Task Act()
				{
					await That(subject).ContainsConstructors(constructors => constructors.With<MarkerAttribute>());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             contains constructors with ThatType.ContainsConstructors.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsConstructors.ClassWithoutMarkedConstructor
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeContainsMatchingConstructor_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedConstructor);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsConstructors(constructors => constructors.With<MarkerAttribute>()));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain constructors with ThatType.ContainsConstructors.MarkerAttribute at least once,
					             but it contained 1 matching member in ThatType.ContainsConstructors.ClassWithMarkedConstructor
					             """);
			}
		}

		[AttributeUsage(AttributeTargets.Constructor)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedConstructor
		{
			[Marker]
			public ClassWithMarkedConstructor()
			{
			}
		}

		private class ClassWithTwoMarkedConstructors
		{
			[Marker]
			public ClassWithTwoMarkedConstructors()
			{
			}

			[Marker]
			public ClassWithTwoMarkedConstructors(int value)
			{
			}
		}

		private class ClassWithoutMarkedConstructor
		{
		}
	}
}
