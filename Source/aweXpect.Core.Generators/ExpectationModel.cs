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
	string MethodAccessibility,
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
	int CancellationTokenIndex,
	bool ReturnsPredicateResult,
	EquatableArray<ParameterModel> Parameters,
	ResultModel? Result,
	EquatableArray<OverloadModel> Overloads)
{
	public bool PassesCancellationToken => CancellationTokenIndex >= 0;
}

/// <summary>
///     The collection expectation that a method marked with <c>GenerateCollectionExpectationAttribute</c> yields.
/// </summary>
/// <param name="Class">The class that receives the extension methods.</param>
/// <param name="Name">The name of the expectation.</param>
/// <param name="Text">The expectation on a single item.</param>
/// <param name="PluralText">The plural of the expectation on a single item.</param>
/// <param name="Verb">The verb in the past tense in the result, e.g. "were" in "but only 1 of 3 were".</param>
/// <param name="Summary">The XML documentation of the summary, if given.</param>
/// <param name="Remarks">The XML documentation of the remarks, if given.</param>
/// <param name="PredicateKind">How the predicate returns its result.</param>
/// <param name="CancellationTokenIndex">The position of the evaluation cancellation token among the arguments after the item, or <c>-1</c> without one.</param>
/// <param name="ReturnsPredicateResult">Whether the predicate returns a <c>PredicateResult</c> instead of a <see langword="bool" />.</param>
/// <param name="Items">The item types whose collections are extended.</param>
/// <param name="NullItemFailsBothWays">Whether a <see langword="null" /> item that does not reach the predicate fails the expectation and its negation, instead of not matching.</param>
/// <param name="CollectionOverload">Whether the expectation is also generated as an overload directly on the collection, for all items.</param>
/// <param name="NegatedName">The name of the expectation directly on the collection for no item, if any.</param>
/// <param name="FormattedItem">The method that returns the value which stands for an item in the failure message, if any.</param>
/// <param name="ItemTypeParameter">The type parameter of the method that is the type of the item, if any.</param>
/// <param name="ItemTypeConstraints">The constraints of the <paramref name="ItemTypeParameter" />.</param>
/// <param name="SupportsAsyncEnumerables">Whether the referenced aweXpect.Core supports <c>IAsyncEnumerable</c>.</param>
/// <param name="Options">The options that the predicate receives and the result exposes, if any.</param>
internal sealed record CollectionExpectationModel(
	ClassModel Class,
	string Name,
	EquatableArray<TextPart> Text,
	EquatableArray<TextPart> PluralText,
	string Verb,
	string? Summary,
	string? Remarks,
	PredicateKind PredicateKind,
	int CancellationTokenIndex,
	bool ReturnsPredicateResult,
	EquatableArray<ItemModel> Items,
	bool NullItemFailsBothWays,
	bool CollectionOverload,
	string? NegatedName,
	string? FormattedItem,
	string? ItemTypeParameter,
	string? ItemTypeConstraints,
	bool SupportsAsyncEnumerables,
	OptionsModel? Options = null)
{
	public bool PassesCancellationToken => CancellationTokenIndex >= 0;

	/// <summary>
	///     Whether an expectation is generated directly on the collection, whose constraints are told so.
	/// </summary>
	public bool HasCollectionExpectations => CollectionOverload || NegatedName is not null;
}

/// <summary>
///     The parts of a generated file that only depend on the class and the signature of the marked method.
/// </summary>
internal sealed record ClassModel(
	EquatableArray<string> Usings,
	EquatableArray<string> NamespaceUsings,
	string? Namespace,
	string ClassName,
	string ClassAccessibility,
	string MethodAccessibility,
	string HintName,
	string PredicateName,
	EquatableArray<ParameterModel> Parameters);

/// <summary>
///     How the marked method returns its result.
/// </summary>
internal enum PredicateKind
{
	Synchronous,
	Task,
	ValueTask,
}

/// <summary>
///     One item type whose collections the expectation extends, with its own constraints.
/// </summary>
/// <param name="ItemType">The type of the items.</param>
/// <param name="ConstraintPrefix">The prefix of the names of the nested constraint classes.</param>
/// <param name="FailsOnNull">Whether a <see langword="null" /> item does not match.</param>
/// <param name="ItemArgument">The expression that passes the <c>item</c> to the predicate.</param>
internal readonly record struct ItemModel(
	string ItemType,
	string ConstraintPrefix,
	bool FailsOnNull,
	string ItemArgument);

/// <summary>
///     The parameter of the predicate marked with the <c>ExpectationOptionsAttribute</c>: the generated expectation
///     holds the options in an <c>OptionsHolder</c> that the returned result changes, and passes the options of the
///     evaluation to the predicate.
/// </summary>
/// <param name="OptionsType">The type of the options, which has a static <c>Default</c> value.</param>
/// <param name="ResultType">The generic result type without its type arguments, e.g. <c>StringEqualityTypeResult</c>.</param>
/// <param name="ArgumentIndex">The position of the options among the arguments after the item.</param>
/// <param name="PatternParameterIndex">
///     The expectation parameter that the options compare with: the options validate it against its name, and write
///     its placeholder, e.g. <c>starts with "foo" ignoring case</c>.
/// </param>
internal readonly record struct OptionsModel(
	string OptionsType,
	string ResultType,
	int ArgumentIndex,
	int PatternParameterIndex);

/// <summary>
///     The method that describes the subject after "but", and which of the optional arguments it takes.
/// </summary>
internal readonly record struct ResultModel(string Name, bool TakesParameters, bool TakesItAndGrammars);

/// <summary>
///     A part of an expectation text: either literal text or the parameter at <see cref="ParameterIndex" />.
/// </summary>
internal readonly record struct TextPart(string? Text, int ParameterIndex);

/// <summary>
///     A parameter of the expectation, after the subject.
/// </summary>
/// <param name="Name">The name of the parameter.</param>
/// <param name="NegatedName">The name of the parameter in the negated expectation.</param>
/// <param name="Type">The type of the parameter.</param>
/// <param name="IsParams">Whether the parameter is a params array.</param>
/// <param name="RequiresNotNull">Whether a <see langword="null" /> argument throws.</param>
internal readonly record struct ParameterModel(
	string Name,
	string NegatedName,
	string Type,
	bool IsParams,
	bool RequiresNotNull)
{
	public string GetName(bool negated) => negated ? NegatedName : Name;
}

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
///     The outcome of inspecting a method marked for a collection expectation: the expectation to generate, or the
///     problem that prevents it.
/// </summary>
internal sealed record CollectionExpectationResult(CollectionExpectationModel? Model, Problem? Problem);

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
