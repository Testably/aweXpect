// The teaser of an extension leaves out what "Write your own extension" shows in full.
global using aweXpect.Core;
global using aweXpect.Core.Constraints;
global using aweXpect.Results;
global using static Snippets.Prelude;
using System.Text;

namespace Snippets;

internal static class Prelude
{
	public static CancellationToken cancellationToken;
	public static string message = "";
	public static int result;
	public static User user = new();
	public static Guid id;
	public static Users _users = new();
	public static Orders _orders = new();
	public static Payments _payments = new();

	public static IExpectThat<T> Get<T>(this IThat<T> subject) => (IExpectThat<T>)subject;

	public class User
	{
		public bool IsActive { get; set; }
	}

	public class Users
	{
		public Task<User> GetAsync(Guid id) => Task.FromResult(new User());
	}

	public class Order
	{
		public bool IsPriority { get; set; }
	}

	public class Orders
	{
		public async IAsyncEnumerable<Order> StreamAsync()
		{
			await Task.Yield();
			yield return new Order();
		}
	}

	public class Payments
	{
		public Task ChargeAsync(decimal amount) => Task.CompletedTask;
	}
}

public sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<string>(it, grammars),
		IValueConstraint<string>
{
	public ConstraintResult IsMetBy(string actual) => this;

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
}
