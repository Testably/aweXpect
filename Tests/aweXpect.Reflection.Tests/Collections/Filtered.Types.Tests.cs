namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed class Types
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Types_FromConstructors_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Constructors().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo(
						"declaring types of constructors in types in assembly containing type Filtered.Types.Tests");
			}

			[Fact]
			public async Task Types_FromEvents_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Events().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of events in types in assembly containing type Filtered.Types.Tests");
			}

			[Fact]
			public async Task Types_FromFields_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Fields().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of fields in types in assembly containing type Filtered.Types.Tests");
			}

			[Fact]
			public async Task Types_FromMethods_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Methods().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo("declaring types of methods in types in assembly containing type Filtered.Types.Tests");
			}

			[Fact]
			public async Task Types_FromProperties_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>()
					.Types().Properties().DeclaringTypes();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description)
					.IsEqualTo(
						"declaring types of properties in types in assembly containing type Filtered.Types.Tests");
			}

			[Fact]
			public async Task Types_InAssembly_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Types();

				string description = types.GetDescription();

				await That(types).IsNotEmpty();
				await That(description).IsEqualTo("types in assembly containing type Filtered.Types.Tests");
			}
		}
	}
}
