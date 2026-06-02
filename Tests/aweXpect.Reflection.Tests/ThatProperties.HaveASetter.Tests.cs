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
			[Fact]
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

			[Fact]
			public async Task WhenPropertiesContainPropertiesWithoutASetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).HaveASetter();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have a setter,
					             but it contained properties without a setter [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have a setter,
					             but it only contained properties with a setter [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
