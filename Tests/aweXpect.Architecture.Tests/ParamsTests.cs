namespace aweXpect.Architecture.Tests;

public sealed class ParamsTests
{
	[Test]
	public async Task Constructors_ShouldTakeParamsArraysInsteadOfParamsCollections()
		=> await That(Packages.PublicTypes(Packages.All).Public.Constructors().WithParamsParameter())
			.HaveParamsParameter().Matching(p => p.ParameterType.IsArray)
			.Because("a compiler below C# 13 ignores a params collection, so callers could not pass single values");

	[Test]
	public async Task Methods_ShouldTakeParamsArraysInsteadOfParamsCollections()
		=> await That(Packages.PublicTypes(Packages.All).Public.Methods().WithParamsParameter())
			.HaveParamsParameter().Matching(p => p.ParameterType.IsArray)
			.Because("a compiler below C# 13 ignores a params collection, so callers could not pass single values");
}
