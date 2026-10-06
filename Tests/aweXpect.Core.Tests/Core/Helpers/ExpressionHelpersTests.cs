using System.Linq.Expressions;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class ExpressionHelpersTests
{
	[Test]
	public async Task GetMemberExpression_FromArrayLength_ShouldReturnNull()
	{
		Expression<Func<int[], int>> expression = s => s.Length;

		MemberExpression? memberExpression = expression.GetMemberExpression();

		await That(memberExpression).IsNull();
	}

	[Test]
	public async Task GetMemberExpression_FromLambdaExpression_ShouldReturnMemberExpression()
	{
		Expression<Func<MyClass, int>> expression = s => s.Foo;

		MemberExpression? memberExpression = expression.GetMemberExpression();

		await That(memberExpression!.Member.Name).IsEqualTo("Foo");
	}

	[Test]
	public async Task GetMemberExpression_FromLambdaExpressionWithUnaryExpressionBody_ShouldReturnMemberExpression()
	{
		Expression<Func<MyClass, int?>> expression = s => s.Foo;

		MemberExpression? memberExpression = expression.GetMemberExpression();

		await That(memberExpression!.Member.Name).IsEqualTo("Foo");
	}

	[Test]
	public async Task GetMemberExpression_FromMemberExpression_ShouldReturnSameObject()
	{
		MyClass subject = new();
		MemberExpression expression = Expression.Property(Expression.Constant(subject), nameof(MyClass.Foo));

		MemberExpression? memberExpression = expression.GetMemberExpression();

		await That(memberExpression).IsSameAs(expression);
	}

	[Test]
	public async Task GetMemberExpression_Null_ShouldReturnNull()
	{
		Expression? expression = null;

		MemberExpression? memberExpression = expression.GetMemberExpression();

		await That(memberExpression).IsNull();
	}

	private sealed class MyClass
	{
		public int Foo { get; set; }
	}
}
