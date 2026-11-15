namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class DoesNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenTypeDoesNotHaveAttribute_ShouldSucceed()
			{
				Type subject = typeof(BarClass);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooClass2
					             """);
			}

			[Test]
			public async Task WhenTypeHasAttributeIndirectly_Directly_ShouldSucceed()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasAttributeIndirectly_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooChildClass2
					             """);
			}

			[Test]
			[Arguments(1)]
			[Arguments(2)]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldFail(int value)
			{
				Type subject = typeof(MultiClass);

				async Task Act()
				{
					await That(subject).DoesNotHave<MultiAttribute>(multi => multi.Value == value);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.MultiAttribute matching multi => multi.Value == value,
					             but it did in ThatType.DoesNotHave.AttributeTests.MultiClass
					             """)
					.Because("every attribute of the type counts, not only the first one");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[Test]
			public async Task WithPredicate_WhenTypeDoesNotHaveAttribute_ShouldSucceed()
			{
				Type subject = typeof(BarClass);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(_ => true);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenTypeHasMatchingAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooClass2
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenTypeHasMatchingAttributeIndirectly_Directly_ShouldSucceed()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the type itself count");
			}

			[Test]
			public async Task WithPredicate_WhenTypeHasMatchingAttributeIndirectly_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooChildClass2
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenTypeHasNotMatchingAttribute_ShouldSucceed()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("an attribute that does not match the predicate does not count");
			}

			[Test]
			public async Task WithPredicate_WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it was <null>
					             """)
					.Because("a null type cannot be inspected, so the negation must not pass");
			}

			[Test]
			public async Task WithType_WhenTypeHasAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				Type attributeType = typeof(FooAttribute);

				async Task Act()
				{
					await That(subject).DoesNotHave(attributeType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.DoesNotHave.AttributeTests.FooAttribute,
					             but it did in ThatType.DoesNotHave.AttributeTests.FooClass2
					             """);
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class BarAttribute : Attribute
			{
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			[Bar]
			private class BarClass
			{
			}

			private class FooChildClass2 : FooClass2
			{
			}

			[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
			private class MultiAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Multi(Value = 1)]
			[Multi(Value = 2)]
			private class MultiClass
			{
			}
		}
	}
}
