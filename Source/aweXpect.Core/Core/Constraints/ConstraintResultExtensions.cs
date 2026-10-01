using System;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

/// <summary>
///     Extension methods on <see cref="ConstraintResult" />.
/// </summary>
public static class ConstraintResultExtensions
{
	/// <summary>
	///     Negates the <paramref name="constraintResult" /> and returns the same instance.
	/// </summary>
	public static T Invert<T>(this T constraintResult) where T : ConstraintResult
	{
		constraintResult.Negate();
		return constraintResult;
	}

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> from the <paramref name="inner" /> using the given
	///     <paramref name="value" />.
	/// </summary>
	public static ConstraintResult UseValue<T>(this ConstraintResult inner, T value)
		=> new ConstraintResultValueWrapper<T>(inner, value);

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> where the expectation is prepended with the
	///     <paramref name="prefix" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="prefix" /> is treated as a separator, so a trailing <c>that</c> is dropped when the
	///     expectation of <paramref name="inner" /> starts with its own <c>whose</c>.
	/// </remarks>
	public static ConstraintResult PrependExpectationText(this ConstraintResult inner, Action<StringBuilder>? prefix)
		=> new ConstraintResultExpectationWrapper(inner, prefix);

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> where the expectation is appended with the <paramref name="suffix" />.
	/// </summary>
	public static ConstraintResult AppendExpectationText(this ConstraintResult inner, Action<StringBuilder>? suffix)
		=> new ConstraintResultExpectationWrapper(inner, null, suffix);

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> with <see cref="Outcome.Failure" /> from
	///     the <paramref name="inner" /> using the given <paramref name="value" />.
	/// </summary>
	public static ConstraintResult Fail<T>(this ConstraintResult inner, string failure, T value)
		=> new ConstraintResultFailure<T>(inner, failure, value);

	/// <summary>
	///     Checks if the two <see cref="ConstraintResult" />s have the same result text.
	/// </summary>
	public static bool HasSameResultTextAs(this ConstraintResult left, ConstraintResult right)
		=> left.GetResultText() == right.GetResultText();

	/// <summary>
	///     Checks if the result of the <paramref name="operand" /> explains the outcome of the
	///     <paramref name="combination" /> it is part of.
	/// </summary>
	/// <remarks>
	///     An undecided operand only explains an undecided combination, as a failed one is explained by its failed operands.
	/// </remarks>
	internal static bool ExplainsOutcomeOf(this ConstraintResult operand, ConstraintResult combination)
		=> operand.Outcome == Outcome.Failure ||
		   (operand.Outcome == Outcome.Undecided && combination.Outcome == Outcome.Undecided);

	/// <summary>
	///     Appends the "and" which joins a further result to the result that was appended from
	///     <paramref name="resultStart" /> on.
	/// </summary>
	/// <remarks>
	///     After a result that spans several lines (e.g. a diff), the "and" starts a new line, so that the further result
	///     is not glued onto its last line.
	/// </remarks>
	/// <returns><see langword="true" />, when the further result starts on a new line.</returns>
	internal static bool AppendAndSeparator(this StringBuilder stringBuilder, int resultStart, string? indentation)
	{
		for (int i = resultStart; i < stringBuilder.Length; i++)
		{
			if (stringBuilder[i] == '\n')
			{
				stringBuilder.AppendLine().Append(indentation).Append("and ");
				return true;
			}
		}

		stringBuilder.Append(" and ");
		return false;
	}

	/// <summary>
	///     Appends the "and" and the result of <paramref name="right" /> after the result of <paramref name="left" />,
	///     which was appended from <paramref name="leftStart" /> on.
	/// </summary>
	/// <remarks>
	///     The subject of <paramref name="right" /> is omitted, when it continues the same line and the last part of the
	///     result of <paramref name="left" /> starts with the same subject.
	/// </remarks>
	internal static void AppendAndResult(this StringBuilder stringBuilder, int leftStart, ConstraintResult left,
		ConstraintResult right, string? indentation)
	{
		if (stringBuilder.AppendAndSeparator(leftStart, indentation))
		{
			right.AppendResult(stringBuilder, indentation);
			return;
		}

		string? subject = left.TrailingSubject;
		if (subject is null || right.LeadingSubject != subject)
		{
			right.AppendResult(stringBuilder, indentation);
			return;
		}

		string rightResult = GetResultText(right, indentation);
		stringBuilder.Append(rightResult.StartsWith(subject + " ", StringComparison.Ordinal)
			? rightResult.Substring(subject.Length + 1)
			: rightResult);
	}

	private static string GetResultText(this ConstraintResult result, string? indentation = null)
	{
		StringBuilder sb = new();
		result.AppendResult(sb, indentation);
		return sb.ToString();
	}

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> which only contributes the expectation of the
	///     <paramref name="inner" />, e.g. for an operand that was skipped and only evaluated for its expectation text.
	/// </summary>
	internal static ConstraintResult AsExpectationOnly(this ConstraintResult inner)
		=> new ConstraintResultExpectationOnlyWrapper(inner);

	private sealed class ConstraintResultExpectationOnlyWrapper : ConstraintResult
	{
		private readonly ConstraintResult _inner;

		public ConstraintResultExpectationOnlyWrapper(ConstraintResult inner) : base(inner.FurtherProcessingStrategy)
		{
			_inner = inner;
			Outcome = Outcome.Success;
		}

		internal override bool IsExpectationOnly => true;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			// The operand was not evaluated, so there is no result.
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> _inner.TryGetStoredValue(out value);

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			return this;
		}
	}

	private sealed class ConstraintResultValueWrapper<T> : ConstraintResult
	{
		private readonly ConstraintResult _inner;
		private readonly T _value;

		public ConstraintResultValueWrapper(ConstraintResult inner, T value)
			: base(inner.FurtherProcessingStrategy)
		{
			Outcome = inner.Outcome;
			_inner = inner;
			_value = value;
		}

		public override Exception? FailureCause => _inner.FailureCause;

		internal override string? LeadingSubject => _inner.LeadingSubject;

		internal override string? TrailingSubject => _inner.TrailingSubject;

		internal override bool IsExpectationOnly => _inner.IsExpectationOnly;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendResult(stringBuilder, indentation);

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_value is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(T));
		}

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			Outcome = _inner.Outcome;
			return this;
		}
	}

	private sealed class ConstraintResultFailure<T> : ConstraintResult
	{
		private readonly string _failure;
		private readonly ConstraintResult _inner;
		private readonly T _value;

		public ConstraintResultFailure(ConstraintResult inner, string failure, T value) : base(
			inner.FurtherProcessingStrategy)
		{
			Outcome = Outcome.Failure;
			_inner = inner;
			_failure = failure;
			_value = value;
		}

		public override Exception? FailureCause => _inner.FailureCause;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_failure.Indent(indentation));

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_value is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(T));
		}

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			return this;
		}
	}

	private sealed class ConstraintResultExpectationWrapper : ConstraintResult
	{
		private readonly bool _includeInnerExpectation;
		private readonly ConstraintResult _inner;
		private readonly Action<StringBuilder>? _prefix;
		private readonly Action<StringBuilder>? _suffix;

		public ConstraintResultExpectationWrapper(ConstraintResult inner,
			Action<StringBuilder>? prefix = null,
			Action<StringBuilder>? suffix = null,
			bool includeInnerExpectation = true) : base(inner.FurtherProcessingStrategy)
		{
			_inner = inner;
			Outcome = _inner.Outcome;
			_prefix = prefix;
			_suffix = suffix;
			_includeInnerExpectation = includeInnerExpectation;
		}

		public override Exception? FailureCause => _inner.FailureCause;

		internal override string? LeadingSubject => _inner.LeadingSubject;

		internal override string? TrailingSubject => _inner.TrailingSubject;

		internal override bool IsExpectationOnly => _inner.IsExpectationOnly;

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> _inner.TryGetStoredValue(out value);

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			Outcome = _inner.Outcome;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			StringBuilder prefix = new();
			_prefix?.Invoke(prefix);
			if (_includeInnerExpectation)
			{
				stringBuilder.AppendSeparatedExpectation(prefix.ToString(),
					sb => _inner.AppendExpectation(sb, indentation));
			}
			else
			{
				stringBuilder.Append(prefix);
			}

			_suffix?.Invoke(stringBuilder);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendResult(stringBuilder, indentation);
	}
}
