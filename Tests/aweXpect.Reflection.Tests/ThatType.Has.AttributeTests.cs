using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenTypeHasAttribute_ShouldSucceed()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasAttributeIndirectly_ShouldSucceed()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasAttributeIndirectly_WhenInheritIsFalse_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has direct FooAttribute,
					             but it did not in FooChildClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasMatchingAttribute_ShouldSucceed()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(foo => foo.Value == 2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMatchingAttributeIndirectly_ShouldSucceed()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(foo => foo.Value == 2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMatchingAttributeIndirectly_WhenInheritIsFalse_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(foo => foo.Value == 2, false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has direct FooAttribute matching foo => foo.Value == 2,
					             but it did not in FooChildClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasNotMatchingAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(foo => foo.Value == 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has FooAttribute matching foo => foo.Value == 3,
					             but it did not in FooClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasNotMatchingAttributeIndirectly_ShouldFail()
			{
				Type subject = typeof(FooChildClass2);

				async Task Act()
					=> await That(subject).Has<FooAttribute>(foo => foo.Value == 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has FooAttribute matching foo => foo.Value == 3,
					             but it did not in FooChildClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).Has<FooAttribute>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has FooAttribute,
					             but it was <null>
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

			private class FooChildClass2 : FooClass2
			{
			}
		}
	}
}
