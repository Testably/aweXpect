using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class DoesNotHaveASetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyHasASetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             does not have a setter,
					             but it had a setter public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyHasNoSetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveASetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasOnlyAnInitSetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!;

				async Task Act()
				{
					await That(subject).DoesNotHaveASetter();
				}

				await That(Act).DoesNotThrow().Because("an init-only setter is not a regular setter");
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             does not have a setter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyHasASetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveASetter());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasOnlyAnInitSetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotHaveASetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has a setter,
					             but it did not have a setter public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
					             """);
			}
		}
	}
}
