using System;
using System.Threading.Tasks;
using aweXpect.Core;
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
				=> await That(typeof(Order).GetProperty(nameof(Order.Customer))).IsNullable(), "is nullable")),
		new("named types are sealed",
			() => ShouldPass(async () => await That(In.Types<Order, Repository>()).AreSealed())),
		new("the types of a namespace fail when one is not sealed",
			() => ShouldFailOrFailLoudly(async ()
				=> await That(DomainTypes().WhichAreClasses()).AreSealed(), "Customer")),
		new("the methods of a type are not abstract",
			() => ShouldPassOrFailLoudly(async () => await That(In.Type<Order>().Methods()).AreNotAbstract())),
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
	];

	private static Filtered.Types DomainTypes()
		=> In.AssemblyContaining<Order>().Types().WithinNamespace(DomainNamespace);

	private static Filtered.Types InfrastructureTypes()
		=> In.AssemblyContaining<Order>().Types().WithinNamespace(InfrastructureNamespace);

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
