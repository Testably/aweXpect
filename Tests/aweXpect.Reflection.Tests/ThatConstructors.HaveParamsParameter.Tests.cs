using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have a params parameter,
					             but it contained constructors without a params parameter *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(AnotherClassWithParamsParameter).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have a params parameter,
					             but it only contained constructors with a params parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsParameter).GetConstructors().Single(),
					typeof(ClassWithoutModifiers).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithParamsParameter
		{
			public ClassWithParamsParameter(params int[] values)
			{
			}
		}

		private class AnotherClassWithParamsParameter
		{
			public AnotherClassWithParamsParameter(params string[] texts)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
