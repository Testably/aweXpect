using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParameterCount
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<ConstructorInfo> ToAsyncEnumerable(params ConstructorInfo[] items)
		{
			foreach (ConstructorInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif

		public sealed class Tests
		{
			[Test]
			public async Task WhenAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectingOneParameter_ShouldMentionOneParameterInExpectation()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterCount(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have one parameter,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have 2 parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name),
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveExpectedCount_ShouldListNotMatchingConstructors()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have 2 parameters,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAllHaveExpectedCount_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo> constructors = ToAsyncEnumerable(
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
					typeof(TestClass).GetConstructor([typeof(int),])!);

				async Task Act()
				{
					await That(constructors).HaveParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have 2 parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name),
					               ThatConstructors.HaveParameterCount.TestClass(int value)
					             ]
					             """);
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, typeof(TestClass).GetConstructor([typeof(string), typeof(int),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterCount(2));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             not all have 2 parameters,
					             but all 2 did

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name),
					               ThatConstructors.HaveParameterCount.TestClass(string name, int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllHaveExpectedCount_ShouldListMatchingConstructors()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterCount(2));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             not all have 2 parameters,
					             but all 1 did

					             Collection:
					             [
					               ThatConstructors.HaveParameterCount.TestClass(int value, string name)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParameterCount(2));
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass(int value) { }
			public TestClass(int value, string name) { }
			public TestClass(string name, int value) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
