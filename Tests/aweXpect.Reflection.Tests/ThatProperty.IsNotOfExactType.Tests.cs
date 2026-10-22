using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotOfExactType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsOfExactType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type ThatProperty.IsNotOfExactType.DummyBase,
					             but it was of type ThatProperty.IsNotOfExactType.DummyBase
					             """);
			}

			[Test]
			public async Task WhenPropertyTypeInheritsFromType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).IsNotOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenPropertyIsOfExactType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).IsNotOfExactType(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type ThatProperty.IsNotOfExactType.DummyBase,
					             but it was of type ThatProperty.IsNotOfExactType.DummyBase
					             """);
			}

			[Test]
			public async Task WithType_WhenPropertyTypeInheritsFromType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;
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
			public async Task WhenPropertyIsOfExactType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyTypeInheritsFromType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfExactType<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type ThatProperty.IsNotOfExactType.DummyBase,
					             but it was of type ThatProperty.IsNotOfExactType.Dummy
					             """);
			}
		}

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
