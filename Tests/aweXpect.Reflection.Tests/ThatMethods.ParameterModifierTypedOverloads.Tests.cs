using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ParameterModifierTypedOverloads
	{
		private static MethodInfo Method(string name)
			=> typeof(ModifierMethods).GetMethod(name)!;

		public sealed class Tests
		{
			[Fact]
			public async Task HaveOptionalParameterOfType_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.OptionalInt)),
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveParamsParameterOfType_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.ParamsInt)),
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter<int[]>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameterOfType_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)), Method(nameof(ModifierMethods.AnotherRefInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HaveRefParameterOfType_WhenNotAllMatch_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)), Method(nameof(ModifierMethods.PlainInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<int>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of type int with ref modifier,
					             but at least one did not
					             """);
			}

			[Fact]
			public async Task HaveRefParameterOfTypeAndName_WhenAllMatch_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					Method(nameof(ModifierMethods.RefInt)), Method(nameof(ModifierMethods.AnotherRefInt)),
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter<int>("value");
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class ModifierMethods
		{
			public void RefInt(ref int value) => value = 0;

			public void AnotherRefInt(ref int value) => value = 0;

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
