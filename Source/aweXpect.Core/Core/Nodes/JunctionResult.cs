using System;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of two expectations combined with <c>And</c> or <c>Or</c>.
/// </summary>
internal sealed class JunctionResult : CombinedResult
{
	private readonly bool _isAnd;
	private readonly string _separator;

	public JunctionResult(ConstraintResult left, ConstraintResult right, bool isAnd, string separator,
		FurtherProcessingStrategy furtherProcessingStrategy)
		: base(left, right, isAnd, furtherProcessingStrategy)
	{
		_isAnd = isAnd;
		_separator = separator;
		Outcome = CombineOutcomes();
	}

	private bool RendersLeft => Left.ExplainsOutcomeOf(this);

	/// <summary>
	///     The result text of the right operand is omitted when it repeats the one of the left operand.
	/// </summary>
	private bool RendersRight
	{
		get
		{
			(bool explainsLeft, bool explainsRight) = GetExplainingParts();
			return explainsRight && (!explainsLeft || !Left.HasSameResultTextAs(Right));
		}
	}

	/// <inheritdoc />
	public override string? LeadingSubject
	{
		get
		{
			if (RendersLeft)
			{
				return Left.LeadingSubject;
			}

			return RendersRight ? Right.LeadingSubject : null;
		}
	}

	/// <inheritdoc />
	public override string? TrailingSubject
	{
		get
		{
			if (RendersRight)
			{
				return Right.TrailingSubject;
			}

			return RendersLeft ? Left.TrailingSubject : null;
		}
	}

	/// <inheritdoc />
	/// <remarks>
	///     The right operand does not explain the outcome after a failed left operand which ignores the result of the
	///     following ones, e.g. a failed null check.
	/// </remarks>
	protected override (bool Left, bool Right) GetExplainingParts()
	{
		bool explainsLeft = RendersLeft;
		return (explainsLeft, Right.ExplainsOutcomeOf(this) &&
		                      (!explainsLeft || FurtherProcessingStrategy != FurtherProcessingStrategy.IgnoreResult));
	}

	/// <inheritdoc />
	internal override bool IsNegatedAnd => _isAnd && GetAndSeparator() != _separator;

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isAnd)
		{
			Left.AppendExpectation(stringBuilder, indentation);
			stringBuilder.AppendSeparatedExpectation(GetAndSeparator(), Right, indentation);
			return;
		}

		bool isNegatedOr = _separator == " or " && IsNegated;
		AppendOrOperand(stringBuilder, Left, isNegatedOr, indentation);
		stringBuilder.Append(isNegatedOr ? " and " : _separator);
		AppendOrOperand(stringBuilder, Right, isNegatedOr, indentation);
	}

	/// <remarks>
	///     A negated combination is evaluated as "and" (De Morgan). A negated <c>And</c> operand reads as "or", which
	///     binds weaker, so it is put in parentheses to keep the evaluated structure.
	/// </remarks>
	private static void AppendOrOperand(StringBuilder stringBuilder, ConstraintResult operand, bool isNegatedOr,
		string? indentation)
	{
		if (!isNegatedOr || !operand.IsNegatedAnd)
		{
			operand.AppendExpectation(stringBuilder, indentation);
			return;
		}

		stringBuilder.Append('(');
		operand.AppendExpectation(stringBuilder, indentation);
		stringBuilder.Append(')');
	}

	/// <remarks>
	///     A negated combination is evaluated as "or" (De Morgan), so a leading "and" is swapped; <c>AndWhose</c> uses
	///     <c>" and"</c> without a trailing space. Separators without an "and" (<c>" that "</c>, <c>" "</c>) attach the
	///     right expectation to the subject introduced on the left, which an "or" would detach, so they are kept.
	/// </remarks>
	private string GetAndSeparator()
	{
		const string and = " and";
		return IsNegated && _separator.StartsWith(and, StringComparison.Ordinal)
			? " or" + _separator[and.Length..]
			: _separator;
	}

	/// <inheritdoc />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		bool rendersLeft = RendersLeft;
		int leftStart = stringBuilder.Length;
		if (rendersLeft)
		{
			Left.AppendResult(stringBuilder, indentation);
		}

		if (!RendersRight)
		{
			return;
		}

		if (rendersLeft)
		{
			stringBuilder.AppendAndResult(leftStart, Left, Right, indentation);
		}
		else
		{
			Right.AppendResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		IsNegated = !IsNegated;
		Left.Negate();
		Right.Negate();
		Outcome = CombineOutcomes();
		return this;
	}
}
