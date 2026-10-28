using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnePropertyIsNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public ThatProperties.AreNotOfExactType.Dummy ThatProperties.AreNotOfExactType.TestClass.DummyProperty { get; set; },
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOnePropertyIsOfExactType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but it contained properties of exact type ThatProperties.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               public ThatProperties.AreNotOfExactType.DummyBase ThatProperties.AreNotOfExactType.TestClass.DummyBaseProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOnePropertyIsOfExactType_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but it contained properties of exact type ThatProperties.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               public ThatProperties.AreNotOfExactType.DummyBase ThatProperties.AreNotOfExactType.TestClass.DummyBaseProperty { get; set; }
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOnePropertyIsOfExactType_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a property of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but it only contained properties not of exact type ThatProperties.AreNotOfExactType.DummyBase

					             Collection:
					             [
					               public ThatProperties.AreNotOfExactType.Dummy ThatProperties.AreNotOfExactType.TestClass.DummyProperty { get; set; }
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenOnePropertyIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but it contained properties of exact type ThatProperties.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               public ThatProperties.AreNotOfExactType.DummyBase ThatProperties.AreNotOfExactType.TestClass.DummyBaseProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOnePropertyIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty)),
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatProperties.AreNotOfExactType.DummyBase,
					             but it contained properties of exact type ThatProperties.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               public ThatProperties.AreNotOfExactType.DummyBase ThatProperties.AreNotOfExactType.TestClass.DummyBaseProperty { get; set; }
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public DummyBase DummyBaseProperty { get; set; } = null!;
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
