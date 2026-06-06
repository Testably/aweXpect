// ReSharper disable All

#pragma warning disable CS0169 // field is never used
#pragma warning disable CS0649 // field is never assigned
#pragma warning disable CS8618 // non-nullable field must contain a non-null value

using System.Collections.Generic;
using System.Text;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Acyclic.Low;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Family.Inner;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.GlobalRing;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Api;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Domain;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Other;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Parent.Child;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Resolver.B;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part2;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.A;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.B;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.C;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Billing;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Orders;

// Fixtures for namespace dependency cycle detection. Each type references the next via a private field, which is a
// signature-level dependency, so the namespaces form the intended directed graph.

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Acyclic.High
{
	public class HighType
	{
		private LowType _low;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Acyclic.Low
{
	public class LowType;
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Orders
{
	public class Order
	{
		private Invoice _invoice;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Billing
{
	public class Invoice
	{
		private Order _order;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.A
{
	public class TypeA
	{
		private TypeB _b;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.B
{
	public class TypeB
	{
		private TypeC _c;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.C
{
	public class TypeC
	{
		private TypeA _a;
	}
}

// Two sub-namespaces of the same module reference each other: a cycle per-namespace, but collapsed into a single
// slice (no cycle) when grouped under the "Sliced" root.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part1
{
	public class Part1Type
	{
		private Part2Type _other;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part2
{
	public class Part2Type
	{
		private Part1Type _other;
	}
}

// Three namespaces forming a cycle that collapses to a TWO-slice cycle (Orders <-> Billing) when grouped under the
// "Grouped" root, because Orders.Domain and Orders.Api share the "Orders" slice.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Domain
{
	public class OrderDomain
	{
		private Billing.Invoice _invoice;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Api
{
	public class OrderApi
	{
		private OrderDomain _domain;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Billing
{
	public class Invoice
	{
		private OrderApi _api;
	}
}

// Only references framework types, so it never forms an edge to another node in the analyzed set.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.External
{
	public class Service
	{
		private StringBuilder _builder;
		private List<int> _values;
	}
}

// A signature-level edge A -> B exists (ResolverA references ResolverB via a field), but the reverse edge
// B -> A only exists at the body level, which the signature resolver cannot see. A custom resolver can supply
// it, closing the cycle.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Resolver.A
{
	public class ResolverA
	{
		private ResolverB _b;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Resolver.B
{
	public class ResolverB;
}

// A namespaced type and a global-namespace type that reference each other, so the cycle path renders the
// global-namespace node as "<global namespace>".
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.GlobalRing
{
	public class GlobalRingType
	{
		private GlobalCycleType _global;
	}
}

// A parent namespace and one of its sub-namespaces reference each other: by default they are one family (no cycle),
// but with ExcludingSubNamespaces() every namespace is its own node, so they form a cycle.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Family
{
	public class FamilyParent
	{
		private FamilyChild _child;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Family.Inner
{
	public class FamilyChild
	{
		private FamilyParent _parent;
	}
}

// An indirect family cycle: Parent (with a type directly in it) references Other, and Other references a type in the
// sub-namespace Parent.Child. By default Parent and Parent.Child are one node, so this forms a Parent <-> Other cycle
// that a mere edge-suppression (instead of node-merging) would miss; ExcludingSubNamespaces() splits the family again,
// so the back-reference lands on a different node and no cycle remains.
namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Parent
{
	public class IndirectParentType
	{
		private IndirectOtherType _other;
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Parent.Child
{
	public class IndirectChildType;
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Other
{
	public class IndirectOtherType
	{
		private IndirectChildType _child;
	}
}

#pragma warning disable CA1050 // Declare types in namespaces

// This type intentionally lives in the global namespace to exercise global-namespace cycle rendering.
public class GlobalCycleType
{
	private GlobalRingType _ring;
}
