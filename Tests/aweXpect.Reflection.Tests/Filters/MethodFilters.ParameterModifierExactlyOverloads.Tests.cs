using System.IO;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static MethodInfo? Method(string name)
			=> typeof(ModifierMethods).GetMethod(name);

		public sealed class Tests
		{
			[Fact]
			public async Task WithInParameterExactly_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.InInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithInParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Methods matching = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly<int>("value");
				Filtered.Methods wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly<int>("wrong");

				await That(matching).Contains(Method(nameof(ModifierMethods.InInt)));
				await That(wrongName).DoesNotContain(Method(nameof(ModifierMethods.InInt)));
			}

			[Fact]
			public async Task WithOptionalParameterExactly_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.OptionalInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
			}

			[Fact]
			public async Task WithOptionalParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Methods matching = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly<int>("value");
				Filtered.Methods wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly<int>("wrong");

				await That(matching).Contains(Method(nameof(ModifierMethods.OptionalInt)));
				await That(wrongName).DoesNotContain(Method(nameof(ModifierMethods.OptionalInt)));
			}

			[Fact]
			public async Task WithOutParameterExactly_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.OutInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithOutParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Methods matching = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly<int>("value");
				Filtered.Methods wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly<int>("wrong");

				await That(matching).Contains(Method(nameof(ModifierMethods.OutInt)));
				await That(wrongName).DoesNotContain(Method(nameof(ModifierMethods.OutInt)));
			}

			[Fact]
			public async Task WithParamsParameterExactly_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly<int[]>();

				await That(methods).Contains(Method(nameof(ModifierMethods.ParamsInt)));
			}

			[Fact]
			public async Task WithParamsParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Methods matching = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly<int[]>("values");
				Filtered.Methods wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly<int[]>("wrong");

				await That(matching).Contains(Method(nameof(ModifierMethods.ParamsInt)));
				await That(wrongName).DoesNotContain(Method(nameof(ModifierMethods.ParamsInt)));
			}

			[Fact]
			public async Task WithRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				// Contrast: the non-exact overload matches assignable (derived) parameter types.
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<Stream>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefMemoryStream)));
			}

			[Fact]
			public async Task WithRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<Stream>();

				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefMemoryStream)));
			}

			[Fact]
			public async Task WithRefParameterExactly_OfExactType_ShouldMatchParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<MemoryStream>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefMemoryStream)));
			}

			[Fact]
			public async Task WithRefParameterExactly_ShouldFilterAndDescribe()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.OutInt)));
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Methods matching = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<int>("value");
				Filtered.Methods wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<int>("wrong");

				await That(matching).Contains(Method(nameof(ModifierMethods.RefInt)));
				await That(wrongName).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ModifierMethods
		{
			public void RefInt(ref int value) => value = 0;

			public void OutInt(out int value) => value = 0;

			public void InInt(in int value)
			{
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

			public void RefMemoryStream(ref MemoryStream value) => value = null!;
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
