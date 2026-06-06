using System.IO;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class ParameterModifierExactlyOverloads
	{
		private static ConstructorInfo? Constructor<T>()
			=> typeof(T).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
			public async Task WithInParameterExactly_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly<int>();

				await That(constructors).Contains(Constructor<InIntCtor>());
				await That(constructors).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithInParameterExactly_UsingType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly(typeof(int));

				await That(constructors).Contains(Constructor<InIntCtor>());
				await That(constructors).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithInParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly<int>("value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly<int>("wrong");

				await That(matching).Contains(Constructor<InIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<InIntCtor>());
			}

			[Fact]
			public async Task WithInParameterExactlyAndName_UsingType_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly(typeof(int), "value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly(typeof(int), "wrong");

				await That(matching).Contains(Constructor<InIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<InIntCtor>());
			}

			[Fact]
			public async Task WithOptionalParameterExactly_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly<int>();

				await That(constructors).Contains(Constructor<OptionalIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
			}

			[Fact]
			public async Task WithOptionalParameterExactly_UsingType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly(typeof(int));

				await That(constructors).Contains(Constructor<OptionalIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
			}

			[Fact]
			public async Task WithOptionalParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly<int>("value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly<int>("wrong");

				await That(matching).Contains(Constructor<OptionalIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<OptionalIntCtor>());
			}

			[Fact]
			public async Task WithOptionalParameterExactlyAndName_UsingType_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly(typeof(int), "value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly(typeof(int), "wrong");

				await That(matching).Contains(Constructor<OptionalIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<OptionalIntCtor>());
			}

			[Fact]
			public async Task WithOutParameterExactly_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly<int>();

				await That(constructors).Contains(Constructor<OutIntCtor>());
				await That(constructors).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithOutParameterExactly_UsingType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly(typeof(int));

				await That(constructors).Contains(Constructor<OutIntCtor>());
				await That(constructors).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithOutParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly<int>("value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly<int>("wrong");

				await That(matching).Contains(Constructor<OutIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<OutIntCtor>());
			}

			[Fact]
			public async Task WithOutParameterExactlyAndName_UsingType_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly(typeof(int), "value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly(typeof(int), "wrong");

				await That(matching).Contains(Constructor<OutIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<OutIntCtor>());
			}

			[Fact]
			public async Task WithParamsParameterExactly_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly<int[]>();

				await That(constructors).Contains(Constructor<ParamsIntCtor>());
			}

			[Fact]
			public async Task WithParamsParameterExactly_UsingType_ShouldFilter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly(typeof(int[]));

				await That(constructors).Contains(Constructor<ParamsIntCtor>());
			}

			[Fact]
			public async Task WithParamsParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly<int[]>("values");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly<int[]>("wrong");

				await That(matching).Contains(Constructor<ParamsIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<ParamsIntCtor>());
			}

			[Fact]
			public async Task WithParamsParameterExactlyAndName_UsingType_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly(typeof(int[]), "values");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly(typeof(int[]), "wrong");

				await That(matching).Contains(Constructor<ParamsIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<ParamsIntCtor>());
			}

			[Fact]
			public async Task WithRefParameter_OfBaseType_ShouldMatchDerivedParameterType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<Stream>();

				await That(constructors).Contains(Constructor<RefMemoryStreamCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactly_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<Stream>();

				await That(constructors).DoesNotContain(Constructor<RefMemoryStreamCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactly_OfExactType_ShouldMatchParameterType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<MemoryStream>();

				await That(constructors).Contains(Constructor<RefMemoryStreamCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactly_ShouldFilterAndDescribe()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<int>();

				await That(constructors).Contains(Constructor<RefIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
				await That(constructors).DoesNotContain(Constructor<OutIntCtor>());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of exact type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterExactly_UsingType_OfBaseType_ShouldNotMatchDerivedParameterType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(Stream));

				await That(constructors).DoesNotContain(Constructor<RefMemoryStreamCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactly_UsingType_OfExactType_ShouldMatchParameterType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(MemoryStream));

				await That(constructors).Contains(Constructor<RefMemoryStreamCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactly_UsingType_ShouldFilterAndDescribe()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(int));

				await That(constructors).Contains(Constructor<RefIntCtor>());
				await That(constructors).DoesNotContain(Constructor<PlainIntCtor>());
				await That(constructors).DoesNotContain(Constructor<OutIntCtor>());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of exact type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithRefParameterExactlyAndName_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<int>("value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<int>("wrong");

				await That(matching).Contains(Constructor<RefIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<RefIntCtor>());
			}

			[Fact]
			public async Task WithRefParameterExactlyAndName_UsingType_ShouldFilterByName()
			{
				Filtered.Constructors matching = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(int), "value");
				Filtered.Constructors wrongName = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(int), "wrong");

				await That(matching).Contains(Constructor<RefIntCtor>());
				await That(wrongName).DoesNotContain(Constructor<RefIntCtor>());
			}
		}

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

		private class InIntCtor
		{
			public InIntCtor(in int value)
			{
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

		private class RefMemoryStreamCtor
		{
			public RefMemoryStreamCtor(ref MemoryStream value)
			{
				value = null!;
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
