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
	public async Task SetDefaultEquivalencyOptions_ShouldAlsoApplyWhenOptionsArePassedPerCall()
	{
		ClassWithField actual = new(1, "foo");
		ClassWithField expected = new(2, "foo");

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected, o => o);

		using (IDisposable __ = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Set(new EquivalencyOptions
		       {
			       Fields = IncludeMembers.None,
		       }))
		{
			await That(Act).DoesNotThrow()
				.Because("the customized default must not be dropped when a callback creates the typed options");
		}
	}

	[Fact]
	public async Task SetMaxRecursionDepth_ShouldApplyOptionsWithinScope()
	{
		NestedNode actual = new(3);
		NestedNode expected = new(3);

		async Task Act()
			=> await That(actual).IsEquivalentTo(expected);

		await That(Act).DoesNotThrow();

		using (IDisposable __ = Customize.aweXpect.Equivalency().DefaultEquivalencyOptions.Set(new EquivalencyOptions
		       {
			       MaxRecursionDepth = 2,
		       }))
		{
			await That(Act).Throws()
				.WithMessage("""
				             Expected that actual
				             is equivalent to CustomizeEquivalencyTests.NestedNode {
				                 Inner = CustomizeEquivalencyTests.NestedNode {
				                   Inner = CustomizeEquivalencyTests.NestedNode {
				                     Inner = <null>
				                   }
				                 }
				               },
				             but it was not:
				               Property Inner.Inner exceeded the maximum recursion depth of 2

				             Equivalency options:
				              - include public fields and properties
				              - limit the recursion depth to 2
				             """);
		}
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

	private sealed class ClassWithField(int value, string name)
	{
		public readonly int Value = value;

		public string Name { get; } = name;
	}

	/// <remarks>
	///     Builds a chain of <paramref name="depth" /> nodes, so a comparison recurses exactly that many levels.
	/// </remarks>
	private sealed class NestedNode(int depth)
	{
		public NestedNode? Inner { get; } = depth > 1 ? new NestedNode(depth - 1) : null;
	}
}
