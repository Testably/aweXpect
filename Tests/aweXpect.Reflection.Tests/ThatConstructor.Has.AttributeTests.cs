using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class Has
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.MarkerAttribute,
					             but it did not in ThatConstructor.Has.AttributeTests.TestClass()
					             """);
			}

			[Test]
			public async Task WhenConstructorHasAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasMatchingAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "ConstructorValue");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasNotMatchingAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but it did not in ThatConstructor.Has.AttributeTests.TestClass(string value)
					             """);
			}

			[Test]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
				{
					await That(subject).Has<MarkerAttribute>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has ThatConstructor.Has.AttributeTests.MarkerAttribute,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenConstructorHasAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				Type attributeType = typeof(MarkerAttribute);

				async Task Act()
				{
					await That(subject).Has(attributeType);
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }

				[Marker(Value = "ConstructorValue")]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value) { }
			}
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHas
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenConstructorHasBothAttributes_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorHasFirstAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorHasMatchingAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == 1)
							.OrHas<BarAttribute>(bar => bar.Name == "does-not-match");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorHasMatchingSecondAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == int.MaxValue)
							.OrHas<BarAttribute>(bar => bar.Name == "foo");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorHasNeitherAttribute_ShouldFail()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has ThatConstructor.Has.OrHas.AttributeTests.FooAttribute or ThatConstructor.Has.OrHas.AttributeTests.BarAttribute,
						             but it did not in ThatConstructor.Has.OrHas.AttributeTests.TestClass()
						             """);
				}

				[Test]
				public async Task WhenConstructorHasSecondAttribute_ShouldSucceed()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>().OrHas<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenConstructorHasNotMatchingAttribute_ShouldFail()
				{
					ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

					async Task Act()
					{
						await That(subject).Has<FooAttribute>(foo => foo.Value == 42)
							.OrHas<BarAttribute>(bar => bar.Name == "does-not-match");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has ThatConstructor.Has.OrHas.AttributeTests.FooAttribute matching foo => foo.Value == 42 or ThatConstructor.Has.OrHas.AttributeTests.BarAttribute matching bar => bar.Name == "does-not-match",
						             but it did not in ThatConstructor.Has.OrHas.AttributeTests.TestClass(int value, string name)
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
			[Test]
			public async Task WhenConstructorDoesNotHaveAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorDoesNotHaveMatchingAttribute_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.Has<MarkerAttribute>(attr => attr.Value == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasAttribute_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClass).GetConstructor([typeof(string),])!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Has<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has no ThatConstructor.Has.NegatedTests.MarkerAttribute,
					             but it did in ThatConstructor.Has.NegatedTests.TestClass(string value)
					             """);
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			private class TestClass
			{
				public TestClass() { }

				[Marker(Value = "ConstructorValue")]
				// ReSharper disable once UnusedParameter.Local
				public TestClass(string value) { }
			}
			// ReSharper restore UnusedMember.Local
		}
	}
}
