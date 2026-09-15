using aweXpect.Customization;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeEquivalencyTests
{
	[Fact]
	public async Task SetDefaultEquivalencyDocumentOptions_ShouldApplyOptionsWithinScope()
	{
		int[] actual = [1, 2,];
		int[] expected = [2, 1,];

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		using (IDisposable __ = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Set(new EquivalencyOptions
		       {
			       IgnoreCollectionOrder = true,
		       }))
		{
			await That(Act).DoesNotThrow();
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that actual
			             is equivalent to [
			               2,
			               1
			             ],
			             but it was not:
			               Element [0] differed:
			                    Found: 1
			                 Expected: 2
			             and
			               Element [1] differed:
			                    Found: 2
			                 Expected: 1
			             
			             Equivalency options:
			              - include public fields and properties
			             """);
	}

	[Fact]
	public async Task ShouldChangeIndividualProperties()
	{
		await That(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get().IgnoreCollectionOrder)
			.IsFalse();

		using (Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Set(new EquivalencyOptions
		       {
			       IgnoreCollectionOrder = true,
		       }))
		{
			await That(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get().IgnoreCollectionOrder)
				.IsTrue();
		}

		await That(Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Get().IgnoreCollectionOrder)
			.IsFalse();
	}
}
