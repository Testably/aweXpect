using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class ParameterModifierTypedOverloads
	{
		private static ConstructorInfo Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		public sealed class Tests
		{
			[Test]
			public async Task HaveParamsParameterOfType_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<ParamsIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveParamsParameter<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameterOfType_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(), Constructor<AnotherRefIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task HaveRefParameterOfType_WhenNotAllMatch_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(), Constructor<PlainIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of type int with ref modifier,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.ParameterModifierTypedOverloads.PlainIntCtor(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.ParameterModifierTypedOverloads.RefIntCtor(ref int value),
					               ThatConstructors.ParameterModifierTypedOverloads.PlainIntCtor(int value)
					             ]
					             """);
			}

			[Test]
			public async Task HaveRefParameterOfTypeAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					Constructor<RefIntCtor>(), Constructor<AnotherRefIntCtor>(),
				};

				async Task Act()
				{
					await That(constructors).HaveRefParameter<int>("value");
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class RefIntCtor
		{
			public RefIntCtor(ref int value)
			{
				value = 0;
			}
		}

		private class AnotherRefIntCtor
		{
			public AnotherRefIntCtor(ref int value)
			{
				value = 0;
			}
		}

		private class ParamsIntCtor
		{
			public ParamsIntCtor(params int[] values)
			{
			}
		}

		private class PlainIntCtor
		{
			public PlainIntCtor(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
	}
}
