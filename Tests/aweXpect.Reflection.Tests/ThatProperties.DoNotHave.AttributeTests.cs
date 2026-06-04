using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
#if NET8_0_OR_GREATER
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching properties [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]
					             """);
			}
#endif

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching properties [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.NoAttributeProperty { get; set; }
					             ]
					             """);
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatProperties.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching properties [
					               public string ThatProperties.DoNotHave.AttributeTests.TestClass.TestProperty { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoPropertyHasAttribute_ShouldSucceed()
			{
				IEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(TestClass).GetProperty(nameof(TestClass.NoAttributeProperty)), null,
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
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
		}
	}
}
