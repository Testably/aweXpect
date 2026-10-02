using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	///     Specifies the selector how types should be compared, if not overwritten with <see cref="For{TMember}" />.
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

	/// <remarks>
	///     Only ever replaced as a whole, so that copies made with <see langword="with" /> can share it.
	/// </remarks>
	private Dictionary<Type, Func<EquivalencyTypeOptions, EquivalencyTypeOptions>> Registrations { get; init; } = new();

	/// <summary>
	///     Specifies the <paramref name="options" /> for members of type <typeparamref name="TMember" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="options" /> are applied to the final options when a comparison looks them up, so every
	///     other option applies to <typeparamref name="TMember" /> as well, regardless of the order of the calls. The
	///     last registration for a type wins, so that a single expectation can override what the customized default
	///     already specifies for that type.<br />
	///     The options are looked up by the runtime type of a value, and a boxed <see cref="Nullable{T}" /> has the
	///     underlying type, so a registration for <c>T?</c> applies to <c>T</c>, also to its non-nullable members.
	/// </remarks>
	/// <exception cref="ArgumentException"><typeparamref name="TMember" /> is an interface.</exception>
	public EquivalencyOptions For<TMember>(
		Func<EquivalencyTypeOptions, EquivalencyTypeOptions> options)
	{
		if (typeof(TMember).IsInterface)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"Options cannot be registered for the interface {Formatter.Format(typeof(TMember))}, because they are looked up by the runtime type of a value and its base types. Register them for a class or struct instead.",
				nameof(TMember)));
		}

		return this with
		{
			Registrations = new Dictionary<Type, Func<EquivalencyTypeOptions, EquivalencyTypeOptions>>(Registrations)
			{
				[Nullable.GetUnderlyingType(typeof(TMember)) ?? typeof(TMember)] = options,
			},
		};
	}

	/// <summary>
	///     Returns the options that apply to values of the <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     Uses the registration with <see cref="For{TMember}" /> for the <paramref name="type" /> or its nearest base
	///     type, and otherwise these options.
	/// </remarks>
	public EquivalencyTypeOptions GetOptionsFor(Type type)
		=> TryGetOptionsFor(type, out EquivalencyTypeOptions? options) ? options : this;

	/// <remarks>
	///     The base types are walked, most derived first: the <paramref name="type" /> is the runtime type of a value,
	///     which can never be an abstract type the user registered options for. A <see cref="Type" /> member is a
	///     <c>RuntimeType</c> at runtime, a type that cannot even be named, so an exact match alone would make
	///     <see cref="For{TMember}" /> unreachable for it. Interfaces cannot be registered, because several of them can
	///     match without an order that decides between them.
	/// </remarks>
	internal bool TryGetOptionsFor(Type type, [NotNullWhen(true)] out EquivalencyTypeOptions? options)
	{
		for (Type? candidate = type; candidate != null; candidate = candidate.BaseType)
		{
			if (Registrations.TryGetValue(candidate,
				    out Func<EquivalencyTypeOptions, EquivalencyTypeOptions>? registration))
			{
				options = registration(this);
				return true;
			}
		}

		options = null;
		return false;
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

		foreach (KeyValuePair<Type, Func<EquivalencyTypeOptions, EquivalencyTypeOptions>> registration in
		         Registrations)
		{
			sb.Append(" - for ");
			Formatter.Format(sb, registration.Key);
			sb.AppendLine(":");
			registration.Value(this).AppendOptions(sb, "  ");
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
	///     being dropped silently.
	/// </remarks>
	public EquivalencyOptions(EquivalencyOptions inner) : base(inner)
	{
	}

	/// <inheritdoc cref="EquivalencyOptions.For{TMember}" />
	public new EquivalencyOptions<TExpected> For<TMember>(
		Func<EquivalencyTypeOptions, EquivalencyTypeOptions> options)
		=> (EquivalencyOptions<TExpected>)base.For<TMember>(options);

	/// <inheritdoc />
	public override string ToString() => base.ToString();
}
