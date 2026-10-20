using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsOfType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfType<int>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfDifferentType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldTypeInheritsFromExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).IsOfType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Test]
			public async Task WhenFieldIsOneOfTheTypes_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfExactType<int>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldIsOfDifferentType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<string>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsOfExpectedType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfType<int>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type int,
					             but it did
					             """);
			}
		}

		private class TestClass
		{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			public int IntField;
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
			public async Task WhenFieldIsOfDifferentType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(string));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Test]
			public async Task WhenFieldIsNoneOfTheTypes_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfType<bool>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type string or of type bool,
					             but it was of type int
					             """);
			}

			[Test]
			public async Task WithMultipleOrOfType_ShouldSupportChaining()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsOfType<string>().OrOfType(typeof(bool)).OrOfType<int>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#pragma warning restore CA2263
	}
}
