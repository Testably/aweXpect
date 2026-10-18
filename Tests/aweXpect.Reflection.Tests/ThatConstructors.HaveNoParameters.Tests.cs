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
					             but it contained constructors with a different number of parameters*
					             """).AsWildcard();
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
					             but it only contained constructors with no parameters*
					             """).AsWildcard();
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
