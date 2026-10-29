using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class AreOfExactType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFailWhenFieldsInheritFromType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type ThatFields.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]

					             Collection:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllFieldsAreOfExactType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
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
			public async Task WhenAllFieldsAreOneOfTheExactTypes_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>().OrOfExactType<Dummy>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldStrictlyInheritsFromOrOfExactType_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfExactType<int>().OrOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type int or of exact type ThatFields.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]

					             Collection:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task ShouldFailWhenFieldsInheritFromType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are of exact type ThatFields.AreOfExactType.DummyBase,
					             but none of 1 were

					             Not matching items:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]

					             Collection:
					             [
					               ThatFields.AreOfExactType.Dummy ThatFields.AreOfExactType.TestClass.DummyField
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllFieldsAreOfExactType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

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
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public DummyBase DummyBaseField;
			public Dummy DummyField;
#pragma warning restore CS0649
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
