using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class AreOfExactType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFailWhenPropertiesInheritFromType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type ThatProperties.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]

					             Collection:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllPropertiesAreOfExactType()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Test]
			public async Task WhenAllPropertiesAreOneOfTheExactTypes_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>().OrOfExactType<Dummy>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyStrictlyInheritsFromOrOfExactType_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<int>().OrOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type int or of exact type ThatProperties.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]

					             Collection:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task ShouldFailWhenPropertiesInheritFromType()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type ThatProperties.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]

					             Collection:
					             [
					               public ThatProperties.AreOfExactType.Dummy ThatProperties.AreOfExactType.TestClass.DummyProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllPropertiesAreOfExactType()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!,
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
			public DummyBase DummyBaseProperty { get; set; }
			public Dummy DummyProperty { get; set; }
#pragma warning restore CS8618
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
