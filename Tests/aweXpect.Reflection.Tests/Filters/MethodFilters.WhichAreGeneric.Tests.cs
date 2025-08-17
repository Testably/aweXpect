using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyGenericMethods()
			{
				Filtered.Methods subject = In.AssemblyContaining<ThatMethods>().Types()
					.Which(t => t == typeof(ThatMethods.TestClass))
					.Methods()
					.WhichAreGeneric();

				await That(subject).All().Satisfy(m => m.IsGenericMethod).And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Methods subject = In.AssemblyContaining<ThatMethods>().Types()
					.Methods()
					.WhichAreGeneric();

				await That(subject.GetDescription()).Contains("generic methods");
			}
		}
	}
}
