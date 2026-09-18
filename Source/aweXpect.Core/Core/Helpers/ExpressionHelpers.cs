using System.Linq.Expressions;
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

	public static string GetMemberPath(Expression expression)
	{
		MemberExpression? memberExpression = GetMemberExpression(expression);
		StringBuilder path = new();
		while (memberExpression != null)
		{
			if (path.Length > 0)
			{
				path.Insert(0, ".");
			}

			path.Insert(0, memberExpression.Member.Name);
			memberExpression = GetMemberExpression(memberExpression.Expression);
		}

		return path.ToString();
	}
}
