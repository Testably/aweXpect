using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class HasOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorHasOptionalParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOptionalParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorHasNoOptionalParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an optional parameter,
					             but it did not
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? constructorInfo = null;

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has an optional parameter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorHasOptionalParameter_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithOptionalParameter).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have an optional parameter,
					             but it did
					             """);
			}

			[Fact]
			public async Task WhenConstructorHasNoOptionalParameter_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithoutModifiers).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotComplyWith(it => it.HasOptionalParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ClassWithOptionalParameter
		{
			public ClassWithOptionalParameter(int value = 0)
			{
			}
		}

		private class ClassWithoutModifiers
		{
			public ClassWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
