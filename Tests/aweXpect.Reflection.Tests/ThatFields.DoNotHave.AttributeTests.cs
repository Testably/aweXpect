using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class DoNotHave
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
					             all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching fields

					             Not matching items:
					             [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
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
					             not all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching fields

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
					             all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching fields

					             Not matching items:
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
					             all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
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
					             not all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
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

			[AttributeUsage(AttributeTargets.Field)]
			private class FooAttribute : Attribute
			{
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				public string NoAttributeField = "";
				[Foo] public string TestField = "";
			}
#pragma warning restore CS0414
		}
	}
}
