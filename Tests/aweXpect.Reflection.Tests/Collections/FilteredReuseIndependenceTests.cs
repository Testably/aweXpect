using System.Reflection;
using aweXpect.Reflection.Collections;

// ReSharper disable UnusedMember.Local
// ReSharper disable EventNeverSubscribedTo.Local
#pragma warning disable CS0067 // Event is never used
#pragma warning disable CS0649 // Field is never assigned to

namespace aweXpect.Reflection.Tests;

/// <summary>
///     Verifies that the filtered collections behave as immutable value objects: deriving two divergent
///     filtered views from the same base collection must not corrupt the base nor each other.
/// </summary>
public sealed class FilteredReuseIndependenceTests
{
	[Fact]
	public async Task Assemblies_BranchingFromSharedBase_ShouldBeIndependent()
	{
		Assembly assembly1 = typeof(In).Assembly;
		Assembly assembly2 = typeof(FilteredReuseIndependenceTests).Assembly;
		Filtered.Assemblies @base = In.Assemblies(assembly1, assembly2);

		Filtered.Assemblies view1 = @base.Which(a => a == assembly1);
		Filtered.Assemblies view2 = @base.Which(a => a == assembly2);

		await That(view1).IsEqualTo([assembly1,]).InAnyOrder();
		await That(view2).IsEqualTo([assembly2,]).InAnyOrder();
		await That(@base).IsEqualTo([assembly1, assembly2,]).InAnyOrder();
	}

	[Fact]
	public async Task Constructors_BranchingFromSharedBase_ShouldBeIndependent()
	{
		Filtered.Constructors @base = In.Type<SampleWithMembers>().Constructors();

		Filtered.Constructors view1 = @base.Which(c => c.GetParameters().Length == 0);
		Filtered.Constructors view2 = @base.Which(c => c.GetParameters().Length == 1);

		await That(view1).HasCount().AtLeast(1);
		await That(view1).All().Satisfy(c => c.GetParameters().Length == 0);
		await That(view2).HasCount().AtLeast(1);
		await That(view2).All().Satisfy(c => c.GetParameters().Length == 1);
		await That(@base).HasCount().AtLeast(2);
	}

	[Fact]
	public async Task Events_BranchingFromSharedBase_ShouldBeIndependent()
	{
		EventInfo eventA = typeof(SampleWithMembers).GetEvent(nameof(SampleWithMembers.EventA))!;
		EventInfo eventB = typeof(SampleWithMembers).GetEvent(nameof(SampleWithMembers.EventB))!;
		Filtered.Events @base = In.Type<SampleWithMembers>().Events();

		Filtered.Events view1 = @base.Which(e => e.Name == eventA.Name);
		Filtered.Events view2 = @base.Which(e => e.Name == eventB.Name);

		await That(view1).IsEqualTo([eventA,]).InAnyOrder();
		await That(view2).IsEqualTo([eventB,]).InAnyOrder();
		await That(@base).HasCount().AtLeast(2);
	}

	[Fact]
	public async Task Fields_BranchingFromSharedBase_ShouldBeIndependent()
	{
		FieldInfo fieldA = typeof(SampleWithMembers).GetField(nameof(SampleWithMembers.FieldA))!;
		FieldInfo fieldB = typeof(SampleWithMembers).GetField(nameof(SampleWithMembers.FieldB))!;
		Filtered.Fields @base = In.Type<SampleWithMembers>().Fields();

		Filtered.Fields view1 = @base.Which(f => f.Name == fieldA.Name);
		Filtered.Fields view2 = @base.Which(f => f.Name == fieldB.Name);

		await That(view1).IsEqualTo([fieldA,]).InAnyOrder();
		await That(view2).IsEqualTo([fieldB,]).InAnyOrder();
		await That(@base).HasCount().AtLeast(2);
	}

	[Fact]
	public async Task Methods_BranchingFromSharedBase_ShouldBeIndependent()
	{
		MethodInfo methodA = typeof(SampleWithMembers).GetMethod(nameof(SampleWithMembers.DoA))!;
		MethodInfo methodB = typeof(SampleWithMembers).GetMethod(nameof(SampleWithMembers.DoB))!;
		Filtered.Methods @base = In.Type<SampleWithMembers>().Methods()
			.Which(m => m.Name.StartsWith("Do"));

		Filtered.Methods view1 = @base.Which(m => m.Name == methodA.Name);
		Filtered.Methods view2 = @base.Which(m => m.Name == methodB.Name);

		await That(view1).IsEqualTo([methodA,]).InAnyOrder();
		await That(view2).IsEqualTo([methodB,]).InAnyOrder();
		await That(@base).IsEqualTo([methodA, methodB,]).InAnyOrder();
	}

	[Fact]
	public async Task Properties_BranchingFromSharedBase_ShouldBeIndependent()
	{
		PropertyInfo propertyA = typeof(SampleWithMembers).GetProperty(nameof(SampleWithMembers.PropA))!;
		PropertyInfo propertyB = typeof(SampleWithMembers).GetProperty(nameof(SampleWithMembers.PropB))!;
		Filtered.Properties @base = In.Type<SampleWithMembers>().Properties();

		Filtered.Properties view1 = @base.Which(p => p.Name == propertyA.Name);
		Filtered.Properties view2 = @base.Which(p => p.Name == propertyB.Name);

		await That(view1).IsEqualTo([propertyA,]).InAnyOrder();
		await That(view2).IsEqualTo([propertyB,]).InAnyOrder();
		await That(@base).HasCount().AtLeast(2);
	}

	[Fact]
	public async Task Types_BranchingFromSharedBase_ShouldBeIndependent()
	{
		Filtered.Types @base = In.Types<SampleType1, SampleType2>();

		Filtered.Types view1 = @base.Which(t => t == typeof(SampleType1));
		Filtered.Types view2 = @base.Which(t => t == typeof(SampleType2));

		await That(view1).IsEqualTo([typeof(SampleType1),]).InAnyOrder();
		await That(view2).IsEqualTo([typeof(SampleType2),]).InAnyOrder();
		await That(@base).IsEqualTo([typeof(SampleType1), typeof(SampleType2),]).InAnyOrder();
	}

	[Fact]
	public async Task MethodsWhichAreGenericWithArgumentCount_BranchingFromSharedBase_ShouldBeIndependent()
	{
		MethodFilters.GenericMethods @base = In.Type<SampleGenericMethods>().Methods()
			.WhichAreGeneric();

		Filtered.Methods view1 = @base.WithArgumentCount(1);
		Filtered.Methods view2 = @base.WithArgumentCount(2);

		await That(view1).All().Satisfy(m => m.GetGenericArguments().Length == 1);
		await That(view1).HasCount(1);
		await That(view2).All().Satisfy(m => m.GetGenericArguments().Length == 2);
		await That(view2).HasCount(1);
		await That(@base).All().Satisfy(m => m.IsGenericMethod);
		await That(@base).HasCount(2);
	}

	[Fact]
	public async Task MethodsWhichAreGenericWithArgument_BranchingFromSharedBase_ShouldBeIndependent()
	{
		MethodFilters.GenericMethods @base = In.Type<SampleGenericArgumentMethods>().Methods()
			.WhichAreGeneric();

		Filtered.Methods view1 = @base.WithArgument<BaseA>();
		Filtered.Methods view2 = @base.WithArgument<BaseB>();

		await That(view1).IsEqualTo([
			typeof(SampleGenericArgumentMethods).GetMethod(nameof(SampleGenericArgumentMethods.ArgumentOfBaseA))!,
		]).InAnyOrder();
		await That(view2).IsEqualTo([
			typeof(SampleGenericArgumentMethods).GetMethod(nameof(SampleGenericArgumentMethods.ArgumentOfBaseB))!,
		]).InAnyOrder();
		await That(@base).HasCount(2);
	}

	[Fact]
	public async Task TypesWhichAreGenericWithArgumentCount_BranchingFromSharedBase_ShouldBeIndependent()
	{
		TypeFilters.GenericTypes @base = In.Types<SampleGenericType1<int>, SampleGenericType2<int, int>>()
			.WhichAreGeneric();

		Filtered.Types view1 = @base.WithArgumentCount(1);
		Filtered.Types view2 = @base.WithArgumentCount(2);

		await That(view1).IsEqualTo([typeof(SampleGenericType1<int>),]).InAnyOrder();
		await That(view2).IsEqualTo([typeof(SampleGenericType2<int, int>),]).InAnyOrder();
		await That(@base).IsEqualTo([
			typeof(SampleGenericType1<int>), typeof(SampleGenericType2<int, int>),
		]).InAnyOrder();
	}

	private class BaseA;

	private class BaseB;

	private class SampleGenericMethods
	{
		public static void OneArgument<T1>()
		{
		}

		public static void TwoArguments<T1, T2>()
		{
		}
	}

	private class SampleGenericArgumentMethods
	{
		public static void ArgumentOfBaseA<T>() where T : BaseA
		{
		}

		public static void ArgumentOfBaseB<T>() where T : BaseB
		{
		}
	}

	private class SampleGenericType1<T1>;

	private class SampleGenericType2<T1, T2>;

	private class SampleType1;

	private class SampleType2;

	private class SampleWithMembers
	{
		public int FieldA;
		public int FieldB;

		public SampleWithMembers()
		{
		}

		public SampleWithMembers(int value)
		{
			FieldA = value;
		}

		public event EventHandler? EventA;
		public event EventHandler? EventB;

		public int PropA { get; set; }
		public int PropB { get; set; }

		public static void DoA()
		{
		}

		public static void DoB()
		{
		}
	}
}
