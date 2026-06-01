using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenNoFieldHasAttribute_ShouldSucceed()
			{
				IEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClass).GetField(nameof(TestClass.NoAttributeField)), null,
				};

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching fields [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching fields [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.TestField
					             ]
					             """);
			}
#endif

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatFields.DoNotHave.AttributeTests.FooAttribute,
					             but it only contained matching fields [
					               string ThatFields.DoNotHave.AttributeTests.TestClass.NoAttributeField
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class FooAttribute : Attribute
			{
			}

#pragma warning disable CS0414 // Field is assigned but its value is never used
			private class TestClass
			{
				[Foo] public string TestField = "";

				public string NoAttributeField = "";
			}
#pragma warning restore CS0414
		}
	}
}
