using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class HaveASetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllPropertiesHaveASetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!,
				];

				async Task Act()
				{
					await That(subject).HaveASetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainPropertiesWithoutASetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).HaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all have a setter,
					             but only * of * did

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllPropertiesHaveASetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveASetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all have a setter,
					             but all * did

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainPropertiesWithoutASetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveASetter());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
