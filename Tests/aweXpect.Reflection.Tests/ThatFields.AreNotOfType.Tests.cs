using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = [];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoFieldIsOfType_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneFieldInheritsFromType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
					typeof(TestClass).GetField(nameof(TestClass.DummyField)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type ThatFields.AreNotOfType.DummyBase,
					             but it contained fields of type ThatFields.AreNotOfType.DummyBase

					             Not matching items:
					             [
					               ThatFields.AreNotOfType.Dummy ThatFields.AreNotOfType.TestClass.DummyField
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneFieldIsNull_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
					null,
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type int,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatFields.AreNotOfType.TestClass.StringField,
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneFieldIsOfType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField)),
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				];

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type int,
					             but it contained fields of type int

					             Not matching items:
					             [
					               int ThatFields.AreNotOfType.TestClass.IntField
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<FieldInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenOneFieldIsOfType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField)),
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				];
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type string,
					             but it contained fields of type string

					             Not matching items:
					             [
					               string ThatFields.AreNotOfType.TestClass.StringField
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoFieldIsOfType_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotOfType<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a field of type int,
					             but it only contained fields not of type int

					             Collection:
					             [
					               string ThatFields.AreNotOfType.TestClass.StringField
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneFieldIsOfType_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField)),
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
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
			public async Task WhenNoFieldIsOfType_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneFieldIsOfType_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.IntField)),
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type int,
					             but it contained fields of type int

					             Not matching items:
					             [
					               int ThatFields.AreNotOfType.TestClass.IntField
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOneFieldIsOfType_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.IntField)),
					typeof(TestClass).GetField(nameof(TestClass.StringField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).AreNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not of type int,
					             but it contained fields of type int

					             Not matching items:
					             [
					               int ThatFields.AreNotOfType.TestClass.IntField
					             ]
					             """);
			}
		}
#endif

		private class TestClass
		{
			public int IntField = 1;
			public string StringField = "";
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
