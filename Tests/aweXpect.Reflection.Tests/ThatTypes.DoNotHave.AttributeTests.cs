using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class DoNotHave
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenATypeHasAttribute_ShouldFail()
			{
				List<Type?> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.DoNotHave.AttributeTests.FooClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenATypeHasAttributeIndirectly_Directly_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(FooChildClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenATypeHasAttributeIndirectly_ShouldFail()
			{
				List<Type?> subjects = [typeof(FooChildClass2),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.DoNotHave.AttributeTests.FooChildClass2
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(BarClass), null,];

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
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DoNotHave.AttributeTests.BarClass,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatTypes.DoNotHave.AttributeTests.BarClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNoTypeHasAttribute_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(BarClass),];

				async Task Act()
				{
					await That(subjects).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
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

			private class FooChildClass2 : FooClass2
			{
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenNoTypeHasAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(BarClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHave<FooAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have no ThatTypes.DoNotHave.AttributeTests.FooAttribute,
					             but it contained not matching types

					             Not matching items:
					             [
					               ThatTypes.DoNotHave.AttributeTests.FooClass2
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenATypeHasAttribute_ShouldSucceed()
			{
				List<Type?> subjects = [typeof(FooClass2),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoTypeHasAttribute_ShouldFail()
			{
				List<Type?> subjects = [typeof(BarClass),];

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.DoNotHave<FooAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have no ThatTypes.DoNotHave.NegatedTests.FooAttribute,
					             but it only contained matching types

					             Collection:
					             [
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
