using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class ParameterModifierTypedOverloads
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WithRefParameterOfType_ShouldFilterAndDescribe()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<int>();

				await That(constructors).Contains(Constructor<RefIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
				await That(constructors).DoesNotContain(Constructor<OutIntCtor>());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterOfType_ShouldNotMatchWrongType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<string>();

				await That(constructors).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithRefParameterOfTypeAndName_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<int>("value");

				await That(constructors).Contains(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithRefParameterByName_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter("value");

				await That(constructors).Contains(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithOptionalParameterOfType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter<int>();

				await That(constructors).Contains(Constructor<OptionalIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
			}

			[Fact]
			public async Task WithParamsParameterOfType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter<int[]>();

				await That(constructors).Contains(Constructor<ParamsIntCtor>());
			}
		}

		private static ConstructorInfo? Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class RefIntCtor
		{
			public RefIntCtor(ref int value)
			{
				value = 0;
			}
		}

		private class OutIntCtor
		{
			public OutIntCtor(out int value)
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

		private class InIntCtor
		{
			public InIntCtor(in int value)
			{
			}
		}

		private class RefBaseCtor
		{
			public RefBaseCtor(ref BaseType value)
			{
				value = null!;
			}
		}

		private class RefDerivedCtor
		{
			public RefDerivedCtor(ref DerivedType value)
			{
				value = null!;
			}
		}

		private class BaseType;

		private sealed class DerivedType : BaseType;
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
