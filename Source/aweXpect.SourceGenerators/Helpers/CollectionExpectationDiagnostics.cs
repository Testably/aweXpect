using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.SourceGenerators.Helpers;

/// <summary>
///     The diagnostics of <c>[CreateExpectationFamily]</c> declarations that would otherwise fail silently.
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

	/// <summary>
	///     Reported for a declaration whose expected collection could not echo the caller's expression.
	/// </summary>
	public static readonly DiagnosticDescriptor MissingExpression = new(
		"aweXpect3004",
		"The expected collection of the collection expectation needs its caller argument expression",
		"'{0}' takes an expected collection, so the helper needs a string parameter for its caller argument expression",
		Category,
		DiagnosticSeverity.Error,
		true);
}

/// <summary>
///     A diagnostic of a declaration, kept as data until the output step reports it.
/// </summary>
/// <remarks>
///     A <see cref="Location" /> holds its syntax tree, which every edit replaces, so it would defeat the caching of the
///     pipeline; only its position is kept, and the location is rebuilt from it when reporting.
/// </remarks>
internal sealed record Problem
{
	private readonly string? _filePath;
	private readonly LinePositionSpan _lineSpan;
	private readonly TextSpan _span;

	public Problem(DiagnosticDescriptor descriptor, Location location, string subject, string reason = "")
	{
		Descriptor = descriptor;
		Subject = subject;
		Reason = reason;
		if (location.IsInSource)
		{
			_filePath = location.SourceTree!.FilePath;
			_span = location.SourceSpan;
			_lineSpan = location.GetLineSpan().Span;
		}
	}

	public DiagnosticDescriptor Descriptor { get; }
	public string Subject { get; }
	public string Reason { get; }

	public Diagnostic ToDiagnostic()
		=> Diagnostic.Create(Descriptor,
			_filePath is null ? Location.None : Location.Create(_filePath, _span, _lineSpan),
			Subject, Reason);
}
