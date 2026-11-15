using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAFieldHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenAFieldHasMatchingAttribute_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 1 did

					             Matching items:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenNoFieldHasMatchingAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)),
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task Negated_WhenNoFieldHasAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.NoAttributeField
					             ]
					             """);
			}

			[Test]
			public async Task WhenAFieldHasAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<FieldInfo?> subject = [typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)), null,];

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.NoAttributeField,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.NoAttributeField,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoFieldHasAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithPredicate_WhenAFieldHasMatchingAttribute_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)),
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but 1 of 2 did

					             Matching items:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.NoAttributeField,
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}

			[Test]
			public async Task WithPredicate_WhenNoFieldHasMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)),
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).DoesNotThrow()
					.Because("neither a missing attribute nor one that does not match the predicate counts");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.TestField)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>(_ => throw new InvalidOperationException("boom"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatFields.DoNotHave.AttributeTests.FooAttribute matching _ => throw new InvalidOperationException("boom"),
					             but the predicate did throw an InvalidOperationException:
					               boom

					             Collection:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """)
					.Because("an exception of the predicate must fail the negated expectation as well instead of passing");
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Foo(Value = 2)] public string TestField = "";
			}
#pragma warning restore CS0414
		}
	}
}
