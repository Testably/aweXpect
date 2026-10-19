using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasParameterCount
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenConstructorHasDifferentCount_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterCount(3);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has 3 parameters,
					             but it had 2 parameters
					             """);
			}

			[Test]
			public async Task WhenConstructorHasExpectedCount_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterCount(2);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasSingleParameter_ShouldDescribeWithSingular()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int),])!;

				async Task Act()
				{
					await That(constructorInfo).HasParameterCount(2);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has 2 parameters,
					             but it had one parameter
					             """);
			}

			[Test]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasParameterCount(0);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has no parameters,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenConstructorHasDifferentCount_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterCount(3));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorHasExpectedCount_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParameterCount(2));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have 2 parameters,
					             but it did
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass(int value) { }
			public TestClass(int value, string name) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
