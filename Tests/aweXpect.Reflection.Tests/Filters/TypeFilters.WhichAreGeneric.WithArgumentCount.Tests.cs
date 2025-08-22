namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed partial class WhichAreGeneric
	{
		public sealed class WithArgumentCount
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ForOneAsArgumentCount_ShouldFilterMethodsByGenericArgumentCount()
				{
					Reflection.TypeFilters.GenericTypes subject = In.AssemblyContaining<WhichAreGeneric>().Types()
						.WhichAreGeneric()
						.WithArgumentCount(1);

					await That(subject).All().Satisfy(m => m.GetGenericArguments().Length == 1).And.IsNotEmpty();
					await That(subject.GetDescription()).Contains("generic types with 1 generic argument in");
				}

				[Fact]
				public async Task ForValueLargerThanOneAsArgumentCount_ShouldFilterMethodsByGenericArgumentCount()
				{
					Reflection.TypeFilters.GenericTypes subject = In.AssemblyContaining<WhichAreGeneric>().Types()
						.WhichAreGeneric()
						.WithArgumentCount(2);

					await That(subject).All().Satisfy(m => m.GetGenericArguments().Length == 2).And.IsNotEmpty();
					await That(subject.GetDescription()).Contains("generic types with 2 generic arguments in");
				}
			}
		}
	}
}
