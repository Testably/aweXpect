using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
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
					Filtered.Methods subject = In.AssemblyContaining<WhichAreGeneric>().Types()
						.Methods()
						.WhichAreGeneric()
						.WithArgumentCount(1);

					await That(subject).All().Satisfy(m => m.GetGenericArguments().Length == 1).And.IsNotEmpty();
					await That(subject.GetDescription()).Contains("generic methods with 1 generic argument in");
				}

				[Fact]
				public async Task ForValueLargerThanOneAsArgumentCount_ShouldFilterMethodsByGenericArgumentCount()
				{
					Filtered.Methods subject = In.AssemblyContaining<WhichAreGeneric>().Types()
						.Methods()
						.WhichAreGeneric()
						.WithArgumentCount(2);

					await That(subject).All().Satisfy(m => m.GetGenericArguments().Length == 2).And.IsNotEmpty();
					await That(subject.GetDescription()).Contains("generic methods with 2 generic arguments in");
				}
			}
		}
	}
}
