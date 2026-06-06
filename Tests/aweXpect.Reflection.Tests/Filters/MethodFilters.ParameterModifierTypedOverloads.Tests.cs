using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class ParameterModifierTypedOverloads
	{
		private static MethodInfo? Method(string name)
			=> typeof(ModifierMethods).GetMethod(name);

		public sealed class Tests
		{
			[Fact]
			public async Task WithInParameterOfType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.InInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithInParameterOfType_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter(typeof(int));

				await That(methods).Contains(Method(nameof(ModifierMethods.InInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithInParameterOfTypeAndName_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter(typeof(int), "value");

				await That(methods).Contains(Method(nameof(ModifierMethods.InInt)));
			}

			[Fact]
			public async Task WithOptionalParameterOfType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.OptionalInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
			}

			[Fact]
			public async Task WithOptionalParameterOfType_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter(typeof(int));

				await That(methods).Contains(Method(nameof(ModifierMethods.OptionalInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
			}

			[Fact]
			public async Task WithOptionalParameterOfTypeAndName_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter(typeof(int), "value");

				await That(methods).Contains(Method(nameof(ModifierMethods.OptionalInt)));
			}

			[Fact]
			public async Task WithOutParameterOfType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.OutInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithOutParameterOfType_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter(typeof(int));

				await That(methods).Contains(Method(nameof(ModifierMethods.OutInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithOutParameterOfTypeAndName_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter(typeof(int), "value");

				await That(methods).Contains(Method(nameof(ModifierMethods.OutInt)));
			}

			[Fact]
			public async Task WithParamsParameterOfType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter<int[]>();

				await That(methods).Contains(Method(nameof(ModifierMethods.ParamsInt)));
			}

			[Fact]
			public async Task WithParamsParameterOfType_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter(typeof(int[]));

				await That(methods).Contains(Method(nameof(ModifierMethods.ParamsInt)));
			}

			[Fact]
			public async Task WithParamsParameterOfTypeAndName_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter(typeof(int[]), "values");

				await That(methods).Contains(Method(nameof(ModifierMethods.ParamsInt)));
			}

			[Fact]
			public async Task WithRefParameterByName_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter("value");

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithRefParameterOfBaseType_ShouldAlsoMatchDerivedParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<BaseType>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefBase)));
				await That(methods).Contains(Method(nameof(ModifierMethods.RefDerived)));
			}

			[Fact]
			public async Task WithRefParameterOfBaseType_UsingType_ShouldAlsoMatchDerivedParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(BaseType));

				await That(methods).Contains(Method(nameof(ModifierMethods.RefBase)));
				await That(methods).Contains(Method(nameof(ModifierMethods.RefDerived)));
			}

			[Fact]
			public async Task WithRefParameterOfDerivedType_ShouldNotMatchBaseParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<DerivedType>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefDerived)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefBase)));
			}

			[Fact]
			public async Task WithRefParameterOfDerivedType_UsingType_ShouldNotMatchBaseParameterType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(DerivedType));

				await That(methods).Contains(Method(nameof(ModifierMethods.RefDerived)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefBase)));
			}

			[Fact]
			public async Task WithRefParameterOfType_ShouldFilterAndDescribe()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<int>();

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.OutInt)));
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterOfType_ShouldNotMatchWrongType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<string>();

				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithRefParameterOfType_UsingType_ShouldFilterAndDescribe()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(int));

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.PlainInt)));
				await That(methods).DoesNotContain(Method(nameof(ModifierMethods.OutInt)));
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterOfTypeAndName_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<int>("value");

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
			}

			[Fact]
			public async Task WithRefParameterOfTypeAndName_UsingType_ShouldFilter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(int), "value");

				await That(methods).Contains(Method(nameof(ModifierMethods.RefInt)));
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

			public void RefBase(ref BaseType value) => value = null!;

			public void RefDerived(ref DerivedType value) => value = null!;
		}

		private class BaseType;

		private sealed class DerivedType : BaseType;
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
