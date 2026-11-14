using aweXpect.Reflection.Benchmarks.Subjects;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify the architecture rule that only the types in a namespace of an assembly use a type.
///     <br />
///     FluentAssertions has no dependency rules, so it collects the types of the fields, properties, method return values
///     and method parameters of all other types in the assembly and verifies that none of them is the type. This is not
///     equivalent: it ignores e.g. base types, interfaces, generic arguments and attributes.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task TypeIsOnlyUsedBy_aweXpect()
		=> await Expect.That(typeof(Album))
			.IsOnlyUsedBy(In.AssemblyContaining<HappyCaseBenchmarks>().Types().WhichAreWithinNamespace(_namespace))
			.Within(In.AssemblyContaining<HappyCaseBenchmarks>().Types());

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<Type>> TypeIsOnlyUsedBy_FluentAssertions()
		=> AllTypes.From(_assembly).ThatAreNotUnderNamespace(_namespace).SelectMany(GetSignatureTypes).Should()
			.NotContain(typeof(Album));
}
