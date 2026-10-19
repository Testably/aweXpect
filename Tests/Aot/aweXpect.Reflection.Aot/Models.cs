using aweXpect.Reflection.Aot.Domain;

namespace aweXpect.Reflection.Aot.Domain
{
	public interface IEntity
	{
		int Id { get; }
	}

	public sealed class Order(int id, Customer customer) : IEntity
	{
		public Customer Customer { get; } = customer;
		public int Id { get; } = id;

		public decimal Total() => 42m;
	}

	public class Customer
	{
		public string? Nickname { get; set; }
	}
}

namespace aweXpect.Reflection.Aot.Infrastructure
{
	public sealed class Repository
	{
		public Order Load(int id) => new(id, new Customer());
	}
}
