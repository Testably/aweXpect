using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace aweXpect.Analyzers;

/// <remarks>
///     The ID, category and severity are passed to each constructor as constants, because the release tracking
///     (RS2000) only recognises constant values when it checks the rules against <c>AnalyzerReleases.*.md</c>.
/// </remarks>
internal static class Rules
{
	private const string UsageCategory = "Usage";

	public static readonly DiagnosticDescriptor AwaitExpectationRule = new(
		"aweXpect0001", Title("aweXpect0001"), MessageFormat("aweXpect0001"), UsageCategory,
		DiagnosticSeverity.Error, true, Description("aweXpect0001"), HelpLinkUri("aweXpect0001"));

	public static readonly DiagnosticDescriptor EqualsRule = new(
		"aweXpect0002", Title("aweXpect0002"), MessageFormat("aweXpect0002"), UsageCategory,
		DiagnosticSeverity.Error, true, Description("aweXpect0002"), HelpLinkUri("aweXpect0002"));

	public static readonly DiagnosticDescriptor ThrownExceptionVocabularyRule = new(
		"aweXpect0003", Title("aweXpect0003"), MessageFormat("aweXpect0003"), UsageCategory,
		DiagnosticSeverity.Warning, true, Description("aweXpect0003"), HelpLinkUri("aweXpect0003"));

	public static readonly DiagnosticDescriptor DelegateSubjectRule = new(
		"aweXpect0004", Title("aweXpect0004"), MessageFormat("aweXpect0004"), UsageCategory,
		DiagnosticSeverity.Error, true, Description("aweXpect0004"), HelpLinkUri("aweXpect0004"));

	public static readonly DiagnosticDescriptor AsyncVoidExpectationRule = new(
		"aweXpect0005", Title("aweXpect0005"), MessageFormat("aweXpect0005"), UsageCategory,
		DiagnosticSeverity.Warning, true, Description("aweXpect0005"), HelpLinkUri("aweXpect0005"));

	public static readonly DiagnosticDescriptor UnorderedCollectionRule = new(
		"aweXpect0006", Title("aweXpect0006"), MessageFormat("aweXpect0006"), UsageCategory,
		DiagnosticSeverity.Warning, true, Description("aweXpect0006"), HelpLinkUri("aweXpect0006"));

	/// <summary>
	///     The variant of <see cref="UnorderedCollectionRule" /> for an expectation that is about the order itself, so
	///     that <c>InAnyOrder()</c> cannot help.
	/// </summary>
	public static readonly DiagnosticDescriptor UnorderedCollectionNoMeaningRule = new(
		"aweXpect0006", Title("aweXpect0006"), MessageFormat("aweXpect0006", "NoMeaningMessageFormat"), UsageCategory,
		DiagnosticSeverity.Warning, true, Description("aweXpect0006"), HelpLinkUri("aweXpect0006"));

	public static readonly DiagnosticDescriptor ValueTaskDelegateRule = new(
		"aweXpect0007", Title("aweXpect0007"), MessageFormat("aweXpect0007"), UsageCategory,
		DiagnosticSeverity.Error, true, Description("aweXpect0007"), HelpLinkUri("aweXpect0007"));

	/// <summary>
	///     The variant of <see cref="ValueTaskDelegateRule" /> for a <c>Task</c>, which only becomes the value of the
	///     delegate through an explicit type argument, so that <c>AsTask()</c> cannot help.
	/// </summary>
	public static readonly DiagnosticDescriptor ExplicitTaskDelegateRule = new(
		"aweXpect0007", Title("aweXpect0007"), MessageFormat("aweXpect0007", "ExplicitTaskMessageFormat"), UsageCategory,
		DiagnosticSeverity.Error, true, Description("aweXpect0007"), HelpLinkUri("aweXpect0007"));

	public static readonly DiagnosticDescriptor OrResultValueRule = new(
		"aweXpect0008", Title("aweXpect0008"), MessageFormat("aweXpect0008"), UsageCategory,
		DiagnosticSeverity.Warning, true, Description("aweXpect0008"), HelpLinkUri("aweXpect0008"));

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

	private static LocalizableResourceString Title(string diagnosticId)
		=> new(diagnosticId + "Title", Resources.ResourceManager, typeof(Resources));

	private static LocalizableResourceString MessageFormat(string diagnosticId,
		string messageFormatName = "MessageFormat")
		=> new(diagnosticId + messageFormatName, Resources.ResourceManager, typeof(Resources));

	private static LocalizableResourceString Description(string diagnosticId)
		=> new(diagnosticId + "Description", Resources.ResourceManager, typeof(Resources));

	private static string HelpLinkUri(string diagnosticId)
		=> "https://docs.testably.org/aweXpect/analyzers#" + diagnosticId.ToLowerInvariant();
}
