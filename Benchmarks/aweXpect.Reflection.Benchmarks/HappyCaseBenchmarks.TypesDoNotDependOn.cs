using System.Reflection;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify the architecture rule that no type in a namespace of an assembly depends on the
///     <c>System.Net</c> namespace.<br />
///     FluentAssertions has no dependency rules, so it collects the types of the fields, properties, method return values
///     and method parameters and verifies that none of them is in the namespace. This is not equivalent: it ignores
///     e.g. base types, interfaces, generic arguments and attributes.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task TypesDoNotDependOn_aweXpect()
		=> await Expect.That(In.AssemblyContaining<HappyCaseBenchmarks>().Types().WhichAreWithinNamespace(_namespace))
			.DoNotDependOn("System.Net");

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<Type>> TypesDoNotDependOn_FluentAssertions()
		=> AllTypes.From(_assembly).ThatAreUnderNamespace(_namespace).SelectMany(GetSignatureTypes).Should()
			.NotContain(type => type.Namespace != null &&
			                    (type.Namespace == "System.Net" || type.Namespace.StartsWith("System.Net.")));

	private static IEnumerable<Type> GetSignatureTypes(Type type)
		=> type.GetFields(DeclaredMembers).Select(field => field.FieldType)
			.Concat(type.GetProperties(DeclaredMembers).Select(property => property.PropertyType))
			.Concat(type.GetMethods(DeclaredMembers).SelectMany(method => method.GetParameters()
				.Select(parameter => parameter.ParameterType)
				.Append(method.ReturnType)));
}
