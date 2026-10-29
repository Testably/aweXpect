using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The expectation that a method or constructor has a parameter that matches the options, shared by the plural
///     expectations on methods and constructors.
/// </summary>
internal sealed class HaveParameterExpectation<TMethod> : ItemExpectation<TMethod>
	where TMethod : MethodBase
{
	private readonly bool _exactType;
	private readonly string? _expectedName;
	private readonly string? _kind;
	private readonly Type? _parameterType;
	private StringEqualityOptions? _evaluationNameEquality;

	private HaveParameterExpectation(Type? parameterType, string? expectedName, bool exactType, string? kind,
		Func<HaveParameterExpectation<TMethod>, ParameterFilterOptions> parameterFilter)
	{
		_parameterType = parameterType;
		_expectedName = expectedName;
		_exactType = exactType;
		_kind = kind;
		ParameterFilter = new OptionsHolder<ParameterFilterOptions>(parameterFilter(this));
	}

	/// <summary>
	///     The options of <c>AtIndex(…)</c> of the result.
	/// </summary>
	public OptionsHolder<CollectionIndexOptions> CollectionIndex { get; } = new(CollectionIndexOptions.Default);

	/// <summary>
	///     The options of the parameter filters of the result, e.g. <c>WithDefaultValue()</c>.
	/// </summary>
	public OptionsHolder<ParameterFilterOptions> ParameterFilter { get; }

	/// <summary>
	///     The options of the name comparison of the result, e.g. <c>IgnoringCase()</c>.
	/// </summary>
	public OptionsHolder<StringEqualityOptions> NameEquality { get; } = new(StringEqualityOptions.Default);

	private StringEqualityOptions EvaluationNameEquality => _evaluationNameEquality ?? NameEquality.Value;

	/// <summary>
	///     Expects a parameter of the <paramref name="parameterType" />, or of a subtype unless
	///     <paramref name="exactType" /> is set.
	/// </summary>
	public static HaveParameterExpectation<TMethod> OfType(Type parameterType, bool exactType = false)
		=> new(parameterType, null, exactType, null,
			_ => new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, exactType)));

	/// <summary>
	///     Expects a parameter with the <paramref name="expected" /> name, and of the <paramref name="parameterType" />
	///     when it is not <see langword="null" />.
	/// </summary>
	/// <param name="parameterType">The expected parameter type, or <see langword="null" /> for any type.</param>
	/// <param name="expected">The expected parameter name.</param>
	/// <param name="exactType">Whether the parameter type must match exactly.</param>
	/// <param name="parameterName">The name of the caller's parameter that received <paramref name="expected" />.</param>
	public static HaveParameterExpectation<TMethod> Named(Type? parameterType, string expected,
		bool exactType = false, string parameterName = "expected")
		=> new(parameterType, expected, exactType, null, expectation =>
		{
			ParameterFilterOptions options = new(p
				=> expectation.EvaluationNameEquality.AreConsideredEqual(p.Name, expected, parameterName));
			return parameterType is null
				? options
				: options.WithPredicate(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, exactType));
		});

	/// <summary>
	///     Expects a parameter of the <paramref name="kind" />, e.g. "an in", that matches the
	///     <paramref name="predicate" />.
	/// </summary>
	public static HaveParameterExpectation<TMethod> OfKind(Func<ParameterInfo, bool> predicate, string kind)
		=> new(null, null, false, kind, _ => new ParameterFilterOptions(predicate));

	public override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
		=> _evaluationNameEquality = NameEquality.Value.ForEvaluation(context, cancellationToken);

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (_expectedName is not null)
		{
			EvaluationNameEquality.AppendContexts(contexts);
		}
	}

	public override async ValueTask<PredicateResult> IsMatch(TMethod item, CancellationToken cancellationToken)
		=> await item.HasMatchingParameter(CollectionIndex.Value, ParameterFilter.Value);

	public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
	{
		stringBuilder.Append(grammars.IsPlural() ? "have " : "has ");
		if (_kind is not null)
		{
			stringBuilder.Append(_kind).Append(' ');
		}

		stringBuilder.Append("parameter");
		if (_parameterType is not null)
		{
			stringBuilder.Append(_exactType ? " of exact type " : " of type ").Append(Formatter.Format(_parameterType));
		}

		if (_expectedName is not null)
		{
			stringBuilder.Append(" with name ").Append(Formatter.Format(_expectedName));
		}

		stringBuilder.Append(ParameterFilter.Value.GetModifierDescription());
		stringBuilder.Append(CollectionIndex.Value.Match.GetDescription());
	}
}

/// <summary>
///     The expectation that a method or constructor has the <paramref name="expected" /> number of parameters, shared
///     by the plural expectations on methods and constructors.
/// </summary>
internal sealed class HaveParameterCountExpectation<TMethod>(int expected) : ItemExpectation<TMethod>
	where TMethod : MethodBase
{
	public override ValueTask<PredicateResult> IsMatch(TMethod item, CancellationToken cancellationToken)
		=> new(item.GetParameters().Length == expected);

	public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		=> stringBuilder.Append(grammars.IsPlural() ? "have " : "has ").Append(expected switch
		{
			0 => "no parameters",
			1 => "one parameter",
			_ => $"{expected} parameters",
		});
}
