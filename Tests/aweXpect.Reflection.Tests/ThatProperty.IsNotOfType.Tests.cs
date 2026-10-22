using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotOfType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsOfDifferentType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsOfType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

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
			public async Task WhenPropertyTypeInheritsFromType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).IsNotOfType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of type ThatProperty.IsNotOfType.DummyBase,
					             but it was of type ThatProperty.IsNotOfType.Dummy
					             """);
			}

			[Test]
			public async Task WithType_WhenPropertyIsOfDifferentType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).IsNotOfType(unexpectedType);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenPropertyIsOfType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;
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
			public async Task WhenPropertyIsOfDifferentType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

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
			public async Task WhenPropertyIsOfType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.IntProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotOfType<int>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private class TestClass
		{
			public int IntProperty { get; set; }
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
