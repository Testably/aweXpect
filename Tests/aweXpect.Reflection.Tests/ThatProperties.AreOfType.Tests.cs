using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class AreOfType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFailWhenSomePropertiesAreNotOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of type int,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllPropertiesAreOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllPropertiesAreOfType_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are of type int,
					             but all 2 were

					             Collection:
					             [
					               public int ThatProperties.AreOfType.TestClass.IntProperty { get; set; },
					               public int ThatProperties.AreOfType.TestClass.OtherIntProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomePropertiesAreNotOfType_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreOfType<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task ShouldFailWhenSomePropertiesAreNotOfType()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!, typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of type int,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllPropertiesAreOfType()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!, typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
			public int IntProperty { get; set; }
			public int OtherIntProperty { get; set; }
			public string StringProperty { get; set; }
			public Dummy DummyProperty { get; set; }
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldSucceedWhenAllPropertiesAreOfType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.OtherIntProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task WhenAllPropertiesAreOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>().OrOfType(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomePropertiesAreNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<bool>().OrOfType<long>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of type bool or of type long,
					             but none of 2 were

					             Not matching items:
					             [
					               public int ThatProperties.AreOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}
		}
#pragma warning restore CA2263
	}
}
