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
			[Fact]
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

			[Fact]
			public async Task WhenPropertiesContainPropertiesWithoutAnInitSetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).HaveAnInitSetter();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have an init setter,
					             but it contained properties without an init setter [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have an init setter,
					             but it only contained properties with an init setter [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
