using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class Matching
	{
		public sealed class Tests
		{
			[Test]
			public async Task HaveParameter_WhenAllParametersMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithCount).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParameter<int>().Matching(p => p.Name == "count");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveParamsParameter_WhenNotAllAreParamsCollections_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(ClassWithParamsArray).GetConstructors().Single(),
					typeof(ClassWithParamsCollection).GetConstructors().Single(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter().Matching(p => !p.ParameterType.IsArray);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have a params parameter matching p => !p.ParameterType.IsArray,
					             but it contained constructors without a params parameter

					             Not matching items:
					             [
					               ThatConstructors.Matching.ClassWithParamsArray(int[] values)
					             ]
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
