using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldIsOfExactType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type ThatField.IsNotOfExactType.DummyBase,
					             but it was of type ThatField.IsNotOfExactType.DummyBase
					             """);
			}

			[Test]
			public async Task WhenFieldTypeInheritsFromType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenFieldIsOfExactType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).IsNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type ThatField.IsNotOfExactType.DummyBase,
					             but it was of type ThatField.IsNotOfExactType.DummyBase
					             """);
			}

			[Test]
			public async Task WithType_WhenFieldTypeInheritsFromType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).IsNotOfExactType(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldIsOfExactType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldTypeInheritsFromType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type ThatField.IsNotOfExactType.DummyBase,
					             but it was of type ThatField.IsNotOfExactType.Dummy
					             """);
			}
		}

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
