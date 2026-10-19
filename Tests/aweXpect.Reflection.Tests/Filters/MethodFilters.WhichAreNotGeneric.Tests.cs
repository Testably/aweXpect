using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotGeneric
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonGenericMethods()
			{
				Filtered.Methods subject = In.AssemblyContaining<ThatMethods>().Types()
					.Which(t => t == typeof(ThatMethods.TestClass))
					.Methods()
					.WhichAreNotGeneric();

				await That(subject).All().Satisfy(m => !m.IsGenericMethod).And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Methods subject = In.AssemblyContaining<ThatMethods>().Types()
					.Methods()
					.WhichAreNotGeneric();

				await That(subject.GetDescription()).Contains("non-generic methods");
			}
		}
	}
}
