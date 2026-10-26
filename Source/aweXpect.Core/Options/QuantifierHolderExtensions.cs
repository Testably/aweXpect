using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="Quantifier" /> on the <see cref="OptionsHolder{TOptions}" /> of an expectation.
/// </summary>
public static class QuantifierHolderExtensions
{
	/// <inheritdoc cref="Quantifier.AtLeast(int)" />
	public static OptionsHolder<Quantifier> AtLeast(this OptionsHolder<Quantifier> holder, int minimum)
	{
		holder.Value = holder.Value.AtLeast(minimum);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.AtMost(int)" />
	public static OptionsHolder<Quantifier> AtMost(this OptionsHolder<Quantifier> holder, int maximum)
	{
		holder.Value = holder.Value.AtMost(maximum);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.Between(int, int)" />
	public static OptionsHolder<Quantifier> Between(this OptionsHolder<Quantifier> holder, int minimum, int maximum)
	{
		holder.Value = holder.Value.Between(minimum, maximum);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.Exactly(int)" />
	public static OptionsHolder<Quantifier> Exactly(this OptionsHolder<Quantifier> holder, int expected)
	{
		holder.Value = holder.Value.Exactly(expected);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.Exactly(int, QuantifierOption)" />
	internal static OptionsHolder<Quantifier> Exactly(this OptionsHolder<Quantifier> holder, int expected,
		QuantifierOption option)
	{
		holder.Value = holder.Value.Exactly(expected, option);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.LessThan(int)" />
	public static OptionsHolder<Quantifier> LessThan(this OptionsHolder<Quantifier> holder, int maximum)
	{
		holder.Value = holder.Value.LessThan(maximum);
		return holder;
	}

	/// <inheritdoc cref="Quantifier.MoreThan(int)" />
	public static OptionsHolder<Quantifier> MoreThan(this OptionsHolder<Quantifier> holder, int minimum)
	{
		holder.Value = holder.Value.MoreThan(minimum);
		return holder;
	}

	/// <summary>
	///     Verifies that it never occurs.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">Another quantifier is already specified.</exception>
	public static OptionsHolder<Quantifier> Never(this OptionsHolder<Quantifier> holder)
		=> holder.Exactly(0, QuantifierOption.Never);

	/// <summary>
	///     Verifies that it occurs exactly once.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">Another quantifier is already specified.</exception>
	public static OptionsHolder<Quantifier> Once(this OptionsHolder<Quantifier> holder)
		=> holder.Exactly(1, QuantifierOption.Once);

	/// <summary>
	///     Verifies that it occurs exactly twice.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">Another quantifier is already specified.</exception>
	public static OptionsHolder<Quantifier> Twice(this OptionsHolder<Quantifier> holder)
		=> holder.Exactly(2, QuantifierOption.Twice);
}
