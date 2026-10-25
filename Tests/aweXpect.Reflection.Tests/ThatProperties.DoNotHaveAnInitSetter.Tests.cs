using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class DoNotHaveAnInitSetter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNoPropertyHasAnInitSetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithAnInitSetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have an init setter,
					             but 1 of 2 did

					             Matching items:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
					             ]

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; },
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
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
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have an init setter,
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
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have an init setter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoPropertyHasAnInitSetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveAnInitSetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one has an init setter,
					             but none of 2 did

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterOnly { get; },
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithAnInitSetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHaveAnInitSetter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoPropertyHasAnInitSetter_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainAPropertyWithAnInitSetter_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!,
					typeof(TestClassWithPropertyAccessors)
						.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHaveAnInitSetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none have an init setter,
					             but 1 of 2 did

					             Matching items:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
					             ]

					             Collection:
					             [
					               public string TestClassWithPropertyAccessors.WithGetterAndSetter { get; set; },
					               public string TestClassWithPropertyAccessors.WithGetterAndInitSetter { get; set; }
					             ]
					             """);
			}
		}
#endif
	}
}
