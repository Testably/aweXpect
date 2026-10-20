using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).HasName("Method");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Method",
					             but it had name "PublicMethod", which differs at index 0:
					                ↓ (actual)
					               "PublicMethod"
					               "Method"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenMethodInfoHasExpectedPrefix_ShouldSucceed()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).HasName("Public").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoHasMatchingName_ShouldSucceed()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).HasName("PublicMethod");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasName("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodInfoMatchesIgnoringCase_ShouldSucceed()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).HasName("pUBLICmethod").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodInfoDoesNotHaveName_ShouldSucceed()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentMethod"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodInfoHasName_ShouldFail()
			{
				MethodInfo? subject =
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("PublicMethod"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicMethod",
					             but it had name "PublicMethod"
					             """);
			}
		}
	}
}
