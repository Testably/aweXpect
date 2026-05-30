using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasParamsParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasNoParamsParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a params parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a params parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasParamsParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have a params parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasNoParamsParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasParamsParameter());
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
