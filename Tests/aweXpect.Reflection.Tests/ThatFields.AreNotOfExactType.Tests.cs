using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneFieldIsNull_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatFields.AreNotOfExactType.Dummy ThatFields.AreNotOfExactType.TestClass.DummyField,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneFieldIsOfExactType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but it contained fields of exact type ThatFields.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               ThatFields.AreNotOfExactType.DummyBase ThatFields.AreNotOfExactType.TestClass.DummyBaseField
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOneFieldIsOfExactType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but it contained fields of exact type ThatFields.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               ThatFields.AreNotOfExactType.DummyBase ThatFields.AreNotOfExactType.TestClass.DummyBaseField
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOneFieldIsOfExactType_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedFields_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfExactType<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a field of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but it only contained fields not of exact type ThatFields.AreNotOfExactType.DummyBase

					             Collection:
					             [
					               ThatFields.AreNotOfExactType.Dummy ThatFields.AreNotOfExactType.TestClass.DummyField
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenOneFieldIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but it contained fields of exact type ThatFields.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               ThatFields.AreNotOfExactType.DummyBase ThatFields.AreNotOfExactType.TestClass.DummyBaseField
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsDerivedFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOneFieldIsOfExactType_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyBaseField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).AreNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of exact type ThatFields.AreNotOfExactType.DummyBase,
					             but it contained fields of exact type ThatFields.AreNotOfExactType.DummyBase

					             Not matching items:
					             [
					               ThatFields.AreNotOfExactType.DummyBase ThatFields.AreNotOfExactType.TestClass.DummyBaseField
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public DummyBase DummyBaseField = null!;
			public Dummy DummyField = null!;
		}

		private class DummyBase
		{
		}

		private class Dummy : DummyBase
		{
		}
	}
}
