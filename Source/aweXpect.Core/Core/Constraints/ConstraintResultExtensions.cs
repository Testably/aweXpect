using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

/// <summary>
///     Extensions methods on <see cref="ConstraintResult" />
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
	///     <paramref name="prefix" />
	/// </summary>
	public static ConstraintResult PrependExpectationText(this ConstraintResult inner, Action<StringBuilder>? prefix)
		=> new ConstraintResultExpectationWrapper(inner, prefix);

	/// <summary>
	///     Creates a new <see cref="ConstraintResult" /> where the expectation is appended with the <paramref name="suffix" />
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

	private static string GetResultText(this ConstraintResult result)
	{
		StringBuilder sb = new();
		result.AppendResult(sb);
		return sb.ToString();
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

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendResult(stringBuilder, indentation);

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
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

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_value is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(T));
		}

		public override ConstraintResult Negate() => _inner.Negate();
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

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
			=> _inner.TryGetValue(out value);

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			Outcome = _inner.Outcome;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			_prefix?.Invoke(stringBuilder);
			if (_includeInnerExpectation)
			{
				_inner.AppendExpectation(stringBuilder, indentation);
			}

			_suffix?.Invoke(stringBuilder);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendResult(stringBuilder, indentation);
	}
}
