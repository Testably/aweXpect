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
	private readonly Action<StringBuilder> _appendMemberText;
	private readonly string? _member;

	/// <summary>
	///     The positive expectation text of the member, which a negated result keeps, as only the left part renders the
	///     negation.
	/// </summary>
	private string? _negatedRightExpectation;

	public MappingResult(ConstraintResult left, ConstraintResult right, Action<StringBuilder> appendMemberText,
		string? member)
		: base(left, right, true, FurtherProcessingStrategy.Continue)
	{
		_appendMemberText = appendMemberText;
		_member = member;
		Outcome = CombineOutcomes();
	}

	/// <inheritdoc />
	/// <remarks>
	///     Under negation both parts were met, so the left part explains the failure, unless the member could not be
	///     answered. The member is not considered after a left part that stops the further processing.
	/// </remarks>
	protected override (bool Left, bool Right) GetExplainingParts()
	{
		bool explainsLeft = Left.ExplainsOutcomeOf(this);
		bool explainsRight = Right.ExplainsOutcomeOf(this) &&
		                     (!IsNegated || Right.Outcome == Outcome.FailureBothWays || !explainsLeft) &&
		                     (!explainsLeft || Left.FurtherProcessingStrategy == FurtherProcessingStrategy.Continue);
		return (explainsLeft, explainsRight);
	}

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		Left.AppendExpectation(stringBuilder);
		if (_negatedRightExpectation is not null)
		{
			stringBuilder.Append(_negatedRightExpectation);
			return;
		}

		AppendRightExpectation(stringBuilder);
	}

	private void AppendRightExpectation(StringBuilder stringBuilder)
	{
		StringBuilder separator = new();
		_appendMemberText(separator);
		stringBuilder.AppendSeparatedExpectation(separator.ToString(), Right);
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
		=> contexts.VisitOptionalMember(_member, Right);

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		IsNegated = !IsNegated;
		Left.Negate();
		_negatedRightExpectation = IsNegated ? GetRightExpectation() : null;
		Right.Negate();
		Outcome = CombineOutcomes();
		return this;
	}

	private string GetRightExpectation()
	{
		StringBuilder stringBuilder = new();
		AppendRightExpectation(stringBuilder);
		return stringBuilder.ToString();
	}
}
