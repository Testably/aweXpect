using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class ParameterModifierTypedOverloads
	{
		public sealed class Tests
		{
			[Fact]
			public async Task HasRefParameterOfType_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterOfType_WhenWrongType_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<string>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameterOfType_WhenParameterIsNotRef_ShouldFail()
			{
				ConstructorInfo constructorInfo = Constructor<PlainIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<int>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameterOfTypeAndName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterByName_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<RefIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasRefParameter("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterOfType_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<OptionalIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasOptionalParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterOfType_WhenMatching_ShouldSucceed()
			{
				ConstructorInfo constructorInfo = Constructor<ParamsIntCtor>();

				async Task Act()
				{
					await That(constructorInfo).HasParamsParameter<int[]>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		private static ConstructorInfo Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class RefIntCtor
		{
			public RefIntCtor(ref int value)
			{
				value = 0;
			}
		}

		private class OptionalIntCtor
		{
			public OptionalIntCtor(int value = 0)
			{
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
