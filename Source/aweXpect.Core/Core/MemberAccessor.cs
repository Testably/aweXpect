using System;
using System.Linq.Expressions;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The member accessor.
/// </summary>
public abstract class MemberAccessor
{
	/// <summary>
	///     Whether <see cref="ToString()" /> appends a space to the <see cref="_name" />.
	/// </summary>
	/// <remarks>
	///     The space is only appended when the name is read for a failure message, so that the subject of the
	///     expectations on the member (<see cref="GetTrimmedName" />) needs no second string.
	/// </remarks>
	private readonly bool _appendsSpace;

	private readonly string _name;

	/// <summary>
	///     Creates a new member accessor.
	/// </summary>
	protected MemberAccessor(string name)
	{
		_name = name;
	}

	private protected MemberAccessor(string name, bool appendsSpace)
	{
		_name = name;
		_appendsSpace = appendsSpace;
	}

	/// <inheritdoc />
	public override string ToString()
		=> _appendsSpace ? _name + " " : _name;

	/// <summary>
	///     The name without surrounding white-space, e.g. as the subject of the expectations on the member.
	/// </summary>
	internal string GetTrimmedName() => _name.Trim();
}

/// <summary>
///     The member accessor from <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
/// </summary>
public class MemberAccessor<TSource, TTarget> : MemberAccessor
{
	private readonly Func<TSource, TTarget> _accessor;

	private MemberAccessor(Func<TSource, TTarget> accessor, string name, bool appendsSpace = false) :
		base(name, appendsSpace)
	{
		_accessor = accessor;
	}

	/// <summary>
	///     Creates a member accessor from the given <paramref name="expression" />, which is displayed as its member
	///     path (for example <c>x => x.Foo.Bar</c> as <c>Foo.Bar</c>).
	/// </summary>
	/// <remarks>
	///     An <paramref name="expression" /> that is no member path is displayed as the expression itself.
	/// </remarks>
	public static MemberAccessor<TSource, TTarget> FromExpression(
		Expression<Func<TSource, TTarget>> expression)
	{
		Func<TSource, TTarget> compiled = expression.Compile();
		return new MemberAccessor<TSource, TTarget>(
			v => compiled(v),
			ExpressionHelpers.GetMemberPath(expression), true);
	}

	/// <summary>
	///     Creates a member accessor from the given <paramref name="func" />, which is displayed as the
	///     <paramref name="name" /> verbatim.
	/// </summary>
	public static MemberAccessor<TSource, TTarget> FromFuncWithName(
		Func<TSource, TTarget> func, string name)
		=> new(func, name);

	/// <summary>
	///     Creates a member accessor from the given <paramref name="func" />, which treats the
	///     <paramref name="callerExpression" /> as a lambda expression and is displayed as its member path (for example
	///     <c>x => x.Foo</c> as <c>Foo</c>).
	/// </summary>
	/// <remarks>
	///     Pass the expression of the <paramref name="func" /> from a <c>CallerArgumentExpression</c> parameter. A
	///     <paramref name="callerExpression" /> that is not such a lambda expression is displayed as is, without
	///     surrounding whitespace.
	/// </remarks>
	public static MemberAccessor<TSource, TTarget> FromFuncWithCallerExpression(
		Func<TSource, TTarget> func, string callerExpression)
		=> new(func, ExtractMemberPath(callerExpression.Trim()), true);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is MemberAccessor<TSource, TTarget> other && Equals(other);

	private bool Equals(MemberAccessor<TSource, TTarget> other)
		=> ToString().Equals(other.ToString());

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => ToString().GetHashCode();

	/// <remarks>
	///     The parameter and the body are compared by their position in the <paramref name="expression" />, so that
	///     only the returned member path is allocated, as this runs for every expectation on a member.
	/// </remarks>
	private static string ExtractMemberPath(string expression)
	{
		// Example: "x => x.Foo" would result in "Foo"
		int idx = expression.IndexOf("=>", StringComparison.Ordinal);
		if (idx <= 0)
		{
			return expression;
		}

		int parameterStart = 0;
		int parameterEnd = idx;
		TrimRange(expression, ref parameterStart, ref parameterEnd);
		if (string.CompareOrdinal(expression, parameterStart, "async", 0, 5) == 0 &&
		    TryGetAsyncParameter(expression.Substring(parameterStart, parameterEnd - parameterStart),
			    out string asyncParameter))
		{
			return GetAwaitedMemberPath(asyncParameter, expression.Substring(idx + 2).Trim()) ?? expression;
		}

		if (parameterEnd - parameterStart > 2 && expression[parameterStart] == '(' &&
		    expression[parameterEnd - 1] == ')')
		{
			parameterStart++;
			parameterEnd--;
			TrimRange(expression, ref parameterStart, ref parameterEnd);
		}

		if (!IsIdentifier(expression, parameterStart, parameterEnd))
		{
			return expression;
		}

		int bodyStart = idx + 2;
		int bodyEnd = expression.Length;
		TrimRange(expression, ref bodyStart, ref bodyEnd);
		int parameterLength = parameterEnd - parameterStart;
		if (bodyEnd - bodyStart >= parameterLength &&
		    string.CompareOrdinal(expression, bodyStart, expression, parameterStart, parameterLength) == 0)
		{
			if (bodyEnd - bodyStart == parameterLength)
			{
				return "it";
			}

			bodyStart += GetLeadingParameterLength(expression, bodyStart + parameterLength, bodyEnd, parameterLength);
		}

		return expression.Substring(bodyStart, bodyEnd - bodyStart);
	}

	// Only a leading parameter access is stripped; any other body (e.g. a cast) is kept whole,
	// because removing the parameter from it would no longer read as the selected member.
	private static int GetLeadingParameterLength(string expression, int afterParameter, int bodyEnd,
		int parameterLength)
	{
		if (bodyEnd - afterParameter >= 2 && expression[afterParameter] == '?' &&
		    expression[afterParameter + 1] == '.')
		{
			return parameterLength + 2;
		}

		if (expression[afterParameter] == '.')
		{
			return parameterLength + 1;
		}

		return expression[afterParameter] == '[' ? parameterLength : 0;
	}

	private static void TrimRange(string value, ref int start, ref int end)
	{
		while (start < end && char.IsWhiteSpace(value[start]))
		{
			start++;
		}

		while (end > start && char.IsWhiteSpace(value[end - 1]))
		{
			end--;
		}
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
		else if (path.StartsWith('.'))
		{
			path = path.Substring(1);
		}
		else if (!path.StartsWith('['))
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
		int index = path.StartsWith('[') ? 0 : SkipIdentifier(path, 0);
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

	private static bool IsIdentifier(string value) => IsIdentifier(value, 0, value.Length);

	private static bool IsIdentifier(string value, int start, int end)
	{
		if (end - start > 1 && value[start] == '@')
		{
			start++;
		}

		if (end == start || char.IsDigit(value[start]))
		{
			return false;
		}

		for (int i = start; i < end; i++)
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
