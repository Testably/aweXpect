using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed partial class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAPropertyHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.TestProperty)),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]
					             """);
			}
#endif

			[Test]
			public async Task Negated_WhenNoPropertyHasAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty)),
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but none of 1 did

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.NoAttributeProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenAPropertyHasAttribute_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.TestProperty)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<PropertyInfo?> subject = [typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty)), null,];

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
					             none have ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.NoAttributeProperty { get; set; },
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one has ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.NoAttributeProperty { get; set; },
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoPropertyHasAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_Directly_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the attributes declared on the overriding property count");
			}

			[Test]
			public async Task WhenPropertyInheritsAttributeFromOverriddenProperty_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(DerivedClass).GetProperty(nameof(DerivedClass.InheritedProperty)),
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none have ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but 1 of 1 did

					             Matching items:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.DerivedClass.InheritedProperty { get; set; }
					             ]

					             Collection:
					             [
					               public string ThatProperties.DoNotHave.AttributeTests.DerivedClass.InheritedProperty { get; set; }
					             ]
					             """)
					.Because("the attribute on the overridden base property is inherited, as in the Properties().With<TAttribute>() filter");
			}

			[AttributeUsage(AttributeTargets.Property)]
			private class FooAttribute : Attribute
			{
			}

			private class TestClass
			{
				[Foo] public string TestProperty { get; set; } = "";

				public string NoAttributeProperty { get; set; } = "";
			}

			private class BaseClass
			{
				[Foo] public virtual string InheritedProperty { get; set; } = "";
			}

			private class DerivedClass : BaseClass
			{
				public override string InheritedProperty { get; set; } = "";
			}
		}
	}
}
