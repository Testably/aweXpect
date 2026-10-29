using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoPropertyIsOfType_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnePropertyInheritsFromType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type ThatProperties.AreNotOfType.DummyBase,
					             but 1 of 2 were

					             Matching items:
					             [
					               public ThatProperties.AreNotOfType.Dummy ThatProperties.AreNotOfType.TestClass.DummyProperty { get; set; }
					             ]

					             Collection:
					             [
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; },
					               public ThatProperties.AreNotOfType.Dummy ThatProperties.AreNotOfType.TestClass.DummyProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenOnePropertyIsNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type int,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; },
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOnePropertyIsOfType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type int,
					             but 1 of 2 were

					             Matching items:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenOnePropertyIsOfType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				];
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type string,
					             but 1 of 2 were

					             Matching items:
					             [
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoPropertyIsOfType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfType<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is of type int,
					             but none of 1 were

					             Collection:
					             [
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenOnePropertyIsOfType_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfType<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoPropertyIsOfType_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnePropertyIsOfType_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type int,
					             but 1 of 2 were

					             Matching items:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOnePropertyIsOfType_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.IntProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.StringProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are of type int,
					             but 1 of 2 were

					             Matching items:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; }
					             ]

					             Collection:
					             [
					               public int ThatProperties.AreNotOfType.TestClass.IntProperty { get; set; },
					               public string ThatProperties.AreNotOfType.TestClass.StringProperty { get; set; }
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public int IntProperty { get; set; }
			public string StringProperty { get; set; } = "";
			public Dummy DummyProperty { get; set; } = null!;
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
