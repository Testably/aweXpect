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

	public static readonly DiagnosticDescriptor DelegateSubjectRule =
		CreateDescriptor("aweXpect0004", UsageCategory, DiagnosticSeverity.Error);

	public static readonly DiagnosticDescriptor AsyncVoidExpectationRule =
		CreateDescriptor("aweXpect0005", UsageCategory, DiagnosticSeverity.Warning);

	public static readonly DiagnosticDescriptor UnorderedCollectionRule =
		CreateDescriptor("aweXpect0006", UsageCategory, DiagnosticSeverity.Warning);

	/// <summary>
	///     The variant of <see cref="UnorderedCollectionRule" /> for an expectation that is about the order itself, so
	///     that <c>InAnyOrder()</c> cannot help.
	/// </summary>
	public static readonly DiagnosticDescriptor UnorderedCollectionNoMeaningRule =
		CreateDescriptor("aweXpect0006", UsageCategory, DiagnosticSeverity.Warning, "NoMeaningMessageFormat");

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
		DiagnosticSeverity severity, string messageFormatName = "MessageFormat") => new(
		diagnosticId,
		new LocalizableResourceString(diagnosticId + "Title",
			Resources.ResourceManager, typeof(Resources)),
		new LocalizableResourceString(diagnosticId + messageFormatName, Resources.ResourceManager,
			typeof(Resources)),
		category,
		severity,
		true,
		new LocalizableResourceString(diagnosticId + "Description", Resources.ResourceManager,
			typeof(Resources))
	);
}
