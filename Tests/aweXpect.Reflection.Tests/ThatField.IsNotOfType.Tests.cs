using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfDifferentType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsOfType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type int,
					             but it was of type int
					             """);
			}

			[Test]
			public async Task WhenFieldTypeInheritsFromType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type ThatField.IsNotOfType.DummyBase,
					             but it was of type ThatField.IsNotOfType.Dummy
					             """);
			}

			[Test]
			public async Task WithType_WhenFieldIsOfDifferentType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).IsNotOfType(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenFieldIsOfType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).IsNotOfType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type int,
					             but it was of type int
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldIsOfDifferentType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfType<string>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfType<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private class TestClass
		{
			public int IntField = 1;
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
