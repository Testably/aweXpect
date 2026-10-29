using System.Collections.Generic;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(FooClass2), null,];

				async Task Act()
				{
					await That(subject).Have<FooAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Have<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatTypes.Have.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooClass2,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatTypes.Have.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooClass2,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenTypesHaveAttribute_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types().With<FooAttribute>();

				async Task Act()
				{
					await That(subject).Have<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveAttributeIndirectly_Directly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have direct ThatTypes.Have.AttributeTests.FooAttribute,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesHaveAttributeIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveMatchingAttribute_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveMatchingAttributeIndirectly_Directly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>(foo => foo.Value == 2).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have direct ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 2,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesHaveMatchingAttributeIndirectly_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>(foo => foo.Value == 2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesHaveNotMatchingAttribute_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatTypes.Have.AttributeTests.FooClass2
					             ]

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesHaveNotMatchingAttributeIndirectly_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AttributeTests>().Types()
					.Which(type => type == typeof(FooChildClass2));

				async Task Act()
				{
					await That(subject).Have<FooAttribute>(foo => foo.Value == 3);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type == typeof(FooChildClass2) in assembly containing type ThatTypes.Have.AttributeTests
					             all have ThatTypes.Have.AttributeTests.FooAttribute matching foo => foo.Value == 3,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
					             ]

					             Collection:
					             [
					               ThatTypes.Have.AttributeTests.FooChildClass2
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

			private class FooChildClass2 : FooClass2
			{
			}
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenTypeHasBothAttributes_ShouldSucceed()
				{
					List<Type> types = [typeof(FooBarClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeHasFirstAttribute_ShouldSucceed()
				{
					List<Type> types = [typeof(FooClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeHasMatchingAttribute_ShouldSucceed()
				{
					List<Type> types = [typeof(FooClass2),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>(foo => foo.Value == 2).OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeHasMatchingSecondAttribute_ShouldSucceed()
				{
					List<Type> types = [typeof(BarClass3),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>(foo => foo.Value == 5)
							.OrHave<BarAttribute>(bar => bar.Name == "test");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeHasNeitherAttribute_ShouldFail()
				{
					List<Type> types = [typeof(BazClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that types
						             all have ThatTypes.Have.FooAttribute or ThatTypes.Have.BarAttribute,
						             but none of 1 did

						             Not matching items:
						             [
						               ThatTypes.Have.BazClass
						             ]

						             Collection:
						             [
						               ThatTypes.Have.BazClass
						             ]
						             """);
				}

				[Test]
				public async Task WhenTypeHasSecondAttribute_ShouldSucceed()
				{
					List<Type> types = [typeof(BarClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithDirectly_ShouldWorkCorrectly()
				{
					List<Type> types = [typeof(FooChildClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>().Directly();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that types
						             all have direct ThatTypes.Have.FooAttribute or direct ThatTypes.Have.BarAttribute,
						             but none of 1 did

						             Not matching items:
						             [
						               ThatTypes.Have.FooChildClass
						             ]

						             Collection:
						             [
						               ThatTypes.Have.FooChildClass
						             ]
						             """);
				}

				[Test]
				public async Task WithInheritance_ShouldWorkCorrectly()
				{
					List<Type> types = [typeof(FooChildClass),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenTypeHasNotMatchingAttribute_ShouldFail()
				{
					List<Type> types = [typeof(FooClass2),];

					async Task Act()
					{
						await That(types).Have<FooAttribute>(foo => foo.Value == 5)
							.OrHave<BarAttribute>(bar => bar.Name == "test");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that types
						             all have ThatTypes.Have.FooAttribute matching foo => foo.Value == 5 or ThatTypes.Have.BarAttribute matching bar => bar.Name == "test",
						             but none of 1 did

						             Not matching items:
						             [
						               ThatTypes.Have.FooClass2
						             ]

						             Collection:
						             [
						               ThatTypes.Have.FooClass2
						             ]
						             """);
				}			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertiesHaveAttribute_ShouldFail()
			{
				List<Type> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<FooAttribute>().OrHave<BarAttribute>(x => x.Name == "foo"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have ThatTypes.Have.FooAttribute or ThatTypes.Have.BarAttribute matching x => x.Name == "foo",
					             but all 1 did

					             Collection:
					             [
					               ThatTypes.Have.FooClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenTypesDoNotHaveAttribute_ShouldSucceed()
			{
				List<Type> subjects = [typeof(BazClass),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<FooAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				List<Type> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<FooAttribute>(attr => attr.Value == 3));
				}

				await That(Act).DoesNotThrow();
			}
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
