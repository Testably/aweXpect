using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveParameterCount
	{
#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<MethodInfo> ToAsyncEnumerable(params MethodInfo[] items)
		{
			foreach (MethodInfo item in items)
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
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectingOneParameter_ShouldIncludeSingularDescription()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterCount(1);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have one parameter,
					             but none of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithIntAndString(int value, string name),
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithStringAndInt(string name, int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithIntAndString(int value, string name),
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithStringAndInt(string name, int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
				{
					await That(methods).HaveParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have 2 parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithIntAndString(int value, string name),
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithInt(int value)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAllHaveExpectedCount_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!);

				async Task Act()
				{
					await That(methods).HaveParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllHaveExpectedCount_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!);

				async Task Act()
				{
					await That(methods).HaveParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have 2 parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithIntAndString(int value, string name),
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithInt(int value)
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
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithStringAndInt))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterCount(2));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             not all have 2 parameters,
					             but all 2 did

					             Collection:
					             [
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithIntAndString(int value, string name),
					               void ThatMethods.HaveParameterCount.TestClass.MethodWithStringAndInt(string name, int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveExpectedCount_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithIntAndString))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParameterCount(2));
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithInt(int value) { }
			public void MethodWithIntAndString(int value, string name) { }
			public void MethodWithStringAndInt(string name, int value) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
