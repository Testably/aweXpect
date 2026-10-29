using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveNoParameters
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllHaveNoParameters_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithoutParameters))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithoutParameters))!,
				};

				async Task Act()
				{
					await That(methods).HaveNoParameters();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveNoParameters_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithoutParameters))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
				{
					await That(methods).HaveNoParameters();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             all have no parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               void ThatMethods.HaveNoParameters.TestClass.MethodWithInt(int value)
					             ]

					             Collection:
					             [
					               void ThatMethods.HaveNoParameters.TestClass.FirstMethodWithoutParameters(),
					               void ThatMethods.HaveNoParameters.TestClass.MethodWithInt(int value)
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveNoParameters_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithoutParameters))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithoutParameters))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveNoParameters());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods
					             not all have no parameters,
					             but all 2 did

					             Collection:
					             [
					               void ThatMethods.HaveNoParameters.TestClass.FirstMethodWithoutParameters(),
					               void ThatMethods.HaveNoParameters.TestClass.SecondMethodWithoutParameters()
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveNoParameters_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithoutParameters))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInt))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveNoParameters());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void FirstMethodWithoutParameters() { }
			public void SecondMethodWithoutParameters() { }
			public void MethodWithInt(int value) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
