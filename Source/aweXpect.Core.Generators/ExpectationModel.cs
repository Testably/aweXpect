using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.Core.Generators;

/// <summary>
///     The expectation that a method marked with <c>GenerateExpectationAttribute</c> yields.
/// </summary>
/// <remarks>
///     The usings of the file that declares the method are repeated, so that the documentation from the attribute
///     resolves its references like in that file.
/// </remarks>
internal sealed record ExpectationModel(
	EquatableArray<string> Usings,
	EquatableArray<string> NamespaceUsings,
	string? Namespace,
	string ClassName,
	string ClassAccessibility,
	string HintName,
	string PredicateName,
	string PositiveName,
	string? NegatedName,
	EquatableArray<TextPart> PositiveText,
	EquatableArray<TextPart> NegatedText,
	string? Summary,
	string? NegatedSummary,
	string? Remarks,
	string? NegatedRemarks,
	bool IsAsync,
	bool PassesCancellationToken,
	EquatableArray<ParameterModel> Parameters,
	string? ResultMethod,
	bool ResultTakesParameters,
	EquatableArray<OverloadModel> Overloads);

/// <summary>
///     A part of an expectation text: either literal text or the parameter at <see cref="ParameterIndex" />.
/// </summary>
internal readonly record struct TextPart(string? Text, int ParameterIndex);

/// <summary>
///     A parameter of the expectation, after the subject.
/// </summary>
internal readonly record struct ParameterModel(string Name, string Type, bool IsParams, bool RequiresNotNull);

/// <summary>
///     One extension method pair on <see cref="ThatType" />, with its own constraint.
/// </summary>
/// <param name="ThatType">The type argument of the extended <c>IThat&lt;T&gt;</c>.</param>
/// <param name="ConstraintName">The name of the nested constraint class.</param>
/// <param name="FailsOnNull">Whether a <see langword="null" /> subject fails the expectation and its negation.</param>
/// <param name="PositiveResultType">The value that the expectation continues with.</param>
/// <param name="NegatedResultType">The value that the negated expectation continues with.</param>
/// <param name="PositiveGuaranteesNotNull">Whether the expectation guarantees a subject that is not <see langword="null" />.</param>
/// <param name="NegatedGuaranteesNotNull">Whether the negation guarantees a subject that is not <see langword="null" />.</param>
/// <param name="SubjectArgument">The expression that passes the <c>actual</c> value to the predicate.</param>
/// <param name="ResultArgument">The expression that passes the <c>Actual</c> value to the result method.</param>
internal readonly record struct OverloadModel(
	string ThatType,
	string ConstraintName,
	bool FailsOnNull,
	string PositiveResultType,
	string NegatedResultType,
	bool PositiveGuaranteesNotNull,
	bool NegatedGuaranteesNotNull,
	string SubjectArgument,
	string ResultArgument);

/// <summary>
///     The outcome of inspecting a marked method: the expectation to generate, or the problem that prevents it.
/// </summary>
internal sealed record ExpectationResult(ExpectationModel? Model, Problem? Problem);

/// <summary>
///     A diagnostic to report.
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
