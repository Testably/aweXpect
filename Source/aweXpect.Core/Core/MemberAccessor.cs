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
	///     Creates a member accessor from the given <paramref name="func" />, which is displayed as the
	///     <paramref name="name" /> verbatim.
	/// </summary>
	public static MemberAccessor<TSource, TTarget> FromFunc(
		Func<TSource, TTarget> func, string name)
		=> new(func, name);

	/// <summary>
	///     Creates a member accessor from the given <paramref name="func" />, which treats the <paramref name="name" /> as
	///     a lambda expression and is displayed as its member path (for example <c>x => x.Foo</c> as <c>Foo</c>).
	/// </summary>
	/// <remarks>
	///     A <paramref name="name" /> that is not such a lambda expression is displayed as is, without surrounding
	///     white-space.
	/// </remarks>
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
			string parameter = expression.Substring(0, idx).Trim();
			if (parameter.Length > 2 && parameter[0] == '(' && parameter[parameter.Length - 1] == ')')
			{
				parameter = parameter.Substring(1, parameter.Length - 2).Trim();
			}

			if (IsIdentifier(parameter))
			{
				string body = expression.Substring(idx + 2).Trim();
				if (body == parameter)
				{
					return "it ";
				}

				// Only a leading parameter access is stripped; any other body (e.g. a cast) is kept whole,
				// because removing the parameter from it would no longer read as the selected member.
				if (body.StartsWith(parameter + "?.", StringComparison.Ordinal))
				{
					body = body.Substring(parameter.Length + 2);
				}
				else if (body.StartsWith(parameter + ".", StringComparison.Ordinal))
				{
					body = body.Substring(parameter.Length + 1);
				}
				else if (body.StartsWith(parameter + "[", StringComparison.Ordinal))
				{
					body = body.Substring(parameter.Length);
				}

				return $"{body} ";
			}
		}

		return $"{expression} ";
	}

	private static bool IsIdentifier(string value)
	{
		int start = value.Length > 1 && value[0] == '@' ? 1 : 0;
		if (value.Length == start || char.IsDigit(value[start]))
		{
			return false;
		}

		for (int i = start; i < value.Length; i++)
		{
			if (!char.IsLetterOrDigit(value[i]) && value[i] != '_')
			{
				return false;
			}
		}

		return true;
	}

	internal TTarget AccessMember(TSource value) => _accessor.Invoke(value);
}
