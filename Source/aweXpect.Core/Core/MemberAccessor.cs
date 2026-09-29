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
			if (TryGetAsyncParameter(parameter, out string asyncParameter))
			{
				string? path = GetAwaitedMemberPath(asyncParameter, expression.Substring(idx + 2).Trim());
				return $"{path ?? expression} ";
			}

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

	private static bool TryGetAsyncParameter(string parameter, out string name)
	{
		const string asyncKeyword = "async";
		name = "";
		if (parameter.Length <= asyncKeyword.Length ||
		    !parameter.StartsWith(asyncKeyword, StringComparison.Ordinal) ||
		    !(char.IsWhiteSpace(parameter[asyncKeyword.Length]) || parameter[asyncKeyword.Length] == '('))
		{
			return false;
		}

		string declaration = parameter.Substring(asyncKeyword.Length).Trim();
		if (declaration.Length > 2 && declaration[0] == '(' && declaration[declaration.Length - 1] == ')')
		{
			declaration = declaration.Substring(1, declaration.Length - 2).Trim();
			// A typed parameter (e.g. "Foo o") is named by its last word.
			int separator = declaration.LastIndexOfAny([' ', '\t', '\r', '\n',]);
			if (separator > 0 && !HasTopLevelComma(declaration.Substring(0, separator)))
			{
				declaration = declaration.Substring(separator + 1);
			}
		}

		name = declaration;
		return IsIdentifier(name);
	}

	private static bool HasTopLevelComma(string value)
	{
		int depth = 0;
		foreach (char c in value)
		{
			if (c is '<' or '(' or '[')
			{
				depth++;
			}
			else if (c is '>' or ')' or ']')
			{
				depth--;
			}
			else if (c == ',' && depth == 0)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Returns the member path of an async lambda body like <c>await o.Foo.GetAsync()</c> (<c>Foo.GetAsync()</c>),
	///     or <see langword="null" /> if the body is anything else.
	/// </summary>
	private static string? GetAwaitedMemberPath(string parameter, string body)
	{
		const string awaitKeyword = "await";
		if (body.Length <= awaitKeyword.Length ||
		    !body.StartsWith(awaitKeyword, StringComparison.Ordinal) ||
		    !char.IsWhiteSpace(body[awaitKeyword.Length]))
		{
			return null;
		}

		string awaited = body.Substring(awaitKeyword.Length).TrimStart();
		if (!awaited.StartsWith(parameter, StringComparison.Ordinal))
		{
			return null;
		}

		string path = awaited.Substring(parameter.Length);
		if (path.StartsWith("?.", StringComparison.Ordinal))
		{
			path = path.Substring(2);
		}
		else if (path.StartsWith(".", StringComparison.Ordinal))
		{
			path = path.Substring(1);
		}
		else if (!path.StartsWith("[", StringComparison.Ordinal))
		{
			return null;
		}

		int lastSegment = GetLastSegmentStart(path);
		return lastSegment < 0 ? null : WithoutConfigureAwait(path, lastSegment);
	}

	// ConfigureAwait only controls how the value is awaited, it is not part of the selected member.
	private static string? WithoutConfigureAwait(string path, int lastSegment)
	{
		if (!path.Substring(lastSegment).StartsWith("ConfigureAwait(", StringComparison.Ordinal) ||
		    path[path.Length - 1] != ')')
		{
			return path;
		}

		return lastSegment > 1 && path[lastSegment - 1] == '.' && path[lastSegment - 2] != '?'
			? path.Substring(0, lastSegment - 1)
			: null;
	}

	private static int GetSeparatorLength(string path, int index)
	{
		if (path[index] == '.')
		{
			return 1;
		}

		return path[index] == '?' && index + 1 < path.Length && path[index + 1] == '.' ? 2 : 0;
	}

	/// <summary>
	///     Returns the start of the last segment, if the <paramref name="path" /> is a chain of members, method calls and
	///     indexers (e.g. <c>Foo?.Bar[0].GetAsync&lt;int&gt;(1)</c>), otherwise <c>-1</c>.
	/// </summary>
	private static int GetLastSegmentStart(string path)
	{
		int segmentStart = 0;
		int index = path.StartsWith("[", StringComparison.Ordinal) ? 0 : SkipIdentifier(path, 0);
		while (index >= 0 && index < path.Length)
		{
			int separatorLength = GetSeparatorLength(path, index);
			if (path[index] is '(' or '[')
			{
				index = SkipGroup(path, index);
			}
			else if (separatorLength > 0)
			{
				segmentStart = index + separatorLength;
				index = SkipIdentifier(path, segmentStart);
			}
			else if (path[index] == '?' && index + 1 < path.Length && path[index + 1] == '[')
			{
				index++;
			}
			else
			{
				return -1;
			}
		}

		return index < 0 ? -1 : segmentStart;
	}

	private static int SkipIdentifier(string value, int start)
	{
		int index = start < value.Length && value[start] == '@' ? start + 1 : start;
		if (index >= value.Length || !(char.IsLetter(value[index]) || value[index] == '_'))
		{
			return -1;
		}

		while (index < value.Length && (char.IsLetterOrDigit(value[index]) || value[index] == '_'))
		{
			index++;
		}

		return index < value.Length && value[index] == '<' ? SkipTypeArguments(value, index) : index;
	}

	private static int SkipTypeArguments(string value, int start)
	{
		int depth = 0;
		for (int index = start; index < value.Length; index++)
		{
			char c = value[index];
			if (c == '<')
			{
				depth++;
			}
			else if (c == '>')
			{
				depth--;
				if (depth == 0)
				{
					// Only a generic method call is part of a member path, otherwise it is a comparison.
					return index + 1 < value.Length && value[index + 1] == '(' ? index + 1 : -1;
				}
			}
			else if (!char.IsLetterOrDigit(c) && c is not ('_' or '.' or ',' or ' ' or '?' or '[' or ']'))
			{
				return -1;
			}
		}

		return -1;
	}

	private static int SkipGroup(string value, int start)
	{
		int depth = 0;
		int index = start;
		while (index >= 0 && index < value.Length)
		{
			char c = value[index];
			if (c is '(' or '[')
			{
				depth++;
			}
			else if (c is ')' or ']')
			{
				depth--;
				if (depth == 0)
				{
					return index + 1;
				}
			}

			index = SkipLiteralOrComment(value, index);
			if (index >= 0)
			{
				index++;
			}
		}

		return -1;
	}

	/// <summary>
	///     Returns the end of the literal or comment starting at <paramref name="start" />, so that brackets within it are
	///     not counted, the <paramref name="start" /> for any other character or <c>-1</c> if it is not terminated.
	/// </summary>
	private static int SkipLiteralOrComment(string value, int start)
	{
		char c = value[start];
		if (c is '"' or '\'')
		{
			return SkipLiteral(value, start);
		}

		if (c != '/' || start + 1 >= value.Length)
		{
			return start;
		}

		if (value[start + 1] == '*')
		{
			int end = value.IndexOf("*/", start + 2, StringComparison.Ordinal);
			return end < 0 ? -1 : end + 1;
		}

		return value[start + 1] == '/' ? value.IndexOf('\n', start + 2) : start;
	}

	private static int SkipLiteral(string value, int start)
	{
		char quote = value[start];
		int index = start + 1;
		while (index < value.Length)
		{
			if (value[index] == '\\')
			{
				index++;
			}
			else if (value[index] == quote)
			{
				return index;
			}

			index++;
		}

		return -1;
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
