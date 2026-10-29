using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class AreOfType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFailWhenSomeFieldsAreNotOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
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
					               string ThatFields.AreOfType.TestClass.StringField
					             ]

					             Collection:
					             [
					               int ThatFields.AreOfType.TestClass.IntField,
					               string ThatFields.AreOfType.TestClass.StringField
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllFieldsAreOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
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
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
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
			public async Task WhenAllFieldsAreOfType_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
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
					               int ThatFields.AreOfType.TestClass.IntField,
					               int ThatFields.AreOfType.TestClass.OtherIntField
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeFieldsAreNotOfType_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
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
			public async Task ShouldFailWhenSomeFieldsAreNotOfType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.IntField))!, typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

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
					               string ThatFields.AreOfType.TestClass.StringField
					             ]

					             Collection:
					             [
					               int ThatFields.AreOfType.TestClass.IntField,
					               string ThatFields.AreOfType.TestClass.StringField
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllFieldsAreOfType()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.IntField))!, typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.DummyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

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
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public int IntField;
			public int OtherIntField;
			public string StringField;
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

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldSucceedWhenAllFieldsAreOfType()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.OtherIntField))!,
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
			public async Task WhenAllFieldsAreOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
				];

				async Task Act()
				{
					await That(subject).AreOfType<int>().OrOfType(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeFieldsAreNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClass).GetField(nameof(TestClass.IntField))!,
					typeof(TestClass).GetField(nameof(TestClass.StringField))!,
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
					               int ThatFields.AreOfType.TestClass.IntField,
					               string ThatFields.AreOfType.TestClass.StringField
					             ]

					             Collection:
					             [
					               int ThatFields.AreOfType.TestClass.IntField,
					               string ThatFields.AreOfType.TestClass.StringField
					             ]
					             """);
			}
		}
#pragma warning restore CA2263
	}
}
