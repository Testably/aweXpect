using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Equivalency;

/// <summary>
///     Options for equivalency.
/// </summary>
public record EquivalencyOptions : EquivalencyTypeOptions
{
	private const int DefaultMaxRecursionDepth = 100;

	private readonly Func<Type, EquivalencyComparisonType>? _defaultComparisonTypeSelector;

	private readonly int _maxRecursionDepth = DefaultMaxRecursionDepth;

	/// <summary>
	///     Specifies the selector how types should be compared, if not overwritten in the <see cref="CustomOptions" />.
	/// </summary>
	/// <remarks>
	///     Defaults to use the <see cref="EquivalencyDefaults.DefaultComparisonType" />.
	/// </remarks>
	public Func<Type, EquivalencyComparisonType> DefaultComparisonTypeSelector
	{
		get => _defaultComparisonTypeSelector ?? EquivalencyDefaults.DefaultComparisonType;
		init => _defaultComparisonTypeSelector = value;
	}

	/// <summary>
	///     The maximum number of nested objects that are compared on a single path.
	/// </summary>
	/// <remarks>
	///     Defaults to 100. A graph that is deeper fails the comparison instead of overflowing the stack.
	/// </remarks>
	public int MaxRecursionDepth
	{
		get => _maxRecursionDepth;
		init
		{
			if (value < 1)
			{
				throw Tracing.WriteException(
					new ArgumentOutOfRangeException(nameof(value), value,
						"The maximum recursion depth must be greater than zero."));
			}

			_maxRecursionDepth = value;
		}
	}

	/// <summary>
	///     Custom type-specific equivalency options.
	/// </summary>
	public Dictionary<Type, EquivalencyTypeOptions> CustomOptions { get; init; } = new();

	/// <summary>
	///     Specifies the <paramref name="options" /> for members of type <typeparamref name="TMember" />.
	/// </summary>
	/// <remarks>
	///     The last registration for a type wins, so that a single expectation can override what the customized
	///     default already specifies for that type.
	/// </remarks>
	public EquivalencyOptions For<TMember>(
		Func<EquivalencyTypeOptions, EquivalencyTypeOptions> options)
	{
		EquivalencyTypeOptions typeOptions = options(this);
		CustomOptions[typeof(TMember)] = typeOptions;
		return this;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		StringBuilder? sb = new();
		AppendOptions(sb);
		if (MaxRecursionDepth != DefaultMaxRecursionDepth)
		{
			sb.Append(" - limit the recursion depth to ").Append(MaxRecursionDepth).AppendLine();
		}

		foreach (KeyValuePair<Type, EquivalencyTypeOptions> customOption in CustomOptions)
		{
			sb.Append(" - for ");
			Formatter.Format(sb, customOption.Key);
			sb.AppendLine(":");
			customOption.Value.AppendOptions(sb, "  ");
		}

		return sb.ToString().TrimEnd();
	}
}

/// <summary>
///     Options for equivalency for expected type <typeparamref name="TExpected" />.
/// </summary>
public record EquivalencyOptions<TExpected> : EquivalencyOptions
{
	/// <summary>
	///     Initializes the values with the <paramref name="inner" /> equivalency options.
	/// </summary>
	/// <remarks>
	///     Delegates to the copy constructor of the record, so that a member added later is copied as well instead of
	///     being dropped silently. Only <see cref="EquivalencyOptions.CustomOptions" /> needs a copy of its own,
	///     because <see cref="For{TMember}" /> mutates the dictionary in place and must not write into the
	///     <paramref name="inner" /> options, which are the customized default shared by every expectation.
	/// </remarks>
	public EquivalencyOptions(EquivalencyOptions inner) : base(inner)
		=> CustomOptions = new Dictionary<Type, EquivalencyTypeOptions>(inner.CustomOptions);

	/// <summary>
	///     Specifies the <paramref name="options" /> for members of type <typeparamref name="TMember" />.
	/// </summary>
	public new EquivalencyOptions<TExpected> For<TMember>(
		Func<EquivalencyTypeOptions, EquivalencyTypeOptions> options)
	{
		base.For<TMember>(options);
		return this;
	}

	/// <inheritdoc />
	public override string ToString() => base.ToString();
}
