namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class ContainsConstructors
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
			public async Task WhenCheckingTheResult_ShouldNotOfferDirectly()
			{
				async Task Act()
				{
					await That(typeof(Reflection.Results.TypeContainingMembersResult<,>))
						.ContainsMethods(methods => methods.WithName("Directly")).Never().And
						.ContainsMethods(methods => methods.WithName("IncludingObjectMembers")).Never();
				}

				await That(Act).DoesNotThrow()
					.Because("constructors are never inherited");
			}

			[Test]
			public async Task WhenTypeContainsMatchingConstructor_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMarkedConstructor);

				async Task Act()
				{
					await That(subject).ContainsConstructors(constructors => constructors.With<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeContainsNoMatchingConstructor_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMarkedConstructor);

				async Task Act()
				{
					await That(subject).ContainsConstructors(constructors => constructors.With<MarkerAttribute>());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             contains constructors with ThatType.ContainsConstructors.MarkerAttribute at least once,
					             but it contained 0 matching members in ThatType.ContainsConstructors.ClassWithoutMarkedConstructor
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeContainsMatchingConstructor_ShouldFail()
			{
				Type subject = typeof(ClassWithMarkedConstructor);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> it.ContainsConstructors(constructors => constructors.With<MarkerAttribute>()));
				}

				await That(Act).Throws<FailException>()
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
