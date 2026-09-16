using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace aweXpect.Analyzers;

internal static class Rules
{
	private const string UsageCategory = "Usage";

	public static readonly DiagnosticDescriptor AwaitExpectationRule =
		CreateDescriptor("aweXpect0001", UsageCategory, DiagnosticSeverity.Error);

	public static readonly DiagnosticDescriptor EqualsRule =
		CreateDescriptor("aweXpect0002", UsageCategory, DiagnosticSeverity.Error);

	public static readonly DiagnosticDescriptor ThrownExceptionVocabularyRule =
		CreateDescriptor("aweXpect0003", UsageCategory, DiagnosticSeverity.Warning);

	/// <summary>
	///     The same rule as <see cref="ThrownExceptionVocabularyRule" /> for a <c>Has…</c> expectation without a
	///     <c>With…</c> twin, where the message can only suggest inserting <c>.Which</c>.
	/// </summary>
	public static readonly DiagnosticDescriptor ThrownExceptionVocabularyWithoutTwinRule =
		CreateDescriptor("aweXpect0003", UsageCategory, DiagnosticSeverity.Warning, "WithoutTwinMessageFormat");

	/// <summary>
	///     The nullability warnings that are suppressed after an expectation that guarantees a not-null subject.
	/// </summary>
	public static readonly ImmutableArray<SuppressionDescriptor> IsNotNullSuppressions =
	[
		CreateSuppression("aweXpect1001", "CS8600"),
		CreateSuppression("aweXpect1002", "CS8602"),
		CreateSuppression("aweXpect1003", "CS8604"),
		CreateSuppression("aweXpect1004", "CS8629"),
	];

	private static SuppressionDescriptor CreateSuppression(string suppressionId, string suppressedDiagnosticId) => new(
		suppressionId,
		suppressedDiagnosticId,
		new LocalizableResourceString("IsNotNullSuppressionJustification", Resources.ResourceManager,
			typeof(Resources))
	);

	private static DiagnosticDescriptor CreateDescriptor(string diagnosticId, string category,
		DiagnosticSeverity severity, string messageFormatSuffix = "MessageFormat") => new(
		diagnosticId,
		new LocalizableResourceString(diagnosticId + "Title",
			Resources.ResourceManager, typeof(Resources)),
		new LocalizableResourceString(diagnosticId + messageFormatSuffix, Resources.ResourceManager,
			typeof(Resources)),
		category,
		severity,
		true,
		new LocalizableResourceString(diagnosticId + "Description", Resources.ResourceManager,
			typeof(Resources))
	);
}
