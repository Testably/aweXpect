using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenNoTypeHasAttribute_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(BarClass), null,];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenATypeHasAttribute_ShouldFail()
			{
				List<Type?> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types [
					               ThatTypes.DoNotHave.AttributeTests.FooClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenATypeHasAttributeIndirectly_ShouldFail()
			{
				List<Type?> subjects = [typeof(FooChildClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types [
					               ThatTypes.DoNotHave.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenATypeHasAttributeIndirectly_WhenInheritIsFalse_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(FooChildClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>(false);
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WhenNoTypeHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(BarClass), null,
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WhenATypeHasAttribute_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(FooClass2),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types [
					               ThatTypes.DoNotHave.AttributeTests.FooClass2
					             ]
					             """);
			}
#endif

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			private class BarClass
			{
			}

			private class FooChildClass2 : FooClass2
			{
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenATypeHasAttribute_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeHasAttribute_ShouldFail()
			{
				List<Type?> subjects = [typeof(BarClass),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have no ThatTypes.DoNotHave.NegatedTests.FooAttribute,
					             but it only contained matching types [
					               ThatTypes.DoNotHave.NegatedTests.BarClass
					             ]
					             """);
			}

			[AttributeUsage(AttributeTargets.Class)]
			private class FooAttribute : Attribute
			{
				public int Value { get; set; }
			}

			[Foo(Value = 2)]
			private class FooClass2
			{
			}

			private class BarClass
			{
			}
		}
	}
}
