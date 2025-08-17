using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.TestAttribute,
					             but it did not in Void .ctor()
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasMatchingAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == "ConstructorValue");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasNotMatchingAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(subject).Has<TestAttribute>(attr => attr.Value == "WrongValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.TestAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in Void .ctor(System.String)
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
					=> await That(subject).Has<TestAttribute>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.TestAttribute,
					             but it was <null>
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }

				[Test(Value = "ConstructorValue")]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value) { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHas
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenConstructorHasBothAttributes_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorHasFirstAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorHasMatchingAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 1)
							.OrHas<BarAttribute>(bar => bar.Name == "does-not-match");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorHasMatchingSecondAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == int.MaxValue)
							.OrHas<BarAttribute>(bar => bar.Name == "foo");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenConstructorHasNeitherAttribute_ShouldFail()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatConstructor.Has.OrHas.AttributeTests.FooAttribute or ThatConstructor.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in Void .ctor()
						             """);
				}

				[Fact]
				public async Task WhenConstructorHasSecondAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenConstructorHasNotMatchingAttribute_ShouldFail()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
						=> await That(subject).Has<FooAttribute>(foo => foo.Value == 42)
							.OrHas<BarAttribute>(bar => bar.Name == "does-not-match");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has ThatConstructor.Has.OrHas.AttributeTests.FooAttribute matching foo => foo.Value == 42 or ThatConstructor.Has.OrHas.AttributeTests.BarAttribute matching bar => bar.Name == "does-not-match",
						             but it did not in Void .ctor(Int32, System.String)
						             """);
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class FooAttribute : Attribute
				{
					public int Value { get; set; }
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class BarAttribute : Attribute
				{
					public string? Name { get; set; }
				}

				// ReSharper disable UnusedMember.Local
				// ReSharper disable UnusedParameter.Local
				private class TestClass
				{
					public TestClass() { }

					[Foo(Value = 1)]
					[Bar(Name = "foo")]
					public TestClass(int value, string name) { }

					[Foo(Value = 1)]
					public TestClass(int value) { }

					[Bar(Name = "foo")]
					public TestClass(string name) { }
				}
				// ReSharper restore UnusedParameter.Local
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.Has<TestAttribute>(attr => attr.Value == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Has<TestAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.Has.NegatedTests.TestAttribute,
					             but it did in Void .ctor(System.String)
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class TestAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }

				[Test(Value = "ConstructorValue")]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value) { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
