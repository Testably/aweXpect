using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsOfExactType
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsOfExactType<int>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is of exact type int,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsOfExactType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyTypeInheritsFromExpectedType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).IsOfExactType<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type *DummyBase,
					             but it was of type *Dummy
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsOfExactType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsOfExactType<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not of exact type *DummyBase,
					             but it was of type *DummyBase
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertyTypeInheritsFromExpectedType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

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
			public DummyBase DummyBaseProperty { get; set; }
			public Dummy DummyProperty { get; set; }
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
			public async Task WhenPropertyIsOfExactType_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyBaseProperty))!;

				async Task Act()
				{
					await That(subject).IsOfExactType(typeof(DummyBase));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyTypeInheritsFromExpectedType_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

				async Task Act()
				{
					await That(subject).IsOfExactType(typeof(DummyBase));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is of exact type *DummyBase,
					             but it was of type *Dummy
					             """).AsWildcard();
			}
		}

		public sealed class OrOfExactTypeTests
		{
			[Test]
			public async Task WithMultipleOrOfExactType_ShouldSupportChaining()
			{
				PropertyInfo subject = typeof(TestClass).GetProperty(nameof(TestClass.DummyProperty))!;

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
