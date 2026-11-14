using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Snapshots.DiffEngine;
using aweXpect.Snapshots.PublicApi;

namespace aweXpect.Architecture.Tests;

public sealed class ParamsTests
{
	[Test]
	public async Task Constructors_ShouldTakeParamsArraysInsteadOfParamsCollections()
		=> await That(PublicTypes().Public.Constructors().WithParamsParameter())
			.HaveParamsParameter().Matching(p => p.ParameterType.IsArray)
			.Because("a compiler below C# 13 ignores a params collection, so callers could not pass single values");

	[Test]
	public async Task Methods_ShouldTakeParamsArraysInsteadOfParamsCollections()
		=> await That(PublicTypes().Public.Methods().WithParamsParameter())
			.HaveParamsParameter().Matching(p => p.ParameterType.IsArray)
			.Because("a compiler below C# 13 ignores a params collection, so callers could not pass single values");

	private static Filtered.Types PublicTypes()
		=> In.Assemblies(
				typeof(ThatBool).Assembly,
				typeof(Expect).Assembly,
				typeof(JsonExtensions).Assembly,
				typeof(ThatHttpRequestMessage).Assembly,
				typeof(ThatXElement).Assembly,
				typeof(In).Assembly,
				typeof(ThatSnapshot).Assembly,
				typeof(DiffEngineReporter).Assembly,
				typeof(PublicApiDeclarations).Assembly)
			.Types().Which(type => type.IsVisible);
}
