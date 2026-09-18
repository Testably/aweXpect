using System;
using System.Linq.Expressions;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The member accessor.
/// </summary>
public abstract class MemberAccessor
{
	private readonly string _name;

	/// <summary>
	///     Creates a new member accessor.
	/// </summary>
	protected MemberAccessor(string name)
	{
		_name = name;
	}

	/// <inheritdoc />
	public override string ToString()
		=> _name;
}

/// <summary>
///     The member accessor from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
/// </summary>
public class MemberAccessor<TSource, TTarget> : MemberAccessor
{
	private readonly Func<TSource, TTarget> _accessor;

	private MemberAccessor(Func<TSource, TTarget> accessor, string name) :
		base(name)
	{
		_accessor = accessor;
	}

	/// <summary>
	///     Creates a member accessor from the given <paramref name="expression" />.
	/// </summary>
	public static MemberAccessor<TSource, TTarget> FromExpression(
		Expression<Func<TSource, TTarget>> expression)
	{
		Func<TSource, TTarget> compiled = expression.Compile();
		return new MemberAccessor<TSource, TTarget>(
			v => compiled(v),
			$"{ExpressionHelpers.GetMemberPath(expression)} ");
	}

	/// <summary>
	///     Creates a member accessor from the given <paramref name="func" />.
	/// </summary>
	public static MemberAccessor<TSource, TTarget> FromFunc(
		Func<TSource, TTarget> func, string name)
		=> new(func, name);

	/// <summary>
	///     Creates a member accessor from the given <paramref name="func" />.
	/// </summary>
	public static MemberAccessor<TSource, TTarget> FromFuncAsMemberAccessor(
		Func<TSource, TTarget> func, string name)
		=> new(func, ExtractMemberPath(name.Trim()));

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is MemberAccessor<TSource, TTarget> other && Equals(other);

	private bool Equals(MemberAccessor<TSource, TTarget> other)
		=> ToString().Equals(other.ToString());

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => ToString().GetHashCode();

	private static string ExtractMemberPath(string expression)
	{
		// Example: "x => x.Foo" would result in "Foo"
		int idx = expression.IndexOf("=>", StringComparison.Ordinal);
		if (idx > 0)
		{
			string? prefix = expression.Substring(0, idx).Trim();
			idx = expression.IndexOf(prefix, idx, StringComparison.Ordinal);
			if (idx > 0)
			{
				expression = expression.Substring(idx + prefix.Length).TrimStart();
				if (expression.StartsWith(".", StringComparison.Ordinal))
				{
					expression = expression.Substring(1);
				}
			}
		}

		return $"{expression} ";
	}

	internal TTarget AccessMember(TSource value) => _accessor.Invoke(value);
}
