using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveNoParameters
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllHaveNoParameters_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([])!, typeof(OtherClass).GetConstructor([])!,
				};

				async Task Act()
				{
					await That(constructors).HaveNoParameters();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotAllHaveNoParameters_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([])!, typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveNoParameters();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have no parameters,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.HaveNoParameters.TestClass(int value, string name)
					             ]

					             Collection:
					             [
					               ThatConstructors.HaveNoParameters.TestClass(),
					               ThatConstructors.HaveNoParameters.TestClass(int value, string name)
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllHaveNoParameters_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([])!, typeof(OtherClass).GetConstructor([])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveNoParameters());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             not all have no parameters,
					             but all 2 did

					             Collection:
					             [
					               ThatConstructors.HaveNoParameters.TestClass(),
					               ThatConstructors.HaveNoParameters.OtherClass()
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllHaveNoParameters_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([])!, typeof(TestClass).GetConstructor([typeof(int), typeof(string),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveNoParameters());
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

		private class OtherClass
		{
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
