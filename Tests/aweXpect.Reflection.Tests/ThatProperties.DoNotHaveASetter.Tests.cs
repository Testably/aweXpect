using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class DoNotHaveASetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNoPropertyHasASetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).DoesNotThrow().Because("an init-only setter is not a regular setter");
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithASetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a setter,
					             but 1 of 2 did

					             Matching items:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             ]

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; },
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenPropertiesContainNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly)),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a setter,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterOnly { get; },
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a setter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoPropertyHasASetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveASetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one has a setter,
					             but none of 2 did

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterOnly { get; },
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithASetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveASetter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoPropertyHasASetter_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithASetter_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveASetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have a setter,
					             but 1 of 2 did

					             Matching items:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             ]

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; },
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             ]
					             """);
			}
		}
#endif
	}
}
