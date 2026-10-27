using System;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of an expectation on the subject combined with the expectations on one of its members.
/// </summary>
internal sealed class MappingResult : CombinedResult
{
	/// <summary>
	///     The formatters of the evaluation that created this result, which format the expectation text of the member
	///     also when the result is negated after that evaluation, e.g. by <c>DoesNotComplyWith</c>.
	/// </summary>
	private readonly IValueFormatter[]? _formatters = ValueFormatter.ScopedFormatters;

	private readonly MappingNode _mappingNode;

	/// <summary>
	///     The positive expectation text of the member, which a negated result keeps, as only the left part renders the
	///     negation.
	/// </summary>
	private string? _negatedRightExpectation;

	public MappingResult(ConstraintResult left, ConstraintResult right, MappingNode mappingNode)
		: base(left, right, true, FurtherProcessingStrategy.Continue)
	{
		_mappingNode = mappingNode;
		Outcome = CombineOutcomes();
	}

	/// <summary>
	///     The result of the expectation on the subject, which the members continue from.
	/// </summary>
	internal ConstraintResult Source => Left;

	/// <summary>
	///     Whether the negated expectations on the member are rendered as an alternative to the negated expectation on
	///     the subject, see <see cref="MappingNode.NegatedAlternativeTextGenerator" />.
	/// </summary>
	private bool IsNegatedAsAlternative => IsNegated && _mappingNode.NegatedAlternativeTextGenerator is not null;

	/// <inheritdoc />
	/// <remarks>
	///     Under negation both parts were met, so the left part explains the failure, unless the member could not be
	///     answered. The member is not considered after a left part that stops the further processing.
	/// </remarks>
	protected override (bool Left, bool Right) GetExplainingParts()
	{
		if (IsNegatedAsAlternative && Left.Outcome != Outcome.FailureBothWays && Right.ExplainsOutcomeOf(this))
		{
			return (false, true);
		}

		bool explainsLeft = Left.ExplainsOutcomeOf(this);
		bool explainsRight = Right.ExplainsOutcomeOf(this) &&
		                     (!IsNegated || Right.Outcome == Outcome.FailureBothWays || !explainsLeft) &&
		                     (!explainsLeft || Left.FurtherProcessingStrategy == FurtherProcessingStrategy.Continue);
		return (explainsLeft, explainsRight);
	}

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		Left.AppendExpectation(stringBuilder, indentation);
		if (IsNegatedAsAlternative)
		{
			StringBuilder separator = new();
			_mappingNode.NegatedAlternativeTextGenerator!(_mappingNode.MemberAccessor, separator);
			stringBuilder.AppendSeparatedExpectation(separator.ToString(), Right, indentation);
			return;
		}

		if (_negatedRightExpectation is not null)
		{
			stringBuilder.Append(_negatedRightExpectation);
			return;
		}

		AppendRightExpectation(stringBuilder, indentation);
	}

	private void AppendRightExpectation(StringBuilder stringBuilder, string? indentation)
	{
		StringBuilder separator = new();
		_mappingNode.AppendMemberText(separator);
		stringBuilder.AppendSeparatedExpectation(separator.ToString(), Right, indentation);
	}

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		(bool rendersLeft, bool explainsRight) = GetExplainingParts();
		// The result text of the member is omitted when it repeats the one of the left part.
		bool rendersRight = explainsRight && (!rendersLeft || !Left.HasSameResultTextAs(Right));
		int leftStart = stringBuilder.Length;
		if (rendersLeft)
		{
			Left.AppendResult(stringBuilder, indentation);
		}

		if (rendersRight)
		{
			if (rendersLeft)
			{
				stringBuilder.AppendAndSeparator(leftStart, indentation);
			}

			Right.AppendResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc />
	protected override void VisitRight(ResultContextCollector contexts)
		=> contexts.VisitOptionalMember(_mappingNode.ContextMember, Right, _mappingNode.MemberAccessor.PathRoot);

	/// <inheritdoc />
	protected internal override void NegateInPlace()
	{
		IsNegated = !IsNegated;
		Left.NegateInPlace();
		_negatedRightExpectation = IsNegated && !IsNegatedAsAlternative ? GetRightExpectation() : null;
		Right.NegateInPlace();
		Outcome = CombineOutcomes();
	}

	private string GetRightExpectation()
	{
		using ValueFormatter.FormatterScope _ = ValueFormatter.UseScopedFormatters(_formatters);
		StringBuilder stringBuilder = new();
		AppendRightExpectation(stringBuilder, null);
		return stringBuilder.ToString();
	}
}
