using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class Matching
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotHaveParameter_WhenParameterMatches_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithCount).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).DoesNotHaveParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             does not have parameter of type int matching p => p.Name == "count",
					             but it did
					             """);
			}

			[Test]
			public async Task HasParameter_WhenParameterMatches_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithCount).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasParamsParameter_WhenParamsParameterIsACollection_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsCollection).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HasParamsParameter_WhenParamsParameterIsAnArray_ShouldFail()
			{
				ConstructorInfo constructorInfo = typeof(ClassWithParamsArray).GetConstructors().Single();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructorInfo
					             has a params parameter matching p => !p.ParameterType.IsArray,
					             but it did not
					             """);
			}
		}

		// ReSharper disable UnusedParameter.Local
		private class ClassWithCount
		{
			public ClassWithCount(int count)
			{
			}
		}

		private class ClassWithParamsArray
		{
			public ClassWithParamsArray(params int[] values)
			{
			}
		}

		private class ClassWithParamsCollection
		{
			public ClassWithParamsCollection(params List<int> values)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
	}
}
