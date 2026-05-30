using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasNoParameters
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasNoParameters_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([])!;

				async Task Act()
				{
					await That(constructorInfo).HasNoParameters();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasParameters_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).HasNoParameters();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has no parameters,
					             but it had 2 parameters
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasNoParameters();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has no parameters,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasNoParameters_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasNoParameters());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have no parameters,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasParameters_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!;

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasNoParameters());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass() { }
			public TestClass(int value, string name) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
