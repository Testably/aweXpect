using System.Collections.Generic;
using System.Linq;
using aweXpect.Customization;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Acyclic.High;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Acyclic.Low;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.External;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Family;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Family.Inner;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.GlobalRing;
using Xunit.Sdk;
using ResolverA = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Resolver.A;
using ResolverB = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Resolver.B;
using GroupedBilling = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Billing;
using GroupedOrdersApi = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Api;
using GroupedOrdersDomain = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Grouped.Orders.Domain;
using SlicedPart1 = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part1;
using SlicedPart2 = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Sliced.Module.Part2;
using ThreeA = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.A;
using ThreeB = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.B;
using ThreeC = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Three.C;
using TwoBilling = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Billing;
using TwoOrders = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Two.Orders;
using IndirectOther = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Other;
using IndirectParent = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Parent;
using IndirectParentChild = aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles.Indirect.Parent.Child;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class HaveNoDependencyCycles
	{
		private const string Prefix = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Cycles";

		public sealed class Tests
		{
			[Fact]
			public async Task WhenSetIsAcyclic_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(HighType),
					typeof(LowType),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCollectionContainsNull_ShouldIgnoreNull()
			{
				IEnumerable<Type?> subject =
				[
					typeof(HighType),
					null,
					typeof(LowType),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTwoNamespacesFormCycle_ShouldFailAndListBothNamespaces()
			{
				IEnumerable<Type?> subject =
				[
					typeof(TwoOrders.Order),
					typeof(TwoBilling.Invoice),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                {Prefix}.Two.Billing -> {Prefix}.Two.Orders -> {Prefix}.Two.Billing
					              """);
			}

			[Fact]
			public async Task WhenThreeNamespacesFormCycle_ShouldDetectTheLongerCycle()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ThreeA.TypeA),
					typeof(ThreeB.TypeB),
					typeof(ThreeC.TypeC),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                {Prefix}.Three.A -> {Prefix}.Three.B -> {Prefix}.Three.C -> {Prefix}.Three.A
					              """);
			}

			[Fact]
			public async Task WhenOnlyDependingOnFrameworkTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(Service),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUsingNamespaceSource_ShouldDetectCycle()
			{
				async Task Act()
					=> await That(In.Namespace($"{Prefix}.Two")).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage("*had a dependency cycle*").AsWildcard();
			}

			[Fact]
			public async Task WhenDependencyTargetIsOutOfSet_ShouldNotCreateAnEdge()
			{
				// Order references Two.Billing, but Billing is not part of the analyzed set, so no edge is created.
				IEnumerable<Type?> subject =
				[
					typeof(TwoOrders.Order),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMultipleCyclesExist_ShouldListAllOfThem()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ThreeA.TypeA),
					typeof(ThreeB.TypeB),
					typeof(ThreeC.TypeC),
					typeof(TwoOrders.Order),
					typeof(TwoBilling.Invoice),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had 2 dependency cycles:
					                {Prefix}.Three.A -> {Prefix}.Three.B -> {Prefix}.Three.C -> {Prefix}.Three.A
					                {Prefix}.Two.Billing -> {Prefix}.Two.Orders -> {Prefix}.Two.Billing
					              """);
			}

			[Fact]
			public async Task WhenGlobalNamespaceIsInACycle_ShouldRenderGlobalNamespace()
			{
				IEnumerable<Type?> subject =
				[
					typeof(GlobalCycleType),
					typeof(GlobalRingType),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                <global namespace> -> {Prefix}.GlobalRing -> <global namespace>
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSetHasCycle_ShouldSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(TwoOrders.Order),
					typeof(TwoBilling.Invoice),
				];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveNoDependencyCycles());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetIsAcyclic_ShouldFail()
			{
				IEnumerable<Type?> subject =
				[
					typeof(HighType),
					typeof(LowType),
				];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveNoDependencyCycles());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             have dependency cycles,
					             but it had none
					             """);
			}
		}

		public sealed class ResolverTests
		{
			[Fact]
			public async Task WithSignatureResolver_ABodyOnlyEdge_ShouldNotFormACycle()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ResolverA.ResolverA),
					typeof(ResolverB.ResolverB),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				// Only the signature edge A -> B is visible, so there is no cycle.
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAResolverSuppliesTheReverseEdge_ShouldDetectTheCycle()
			{
				IEnumerable<Type?> subject =
				[
					typeof(ResolverA.ResolverA),
					typeof(ResolverB.ResolverB),
				];

				ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> resolver =
					Customize.aweXpect.Reflection().DependencyResolver();
				Func<Type, IEnumerable<Type>> builtin = resolver.Get()!;

				// Simulate a body-only reference from B back to A, which the signature resolver cannot see.
				using (resolver.Set(type => type == typeof(ResolverB.ResolverB)
					       ? builtin(type).Concat([typeof(ResolverA.ResolverA),])
					       : builtin(type)))
				{
					async Task Act()
						=> await That(subject).HaveNoDependencyCycles();

					await That(Act).Throws<XunitException>()
						.WithMessage("*had a dependency cycle*").AsWildcard();
				}
			}
		}

		public sealed class SliceTests
		{
			private const string Prefix = HaveNoDependencyCycles.Prefix;

			[Fact]
			public async Task WhenSubNamespacesShareSlice_ShouldCollapseAndSucceed()
			{
				IEnumerable<Type?> subject =
				[
					typeof(SlicedPart1.Part1Type),
					typeof(SlicedPart2.Part2Type),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles($"{Prefix}.Sliced");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotGrouped_ShouldDetectTheSubNamespaceCycle()
			{
				IEnumerable<Type?> subject =
				[
					typeof(SlicedPart1.Part1Type),
					typeof(SlicedPart2.Part2Type),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                {Prefix}.Sliced.Module.Part1 -> {Prefix}.Sliced.Module.Part2 -> {Prefix}.Sliced.Module.Part1
					              """);
			}

			[Fact]
			public async Task WhenGroupingCollapsesNamespacesIntoSlices_ShouldReportSliceNames()
			{
				IEnumerable<Type?> subject =
				[
					typeof(GroupedOrdersDomain.OrderDomain),
					typeof(GroupedOrdersApi.OrderApi),
					typeof(GroupedBilling.Invoice),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles($"{Prefix}.Grouped");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles when grouped into slices under "{Prefix}.Grouped",
					              but it had a dependency cycle:
					                {Prefix}.Grouped.Billing -> {Prefix}.Grouped.Orders -> {Prefix}.Grouped.Billing
					              """);
			}
		}

		public sealed class SubNamespaceTests
		{
			private const string Prefix = HaveNoDependencyCycles.Prefix;

			[Fact]
			public async Task WhenParentAndChildReferenceEachOther_ShouldSucceedByDefault()
			{
				// By default a namespace and its sub-namespaces are one family, so the references do not form a cycle.
				IEnumerable<Type?> subject =
				[
					typeof(FamilyParent),
					typeof(FamilyChild),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_ShouldDetectTheFamilyCycle()
			{
				IEnumerable<Type?> subject =
				[
					typeof(FamilyParent),
					typeof(FamilyChild),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles().ExcludingSubNamespaces();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                {Prefix}.Family -> {Prefix}.Family.Inner -> {Prefix}.Family
					              """);
			}

			[Fact]
			public async Task WhenCycleReentersFamilyThroughSubNamespace_ShouldDetectItByDefault()
			{
				// Parent -> Other -> Parent.Child: because Parent and Parent.Child are one node by default, this is a
				// Parent <-> Other cycle. A mere edge-suppression (that did not merge the nodes) would miss it.
				IEnumerable<Type?> subject =
				[
					typeof(IndirectParent.IndirectParentType),
					typeof(IndirectParentChild.IndirectChildType),
					typeof(IndirectOther.IndirectOtherType),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              have no dependency cycles,
					              but it had a dependency cycle:
					                {Prefix}.Indirect.Other -> {Prefix}.Indirect.Parent -> {Prefix}.Indirect.Other
					              """);
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_TheReentryLandsOnAnotherNode_ShouldSucceed()
			{
				// With every namespace its own node, the back-reference lands on Parent.Child (not Parent), so the
				// path Parent -> Other -> Parent.Child dead-ends and there is no cycle.
				IEnumerable<Type?> subject =
				[
					typeof(IndirectParent.IndirectParentType),
					typeof(IndirectParentChild.IndirectChildType),
					typeof(IndirectOther.IndirectOtherType),
				];

				async Task Act()
					=> await That(subject).HaveNoDependencyCycles().ExcludingSubNamespaces();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
