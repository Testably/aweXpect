using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatConstructor.DoesNotHave.AttributeTests.TestClass(string value)
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenConstructorDoesNotHaveAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenConstructorHasMatchingAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in ThatConstructor.DoesNotHave.AttributeTests.TestClass(string value)
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenConstructorHasNotMatchingAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[Test]
			public async Task WithType_WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatConstructor.DoesNotHave.AttributeTests.TestClass(string value)
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass()
				{
				}

				[Foo(Value = 2)]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value)
				{
				}
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
