using Microsoft.CodeAnalysis;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     The diagnostics of <c>[CreateCollectionExpectation]</c> declarations that would otherwise fail silently.
/// </summary>
internal static class CollectionExpectationDiagnostics
{
	private const string Category = "aweXpect.SourceGenerators";

	/// <summary>
	///     Reported for a declaration that renders no overload at all.
	/// </summary>
	public static readonly DiagnosticDescriptor NothingGenerated = new(
		"aweXpect3001",
		"The collection expectation generates no overloads",
		"'{0}' generates no overloads, because {1}",
		Category,
		DiagnosticSeverity.Error,
		true);

	/// <summary>
	///     Reported for a negatable declaration whose negated overloads would get the name of the positive ones.
	/// </summary>
	public static readonly DiagnosticDescriptor SameNegatedName = new(
		"aweXpect3002",
		"The negated overloads of the collection expectation need a name of their own",
		"'{0}' takes a negation flag, but its negated overloads would get the same name; put {{Not}} into the name or set NegatedName",
		Category,
		DiagnosticSeverity.Error,
		true);

	/// <summary>
	///     Reported for a declaration whose overloads would carry an empty summary.
	/// </summary>
	public static readonly DiagnosticDescriptor MissingSummary = new(
		"aweXpect3003",
		"The collection expectation has no summary",
		"'{0}' has no {1}, so its overloads are undocumented",
		Category,
		DiagnosticSeverity.Warning,
		true);
}

/// <summary>
///     A diagnostic of a declaration, kept as data until the output step reports it.
/// </summary>
internal sealed record Problem(DiagnosticDescriptor Descriptor, Location Location, string Subject, string Reason = "")
{
	public Diagnostic ToDiagnostic() => Diagnostic.Create(Descriptor, Location, Subject, Reason);
}
