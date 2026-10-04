// The teaser of an extension leaves out what "Write your own extension" shows in full.
global using aweXpect.Core;
global using aweXpect.Core.Constraints;
global using aweXpect.Core.Extending;
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
	public static Track track = new();

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

public class Track;

public sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<Track>(it, grammars),
		IValueConstraint<Track?>
{
	public ConstraintResult IsMetBy(Track? actual) => this;

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
}
