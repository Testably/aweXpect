using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

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
	[Test]
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

	[Test]
	public async Task Constructors_BranchingFromSharedBase_ShouldBeIndependent()
	{
		Filtered.Constructors @base = In.Type<SampleWithMembers>().Constructors();

		Filtered.Constructors view1 = @base.Which(c => c.GetParameters().Length == 0);
		Filtered.Constructors view2 = @base.Which(c => c.GetParameters().Length == 1);

		await That(view1).HasCount().GreaterThanOrEqualTo(1);
		await That(view1).All().Satisfy(c => c.GetParameters().Length == 0);
		await That(view2).HasCount().GreaterThanOrEqualTo(1);
		await That(view2).All().Satisfy(c => c.GetParameters().Length == 1);
		await That(@base).HasCount().GreaterThanOrEqualTo(2);
	}

	[Test]
	public async Task ConstructorsWithInParameter_WhenAStringOptionIsAdded_ShouldKeepTheModifier()
	{
		ConstructorFilters.ConstructorsWithNamedParameter<object?> stored = In.Type<SampleWithParameters>()
			.Constructors().WithInParameter("VALUE");

		Filtered.Constructors continuation = stored.IgnoringCase();

		await That(stored).IsEmpty();
		await That(continuation).IsEqualTo([
			typeof(SampleWithParameters).GetConstructor([typeof(int).MakeByRefType(),])!,
		]);
	}

	[Test]
	public async Task ConstructorsWithParameter_BranchingFromSharedBase_ShouldBeIndependent()
	{
		ConstructorInfo inConstructor = typeof(SampleWithParameters).GetConstructor([typeof(int).MakeByRefType(),])!;
		ConstructorInfo valueConstructor = typeof(SampleWithParameters).GetConstructor([typeof(int), typeof(string),])!;
		ConstructorFilters.ConstructorsWithParameter<int> @base = In.Type<SampleWithParameters>()
			.Constructors().WithParameter<int>();

		Filtered.Constructors view1 = @base.WithInModifier();
		Filtered.Constructors view2 = @base.Matching(p => !p.IsIn);

		await That(view1).IsEqualTo([inConstructor,]);
		await That(view2).IsEqualTo([valueConstructor,]);
		await That(@base).IsEqualTo([inConstructor, valueConstructor,]).InAnyOrder();
	}

	[Test]
	public async Task Events_BranchingFromSharedBase_ShouldBeIndependent()
	{
		EventInfo eventA = typeof(SampleWithMembers).GetEvent(nameof(SampleWithMembers.EventA))!;
		EventInfo eventB = typeof(SampleWithMembers).GetEvent(nameof(SampleWithMembers.EventB))!;
		Filtered.Events @base = In.Type<SampleWithMembers>().Events();

		Filtered.Events view1 = @base.Which(e => e.Name == eventA.Name);
		Filtered.Events view2 = @base.Which(e => e.Name == eventB.Name);

		await That(view1).IsEqualTo([eventA,]).InAnyOrder();
		await That(view2).IsEqualTo([eventB,]).InAnyOrder();
		await That(@base).HasCount().GreaterThanOrEqualTo(2);
	}

	[Test]
	public async Task Fields_BranchingFromSharedBase_ShouldBeIndependent()
	{
		FieldInfo fieldA = typeof(SampleWithMembers).GetField(nameof(SampleWithMembers.FieldA))!;
		FieldInfo fieldB = typeof(SampleWithMembers).GetField(nameof(SampleWithMembers.FieldB))!;
		Filtered.Fields @base = In.Type<SampleWithMembers>().Fields();

		Filtered.Fields view1 = @base.Which(f => f.Name == fieldA.Name);
		Filtered.Fields view2 = @base.Which(f => f.Name == fieldB.Name);

		await That(view1).IsEqualTo([fieldA,]).InAnyOrder();
		await That(view2).IsEqualTo([fieldB,]).InAnyOrder();
		await That(@base).HasCount().GreaterThanOrEqualTo(2);
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task MethodsWhichAreGenericWithNamedArgument_WhenAStringOptionIsAdded_ShouldKeepTheMatchesOfTheFilterItWasCalledOn()
	{
		MethodFilters.GenericMethodsWithNamedArgument stored = In.Type<SampleGenericMethods>()
			.Methods().WhichAreGeneric().WithArgument("t2");

		Filtered.Methods continuation = stored.IgnoringCase();

		await That(stored).IsEmpty();
		await That(continuation).IsEqualTo([
			typeof(SampleGenericMethods).GetMethod(nameof(SampleGenericMethods.TwoArguments))!,
		]);
	}

	[Test]
	public async Task MethodsWithInParameter_WhenAStringOptionIsAdded_ShouldKeepTheModifier()
	{
		MethodFilters.MethodsWithNamedParameter<object?> stored = In.Type<SampleWithParameters>()
			.Methods().WithInParameter("VALUE");

		Filtered.Methods continuation = stored.IgnoringCase();

		await That(stored).IsEmpty();
		await That(continuation).IsEqualTo([
			typeof(SampleWithParameters).GetMethod(nameof(SampleWithParameters.WithInValue))!,
		]);
	}

	[Test]
	public async Task MethodsWithNamedParameter_WhenAStringOptionIsAdded_ShouldKeepTheMatchesOfTheFilterItWasCalledOn()
	{
		MethodFilters.MethodsWithNamedParameter<int> stored = In.Type<SampleWithParameters>()
			.Methods().WithParameter<int>("VALUE");

		Filtered.Methods continuation = stored.IgnoringCase();

		await That(stored).IsEmpty();
		await That(continuation).IsEqualTo([
			typeof(SampleWithParameters).GetMethod(nameof(SampleWithParameters.WithInValue))!,
			typeof(SampleWithParameters).GetMethod(nameof(SampleWithParameters.WithValue))!,
		]).InAnyOrder();
	}

	[Test]
	public async Task MethodsWithParameter_BranchingFromSharedBase_ShouldBeIndependent()
	{
		MethodInfo inMethod = typeof(SampleWithParameters).GetMethod(nameof(SampleWithParameters.WithInValue))!;
		MethodInfo valueMethod = typeof(SampleWithParameters).GetMethod(nameof(SampleWithParameters.WithValue))!;
		MethodFilters.MethodsWithParameter<int> @base = In.Type<SampleWithParameters>()
			.Methods().WithParameter<int>();

		Filtered.Methods view1 = @base.WithInModifier();
		Filtered.Methods view2 = @base.Matching(p => !p.IsIn);

		await That(view1).IsEqualTo([inMethod,]);
		await That(view2).IsEqualTo([valueMethod,]);
		await That(@base).IsEqualTo([inMethod, valueMethod,]).InAnyOrder();
	}

	[Test]
	public async Task Properties_BranchingFromSharedBase_ShouldBeIndependent()
	{
		PropertyInfo propertyA = typeof(SampleWithMembers).GetProperty(nameof(SampleWithMembers.PropA))!;
		PropertyInfo propertyB = typeof(SampleWithMembers).GetProperty(nameof(SampleWithMembers.PropB))!;
		Filtered.Properties @base = In.Type<SampleWithMembers>().Properties();

		Filtered.Properties view1 = @base.Which(p => p.Name == propertyA.Name);
		Filtered.Properties view2 = @base.Which(p => p.Name == propertyB.Name);

		await That(view1).IsEqualTo([propertyA,]).InAnyOrder();
		await That(view2).IsEqualTo([propertyB,]).InAnyOrder();
		await That(@base).HasCount().GreaterThanOrEqualTo(2);
	}

	[Test]
	[MethodDataSource(nameof(StringFilters))]
	public async Task StringFilter_WhenAStringOptionIsAdded_ShouldNotChangeTheFilterItWasCalledOn(string filter,
		Func<(IDescribableSubject Stored, Func<IDescribableSubject> AddOption, Action ChangeHolder)> create)
	{
		(IDescribableSubject stored, Func<IDescribableSubject> addOption, _) = create();
		string description = stored.GetDescription();

		IDescribableSubject continuation = addOption();

		await That(stored.GetDescription()).IsEqualTo(description)
			.Because($"{filter} must keep its options when a continuation adds an option");
		await That(continuation.GetDescription()).IsNotEqualTo(description);
	}

	[Test]
	[MethodDataSource(nameof(StringFilters))]
	public async Task StringFilter_WhenTheOptionsHolderIsChanged_ShouldUseTheNewOptions(string filter,
		Func<(IDescribableSubject Stored, Func<IDescribableSubject> AddOption, Action ChangeHolder)> create)
	{
		(IDescribableSubject stored, Func<IDescribableSubject> addOption, Action changeHolder) = create();
		string expected = addOption().GetDescription();

		changeHolder();

		await That(stored.GetDescription()).IsEqualTo(expected)
			.Because($"{filter} must read its options when it is evaluated, also when an option bypasses the copy hook");
	}

	[Test]
	public async Task StringFilterContinuation_ShouldAllowStringOptionsInAnyOrder()
	{
		Filtered.Types.StringEqualityResult stored = In.Types<SampleType1, SampleType2>().WithName("SAMPLE");

		Filtered.Types prefixFirst = stored.AsPrefix().IgnoringCase();
		Filtered.Types ignoringCaseFirst = stored.IgnoringCase().AsPrefix();

		await That(prefixFirst).IsEqualTo([typeof(SampleType1), typeof(SampleType2),]).InAnyOrder();
		await That(ignoringCaseFirst).IsEqualTo([typeof(SampleType1), typeof(SampleType2),]).InAnyOrder();
		await That(stored).IsEmpty();
	}

	[Test]
	public async Task StringFilterContinuation_WhenAMatchTypeIsSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		Filtered.Types.StringEqualityResult stored = In.Types<SampleType1, SampleType2>().WithName("Sample");

		void Act() => stored.AsPrefix().AsSuffix();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AsSuffix cannot be combined with AsPrefix.");
	}

	[Test]
	public async Task StringFilterContinuation_WhenAStringOptionIsSpecifiedTwice_ShouldUseTheLastValue()
	{
		Filtered.Types.StringEqualityResult stored = In.Types<SampleType1, SampleType2>().WithName("SAMPLE");

		Filtered.Types caseSensitive = stored.AsPrefix().IgnoringCase().IgnoringCase(false);
		Filtered.Types ignoringCase = stored.AsPrefix().IgnoringCase(false).IgnoringCase();

		await That(caseSensitive).IsEmpty();
		await That(ignoringCase).IsEqualTo([typeof(SampleType1), typeof(SampleType2),]).InAnyOrder();
		await That(stored).IsEmpty();
	}

	[Test]
	public async Task Types_BranchingFromSharedBase_ShouldBeIndependent()
	{
		Filtered.Types @base = In.Types<SampleType1, SampleType2>();

		Filtered.Types view1 = @base.Which(t => t == typeof(SampleType1));
		Filtered.Types view2 = @base.Which(t => t == typeof(SampleType2));

		await That(view1).IsEqualTo([typeof(SampleType1),]).InAnyOrder();
		await That(view2).IsEqualTo([typeof(SampleType2),]).InAnyOrder();
		await That(@base).IsEqualTo([typeof(SampleType1), typeof(SampleType2),]).InAnyOrder();
	}

	[Test]
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

	[Test]
	public async Task TypesWhichDependOn_WidenedWithOrOn_ShouldNotAffectBranchedView()
	{
		const string layer1 = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		const string layer2 = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";
		const string consumers = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";

		Filtered.Types.NamespaceDependencyFilterResult @base = Types.InNamespace(consumers).WhichDependOn(layer1);

		// Branch a separate view off the base, then widen the base. The branch must not see the widening.
		Filtered.Types branch = @base.Which(_ => true);
		Filtered.Types widened = @base.OrOn(layer2);

		await That(branch).Contains(typeof(OnlyLayer1));
		await That(branch).DoesNotContain(typeof(OnlyLayer2));
		await That(widened).Contains(typeof(OnlyLayer1));
		await That(widened).Contains(typeof(OnlyLayer2));
	}

	[Test]
	public async Task TypesWithName_WhenAStringOptionIsAdded_ShouldKeepTheMatchesOfTheFilterItWasCalledOn()
	{
		Filtered.Types.StringEqualityResult stored = In.Types<SampleType1, SampleType2>().WithName("SAMPLETYPE1");

		Filtered.Types continuation = stored.IgnoringCase();

		await That(stored).IsEmpty();
		await That(continuation).IsEqualTo([typeof(SampleType1),]);
	}

	public static IEnumerable<(string, Func<(IDescribableSubject, Func<IDescribableSubject>, Action)>)> StringFilters()
	{
		Filtered.Assemblies assemblies = In.AssemblyContaining<FilteredReuseIndependenceTests>();
		Filtered.Types types = In.Types<SampleType1, SampleType2>();
		Filtered.Constructors constructors = In.Type<SampleWithParameters>().Constructors();
		Filtered.Methods methods = In.Type<SampleWithParameters>().Methods();

		yield return Case("Assemblies.WhichDependOn", () => assemblies.WhichDependOn("x"));
		yield return Case("Assemblies.WhichDependOnlyOn", () => assemblies.WhichDependOnlyOn("x"));
		yield return Case("Assemblies.WhichDoNotDependOn", () => assemblies.WhichDoNotDependOn("x"));
		yield return Case("Assemblies.WhichDoNotTarget", () => assemblies.WhichDoNotTarget("x"));
		yield return Case("Assemblies.WhichTarget", () => assemblies.WhichTarget("x"));
		yield return Case("Assemblies.WithName", () => assemblies.WithName("x"));
		yield return Case("Assemblies.WithoutName", () => assemblies.WithoutName("x"));
		yield return Case("Constructors.WithInParameter", () => constructors.WithInParameter("x"));
		yield return Case("Constructors.WithOptionalParameter", () => constructors.WithOptionalParameter("x"));
		yield return Case("Constructors.WithOutParameter", () => constructors.WithOutParameter("x"));
		yield return Case("Constructors.WithParameter", () => constructors.WithParameter("x"));
		yield return Case("Constructors.WithParameter<T>", () => constructors.WithParameter<int>("x"));
		yield return Case("Constructors.WithParameter(Type)", () => constructors.WithParameter(typeof(int), "x"));
		yield return Case("Constructors.WithParameterExactly<T>", () => constructors.WithParameterExactly<int>("x"));
		yield return Case("Constructors.WithParameterExactly(Type)",
			() => constructors.WithParameterExactly(typeof(int), "x"));
		yield return Case("Constructors.WithParamsParameter", () => constructors.WithParamsParameter("x"));
		yield return Case("Constructors.WithRefParameter", () => constructors.WithRefParameter("x"));
		yield return Case("Constructors.WithoutParameter", () => constructors.WithoutParameter("x"));
		yield return Case("Constructors.WithoutParameter<T>", () => constructors.WithoutParameter<int>("x"));
		yield return Case("Constructors.WithoutParameter(Type)",
			() => constructors.WithoutParameter(typeof(int), "x"));
		yield return Case("Events.WithName", () => In.Type<SampleWithMembers>().Events().WithName("x"));
		yield return Case("Events.WithoutName", () => In.Type<SampleWithMembers>().Events().WithoutName("x"));
		yield return Case("Fields.WithName", () => In.Type<SampleWithMembers>().Fields().WithName("x"));
		yield return Case("Fields.WithoutName", () => In.Type<SampleWithMembers>().Fields().WithoutName("x"));
		yield return Case("Methods.WhichAreGeneric.WithArgument",
			() => In.Type<SampleGenericMethods>().Methods().WhichAreGeneric().WithArgument("x"));
		yield return Case("Methods.WhichAreGeneric.WithArgument<T>",
			() => In.Type<SampleGenericMethods>().Methods().WhichAreGeneric().WithArgument<object>("x"));
		yield return Case("Methods.WithInParameter", () => methods.WithInParameter("x"));
		yield return Case("Methods.WithName", () => methods.WithName("x"));
		yield return Case("Methods.WithOptionalParameter", () => methods.WithOptionalParameter("x"));
		yield return Case("Methods.WithOutParameter", () => methods.WithOutParameter("x"));
		yield return Case("Methods.WithParameter", () => methods.WithParameter("x"));
		yield return Case("Methods.WithParameter<T>", () => methods.WithParameter<int>("x"));
		yield return Case("Methods.WithParameter(Type)", () => methods.WithParameter(typeof(int), "x"));
		yield return Case("Methods.WithParameterExactly<T>", () => methods.WithParameterExactly<int>("x"));
		yield return Case("Methods.WithParameterExactly(Type)", () => methods.WithParameterExactly(typeof(int), "x"));
		yield return Case("Methods.WithParamsParameter", () => methods.WithParamsParameter("x"));
		yield return Case("Methods.WithRefParameter", () => methods.WithRefParameter("x"));
		yield return Case("Methods.WithoutName", () => methods.WithoutName("x"));
		yield return Case("Methods.WithoutParameter", () => methods.WithoutParameter("x"));
		yield return Case("Methods.WithoutParameter<T>", () => methods.WithoutParameter<int>("x"));
		yield return Case("Methods.WithoutParameter(Type)", () => methods.WithoutParameter(typeof(int), "x"));
		yield return Case("Properties.WithName", () => In.Type<SampleWithMembers>().Properties().WithName("x"));
		yield return Case("Properties.WithoutName",
			() => In.Type<SampleWithMembers>().Properties().WithoutName("x"));
		yield return Case("Types.WhichAreGeneric.WithArgument",
			() => types.WhichAreGeneric().WithArgument("x"));
		yield return Case("Types.WhichAreGeneric.WithArgument<T>",
			() => types.WhichAreGeneric().WithArgument<object>("x"));
		yield return Case("Types.WithName", () => types.WithName("x"));
		yield return Case("Types.WithNamespace", () => types.WithNamespace("x"));
		yield return Case("Types.WithoutName", () => types.WithoutName("x"));
		yield return Case("Types.WithoutNamespace", () => types.WithoutNamespace("x"));
	}

	private static (string, Func<(IDescribableSubject, Func<IDescribableSubject>, Action)>) Case<TResult>(string filter,
		Func<TResult> create)
		where TResult : IDescribableSubject, IOptionsProvider<StringEqualityOptions>
		=> (filter, () =>
		{
			TResult stored = create();
			return (stored, () => stored.IgnoringCase(), () => stored.Options.IgnoringCase());
		});

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

	private class SampleWithParameters
	{
		public SampleWithParameters(string value)
		{
		}

		public SampleWithParameters(in int value)
		{
		}

		public SampleWithParameters(int value, string other)
		{
		}

		public static void WithInValue(in int value)
		{
		}

		public static void WithValue(int value)
		{
		}
	}

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

		public int PropA { get; set; }
		public int PropB { get; set; }

		public event EventHandler? EventA;
		public event EventHandler? EventB;

		public static void DoA()
		{
		}

		public static void DoB()
		{
		}
	}
}
