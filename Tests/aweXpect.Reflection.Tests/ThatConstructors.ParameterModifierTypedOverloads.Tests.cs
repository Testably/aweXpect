using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class ParameterModifierTypedOverloads
	{
		private static ConstructorInfo Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
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

			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>();
			}

			[Fact]
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
