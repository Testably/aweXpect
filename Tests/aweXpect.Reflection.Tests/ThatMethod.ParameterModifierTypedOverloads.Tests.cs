using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class ParameterModifierTypedOverloads
	{
		public sealed class Tests
		{
			[Fact]
			public async Task HasRefParameterOfType_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterOfType_WhenWrongType_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<string>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameterOfType_WhenParameterIsNotRef_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.PlainInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasRefParameterOfTypeAndName_WhenWrongName_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<int>("other");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methodInfo
					             has parameter of type int with name "other" with ref modifier,
					             but it did not
					             """);
			}

			[Fact]
			public async Task HasRefParameterOfType_WhenParameterIsOut_ShouldFail()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<int>();
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task HasRefParameterOfTypeAndName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter<int>("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasRefParameterByName_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.RefInt));

				async Task Act()
				{
					await That(methodInfo).HasRefParameter("value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOutParameterOfType_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OutInt));

				async Task Act()
				{
					await That(methodInfo).HasOutParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasOptionalParameterOfType_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.OptionalInt));

				async Task Act()
				{
					await That(methodInfo).HasOptionalParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasParamsParameterOfType_WhenMatching_ShouldSucceed()
			{
				MethodInfo methodInfo = Method(nameof(ModifierMethods.ParamsInt));

				async Task Act()
				{
					await That(methodInfo).HasParamsParameter<int[]>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		private static MethodInfo Method(string name)
			=> typeof(ModifierMethods).GetMethod(name)!;

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ModifierMethods
		{
			public void RefInt(ref int value)
			{
				value = 0;
			}

			public void OutInt(out int value)
			{
				value = 0;
			}

			public void OptionalInt(int value = 0)
			{
			}

			public void ParamsInt(params int[] values)
			{
			}

			public void PlainInt(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
