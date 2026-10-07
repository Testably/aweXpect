using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace aweXpect.Core.Helpers;

internal static class ExpressionHelpers
{
	public static MemberExpression? GetMemberExpression(this Expression? expression)
		=> expression switch
		{
			MemberExpression memberExpression => memberExpression,
			LambdaExpression lambdaExpression => lambdaExpression.Body switch
			{
				MemberExpression body => body,
				UnaryExpression { Operand: MemberExpression m, } => m,
				_ => null,
			},
			_ => null,
		};

	/// <summary>
	///     Returns the member path that the <paramref name="expression" /> selects on its parameter (for example
	///     <c>Foo.Bar</c> for <c>x => x.Foo.Bar</c>), or <c>it</c> when it selects the parameter itself.
	/// </summary>
	/// <remarks>
	///     The path includes method calls and indexers. Conversions are omitted, because the ones the compiler adds
	///     (for example to a nullable type) cannot be told apart from a cast. Anything else is described by the
	///     expression itself, in which a captured variable is replaced by its name.
	/// </remarks>
	public static string GetMemberPath(LambdaExpression expression)
	{
		StringBuilder path = new();
		AppendPath(path, expression.Body, expression.Parameters[0]);
		return path.Length == 0 ? "it" : path.ToString();
	}

	/// <summary>
	///     Appends the <paramref name="expression" /> to the <paramref name="path" />, without the
	///     <paramref name="source" /> parameter the path starts at.
	/// </summary>
	private static void AppendPath(StringBuilder path, Expression expression, ParameterExpression? source)
	{
		switch (expression)
		{
			case ParameterExpression parameter when parameter == source:
				break;
			case UnaryExpression
			{
				NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.TypeAs,
			} conversion:
				AppendPath(path, conversion.Operand, source);
				break;
			case MemberExpression member:
				AppendInstance(path, member.Expression, member.Member.DeclaringType, source, ".");
				path.Append(member.Member.Name);
				break;
			case UnaryExpression { NodeType: ExpressionType.ArrayLength, } arrayLength:
				AppendInstance(path, arrayLength.Operand, null, source, ".");
				path.Append(nameof(Array.Length));
				break;
			case BinaryExpression { NodeType: ExpressionType.ArrayIndex, } arrayIndex:
				AppendPath(path, arrayIndex.Left, source);
				path.Append('[');
				AppendPath(path, arrayIndex.Right, null);
				path.Append(']');
				break;
			case MethodCallExpression call:
				AppendCall(path, call, source);
				break;
			case ConstantExpression { Value: IFormattable value, }:
				path.Append(value.ToString(null, CultureInfo.InvariantCulture));
				break;
			default:
				path.Append(CapturedVariableNames.Instance.Visit(expression));
				break;
		}
	}

	private static void AppendCall(StringBuilder path, MethodCallExpression call, ParameterExpression? source)
	{
		MethodInfo method = call.Method;
		int firstArgument = 0;
		char closing = ')';
		if (call.Object is not null && method.IsSpecialName && call.Arguments.Count > 0 &&
		    method.Name.StartsWith("get_", StringComparison.Ordinal))
		{
			// A property getter with arguments is an indexer.
			AppendInstance(path, call.Object, method.DeclaringType, source, "");
			path.Append('[');
			closing = ']';
		}
		else if (call.Object is null && method.IsDefined(typeof(ExtensionAttribute), false))
		{
			AppendInstance(path, call.Arguments[0], method.DeclaringType, source, ".");
			path.Append(method.Name).Append('(');
			firstArgument = 1;
		}
		else
		{
			AppendInstance(path, call.Object, method.DeclaringType, source, ".");
			path.Append(method.Name).Append('(');
		}

		for (int index = firstArgument; index < call.Arguments.Count; index++)
		{
			if (index > firstArgument)
			{
				path.Append(", ");
			}

			AppendPath(path, call.Arguments[index], null);
		}

		path.Append(closing);
	}

	/// <summary>
	///     Appends the <paramref name="instance" /> a member is accessed on, or its <paramref name="declaringType" />
	///     when it is static, followed by the <paramref name="separator" />.
	/// </summary>
	/// <remarks>
	///     A <see cref="IsCapturing">capturing</see> instance is omitted, because the source code of the expression
	///     does not name it.
	/// </remarks>
	private static void AppendInstance(StringBuilder path, Expression? instance, Type? declaringType,
		ParameterExpression? source, string separator)
	{
		int length = path.Length;
		if (instance is null)
		{
			Formatter.Format(path, declaringType);
		}
		else if (!IsCapturing(instance))
		{
			AppendPath(path, instance, source);
		}

		if (path.Length > length)
		{
			path.Append(separator);
		}
	}

	/// <summary>
	///     Whether the <paramref name="expression" /> is the closure of a captured variable or the instance the
	///     expression was created in.
	/// </summary>
	/// <remarks>
	///     Both are constants that are no literals, or fields of a closure that the compiler names with characters
	///     that are not valid in an identifier.
	/// </remarks>
	private static bool IsCapturing(Expression expression)
		=> expression is ConstantExpression { Value: not (null or IConvertible), } ||
		   (expression is MemberExpression { Expression: { } instance, Member: FieldInfo field, } &&
		    field.Name.IndexOf('<') >= 0 && IsCapturing(instance));

	private sealed class CapturedVariableNames : ExpressionVisitor
	{
		public static readonly CapturedVariableNames Instance = new();

		protected override Expression VisitMember(MemberExpression node)
			=> node.Expression is not null && IsCapturing(node.Expression)
				? Expression.Variable(node.Type, node.Member.Name)
				: base.VisitMember(node);
	}
}
