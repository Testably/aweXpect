using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsOfExactType
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfExactType<int>();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is of exact type int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldIsOfExactType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldTypeInheritsFromExpectedType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<DummyBase>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type *DummyBase,
					             but it was of type *Dummy
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsOfExactType_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfExactType<DummyBase>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type *DummyBase,
					             but it did
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldTypeInheritsFromExpectedType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfExactType<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}
		}

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

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload
		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenFieldIsOfExactType_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyBaseField))!;

				async Task Act()
				{
					await That(subject).IsOfExactType(typeof(DummyBase));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Fact]
			public async Task WithMultipleOrOfExactType_ShouldSupportChaining()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.DummyField))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<DummyBase>().OrOfType(typeof(bool)).OrOfExactType<Dummy>();
				}

				await That(Act).DoesNotThrow();
			}
		}
#pragma warning restore CA2263
	}
}
