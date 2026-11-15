using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenFieldDoesNotHaveAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.NoAttributeField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.TestField
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenFieldDoesNotHaveAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.NoAttributeField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenFieldHasMatchingAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.TestField
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenFieldHasNotMatchingAttribute_ShouldSucceed()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldFail(int value)
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.MultiField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<MultiAttribute>(multi => multi.Value == value);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.MultiAttribute matching multi => multi.Value == value,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.MultiField
					             """)
					.Because("every attribute of the field counts, not only the first one");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[Test]
			public async Task WithType_WhenFieldHasAttribute_ShouldFail()
			{
				FieldInfo subject = typeof(TestClass).GetField(nameof(TestClass.TestField))!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatField.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in string ThatField.DoesNotHave.AttributeTests.TestClass.TestField
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Foo(Value = 2)] public string TestField = "";
				[Multi(Value = 1)] [Multi(Value = 2)] public string MultiField = "";
			}
#pragma warning restore CS0414
		}
	}
}
