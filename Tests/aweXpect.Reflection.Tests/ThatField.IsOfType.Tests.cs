using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsOfType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
					=> await That(subject).IsOfType<int>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is of type int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldIsOfDifferentType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WhenFieldIsOfExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldTypeInheritsFromExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
					=> await That(subject).IsOfType<DummyBase>();

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenFieldIsOfDifferentType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType(typeof(string));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string,
					             but it was of type int
					             """);
			}

			[Fact]
			public async Task WhenFieldIsOfExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType(typeof(int));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfTypeTests
		{
			[Fact]
			public async Task WithMultipleOrOfType_ShouldSupportChaining()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType<string>().OrOfType(typeof(bool)).OrOfType<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNoneOfTheTypes_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType<string>().OrOfType<bool>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of type string or of type bool,
					             but it was of type int
					             """);
			}
		}
#pragma warning restore CA2263

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WhenFieldIsOneOfTheTypes_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).IsOfType<string>().OrOfExactType<int>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsOfDifferentType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOfType<string>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsOfExpectedType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.IntField))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOfType<int>());

				await That(Act).Throws<XunitException>()
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
	}
}
