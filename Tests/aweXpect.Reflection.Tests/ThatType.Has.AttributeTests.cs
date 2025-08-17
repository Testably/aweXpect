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
					             has direct ThatType.Has.AttributeTests.FooAttribute,
					             but it did not in ThatType.Has.AttributeTests.FooChildClass2
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
					             has direct ThatType.Has.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but it did not in ThatType.Has.AttributeTests.FooChildClass2
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
					             has ThatType.Has.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but it did not in ThatType.Has.AttributeTests.FooClass2
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
					             has ThatType.Has.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but it did not in ThatType.Has.AttributeTests.FooChildClass2
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
					             has ThatType.Has.AttributeTests.FooAttribute,
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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeDoesNotHaveAttribute_ShouldSucceed()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<BarAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<FooAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.Has.NegatedTests.FooAttribute,
					             but it did in ThatType.Has.NegatedTests.FooClass2
					             """);
			}

			[Fact]
			public async Task WhenTypeHasMatchingAttribute_ShouldFail()
			{
				Type subject = typeof(FooClass2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<FooAttribute>(foo => foo.Value == 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatType.Has.NegatedTests.FooAttribute matching foo => foo.Value == 2,
					             but it did in ThatType.Has.NegatedTests.FooClass2
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
		}

		public sealed class OrHas
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenTypeHasBothAttributes_ShouldSucceed()
				{
					Type subject = typeof(FooBarClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeHasFirstAttribute_ShouldSucceed()
				{
					Type subject = typeof(FooClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeHasMatchingAttribute_ShouldSucceed()
				{
					Type subject = typeof(FooClass2);

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 2).OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeHasMatchingSecondAttribute_ShouldSucceed()
				{
					Type subject = typeof(BarClass3);

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeHasNeitherAttribute_ShouldFail()
				{
					Type subject = typeof(BazClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatType.Has.OrHas.AttributeTests.FooAttribute or ThatType.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in ThatType.Has.OrHas.AttributeTests.BazClass
						             """);
				}

				[Fact]
				public async Task WhenTypeHasSecondAttribute_ShouldSucceed()
				{
					Type subject = typeof(BarClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithInheritance_ShouldWorkCorrectly()
				{
					Type subject = typeof(FooChildClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithInheritanceFalse_ShouldWorkCorrectly()
				{
					Type subject = typeof(FooChildClass);

					async Task Act()
						=> await That(subject).Has<FooAttribute>(false).OrHas<BarAttribute>(false);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has direct ThatType.Has.OrHas.AttributeTests.FooAttribute or direct ThatType.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in ThatType.Has.OrHas.AttributeTests.FooChildClass
						             """);
				}

				[Fact]
				public async Task WithPredicate_WhenTypeHasNotMatchingAttribute_ShouldFail()
				{
					Type subject = typeof(FooClass2);

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 5)
							.OrHas<BarAttribute>(bar => bar.Name == "test");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatType.Has.OrHas.AttributeTests.FooAttribute matching foo => foo.Value == 5 or ThatType.Has.OrHas.AttributeTests.BarAttribute matching bar => bar.Name == "test",
						             but it did not in ThatType.Has.OrHas.AttributeTests.FooClass2
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
					public string? Name { get; set; }
				}

				[AttributeUsage(AttributeTargets.Class)]
				private class BazAttribute : Attribute
				{
				}

				[Foo]
				private class FooClass
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

				[Bar(Name = "test")]
				private class BarClass3
				{
				}

				[Foo]
				[Bar]
				private class FooBarClass
				{
				}

				[Baz]
				private class BazClass
				{
				}

				private class FooChildClass : FooClass
				{
				}
			}
		}
	}
}
