using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class HaveAnInitSetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllPropertiesHaveAnInitSetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				];

				async Task Act()
				{
					await That(subject).HaveAnInitSetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainPropertiesWithoutAnInitSetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).HaveAnInitSetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all have an init setter,
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
			public async Task WhenAllPropertiesHaveAnInitSetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveAnInitSetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all have an init setter,
					             but all * did

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainPropertiesWithoutAnInitSetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveAnInitSetter());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
