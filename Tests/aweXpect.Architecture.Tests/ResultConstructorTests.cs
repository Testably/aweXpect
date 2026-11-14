namespace aweXpect.Architecture.Tests;

public sealed class ResultConstructorTests
{
	[Test]
	public async Task ResultsOfAweXpect_ShouldOnlyHaveInternalConstructors()
		=> await That(Packages.PublicTypes(Packages.AweXpect).Which(Packages.IsResult).Constructors())
			.AreNot(AccessModifiers.Public | AccessModifiers.Protected | AccessModifiers.ProtectedInternal)
			.Because("only the expectations of aweXpect can build its results, and every visible constructor is permanent API");
}
