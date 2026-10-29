using System;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Aot.Domain;
using aweXpect.Reflection.Aot.Infrastructure;
using aweXpect.Reflection.Collections;
using static aweXpect.Expect;

namespace aweXpect.Reflection.Aot;

internal static class Checks
{
	private const string SwitchName = "aweXpect.ReflectionFallback.IsSupported";
	private const string DomainNamespace = "aweXpect.Reflection.Aot.Domain";
	private const string InfrastructureNamespace = "aweXpect.Reflection.Aot.Infrastructure";

	public static readonly Check[] All =
	[
		new("the reflection fallback is switched off exactly when published",
			() => Task.FromResult(ReflectionFallback.IsSupported == !Program.IsPublished
				? null
				: $"it was {(ReflectionFallback.IsSupported ? "on" : "off")}")),
		new("an assembly has its name",
			() => ShouldPass(async () => await That(typeof(Order).Assembly).HasName("aweXpect.Reflection.Aot"))),
		new("an assembly depends on a referenced assembly",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(typeof(Order).Assembly).DependsOn("aweXpect.Reflection"))),
		new("an assembly without the dependency fails",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(typeof(Order).Assembly).DependsOn("Missing"), "has a dependency on assembly")),
		new("a sealed type is sealed",
			() => ShouldPass(async () => await That(typeof(Order)).IsSealed())),
		new("an unsealed type fails to be sealed",
			() => ShouldFail(async () => await That(typeof(Customer)).IsSealed(), "is sealed")),
		new("a type implements its interface",
			() => ShouldPassOrFailLoudly(async () => await That(typeof(Order)).Implements(typeof(IEntity)))),
		new("a type without the interface fails",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(typeof(Customer)).Implements(typeof(IEntity)), "implements")),
		new("a type implements its interface directly",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(typeof(Order)).Implements(typeof(IEntity)).Directly())),
		new("a class is no record",
			() => ShouldPassOrFailLoudly(async () => await That(typeof(Order)).IsAClass())),
		new("a type contains its method",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(typeof(Order)).ContainsMethods(methods => methods.WithName(nameof(Order.Total)))
					.Exactly(1))),
		new("a type without the method fails",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(typeof(Order)).ContainsMethods(methods => methods.WithName("Missing")).AtLeast()
					.Once(), "contains")),
		new("a property found statically has a getter",
			() => ShouldPass(async ()
				=> await That(typeof(Order).GetProperty(nameof(Order.Id))).HasAGetter())),
		new("a nullable property is nullable",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(typeof(Customer).GetProperty(nameof(Customer.Nickname))).IsNullable())),
		new("a non-nullable property fails to be nullable",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(typeof(Order).GetProperty(nameof(Order.Customer))).IsNullable(),
				"it was non-nullable public Customer Order.Customer { get; }")),
		new("named types are sealed",
			() => ShouldPass(async () => await That(In.Types<Order, Repository>()).AreSealed())),
		new("the types of a namespace fail when one is not sealed",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(DomainTypes().WhichAreClasses()).AreSealed(), "Customer")),
		new("named types fail when one is sealed",
			() => ShouldFail(async () => await That(In.Types<Order, Customer>()).AreNotSealed(),
				"none are sealed", "Order")),
		new("exactly one of the named types implements the interface directly",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Types<Order, Customer>()).Exactly(1).Implement(typeof(IEntity)).Directly())),
		new("named types fail when too few implement the interface",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(In.Types<Order, Customer>()).AtLeast(2).Implement(typeof(IEntity)),
				"implements IEntity for at least 2 items")),
		new("the methods of a type are not abstract",
			() => ShouldPassOrFailLoudly(async () => await That(In.Type<Order>().Methods()).AreNotAbstract())),
		new("the declared and inherited methods of a type including the object members are not abstract",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Type<Order>().Methods().IncludingObjectMembers()).AreNotAbstract())),
		new("the methods of a type fail to be static and are listed",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(In.Type<Order>().Methods()).AreStatic(), "decimal Order.Total()")),
		new("the assemblies depend on a referenced assembly",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.AssemblyContaining<Order>()).DependOn("aweXpect.Reflection"))),
		new("the domain does not depend on the infrastructure",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(DomainTypes()).DoNotDependOn(InfrastructureTypes()))),
		new("the infrastructure fails to not depend on the domain",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(InfrastructureTypes()).DoNotDependOn(DomainTypes()), "Order")),
		new("a type depends on the type in its signature",
			() => ShouldPassOrFailLoudly(async () => await That(typeof(Repository)).DependsOn(typeof(Order)))),
		new("the infrastructure depends on the order type",
			() => ShouldPassOrFailLoudly(async () => await That(InfrastructureTypes()).DependOn<Order>())),
		new("the domain fails to not depend on the customer type",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(DomainTypes()).DoNotDependOn<Customer>(), "Order")),
		new("at least one of the infrastructure types depends on the domain",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(InfrastructureTypes()).AtLeast(1).DependOn(DomainTypes()))),
		new("the infrastructure fails when no type has to depend on the order type",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(InfrastructureTypes()).None().DependOn(typeof(Order)), "depends on type Order for no items")),
		new("the infrastructure types filtered by their dependency on the order type contain the repository",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(InfrastructureTypes().WhichDependOn<Order>()).Contains(typeof(Repository)))),
		new("the types filtered by an operator they do not have contain the order type",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Type<Order>().WhichDoNotHaveOperator(Operator.Addition)).Contains(typeof(Order)))),
		new("a property found statically is not of another type",
			() => ShouldPass(async ()
				=> await That(typeof(Order).GetProperty(nameof(Order.Id))).IsNotOfType<string>())),
		new("a method found statically does not return void",
			() => ShouldPass(async ()
				=> await That(typeof(Order).GetMethod(nameof(Order.Total))).DoesNotReturnVoid())),
		new("a method with the parameter fails to not have it",
			() => ShouldFail(async ()
				=> await That(typeof(Repository).GetMethod(nameof(Repository.Load))).DoesNotHaveParameter<int>(),
				"does not have parameter of type int")),
		new("a type does not have another namespace",
			() => ShouldPass(async () => await That(typeof(Order)).DoesNotHaveNamespace(InfrastructureNamespace))),
		new("an assembly does not target another framework",
			() => ShouldPassOrFailLoudly(async () => await That(typeof(Order).Assembly).DoesNotTarget("net48"))),
		new("the methods of a type do not return void",
			() => ShouldPassOrFailLoudly(async () => await That(In.Type<Order>().Methods()).DoNotReturnVoid())),
		new("at least one method of a type has the parameter",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Type<Repository>().Methods()).AtLeast(1).HaveParameter(typeof(int)).AtIndex(0))),
		new("the methods of a type fail when too few return void",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(In.Type<Order>().Methods()).AtLeast(1).Return(typeof(void)),
				"returns void for at least one item")),
		new("the properties of a type not of a type are of the other type",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Type<Order>().Properties().WhichAreNotOfType<int>()).AreOfType<Customer>())),
		new("exactly one property of a type is of the other type",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Type<Order>().Properties()).Exactly(1).AreOfType(typeof(Customer)))),
		new("none of the named types has the attribute",
			() => ShouldPassOrFailLoudly(async ()
				=> await That(In.Types<Order, Customer>()).None().Have(typeof(ObsoleteAttribute)))),
	];

	private static Filtered.Types DomainTypes()
		=> In.AssemblyContaining<Order>().Types().WhichAreWithinNamespace(DomainNamespace);

	private static Filtered.Types InfrastructureTypes()
		=> In.AssemblyContaining<Order>().Types().WhichAreWithinNamespace(InfrastructureNamespace);

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects the failure of the expectation whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}

	/// <remarks>
	///     Reflection over the members, interfaces, types or referenced assemblies of a subject is switched off when
	///     published, so the expectation either passes or fails with the error naming the switch. A failure for any
	///     other reason would be a silently degraded result and is not accepted.
	/// </remarks>
	private static async Task<string?> ShouldPassOrFailLoudly(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception) when (!ReflectionFallback.IsSupported &&
		                                  exception.Message.Contains(SwitchName, StringComparison.Ordinal))
		{
			return FailedLoudly(exception);
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <remarks>
	///     Like <see cref="ShouldPassOrFailLoudly" />, the expectation either fails like it does with reflection or
	///     with the error naming the switch; a pass would be a silently degraded result.
	/// </remarks>
	private static async Task<string?> ShouldFailOrFailLoudly(Func<Task> act, string part)
	{
		try
		{
			await act();
		}
		catch (FailException exception) when (exception.Message.Contains(part, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception) when (!ReflectionFallback.IsSupported &&
		                                  exception.Message.Contains(SwitchName, StringComparison.Ordinal))
		{
			return FailedLoudly(exception);
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} without the expected failure: {exception.Message}";
		}

		return "did not throw";
	}

	/// <summary>
	///     Accepts the failure naming the switch and reports it, so that the output shows which checks were verified
	///     by their result and which only by failing loudly.
	/// </summary>
	private static string? FailedLoudly(Exception exception)
	{
		Console.WriteLine($"  failed loudly with {exception.GetType().Name}: {exception.Message}");
		return null;
	}
}
