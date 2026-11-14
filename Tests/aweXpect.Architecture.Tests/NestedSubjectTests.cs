using System.Linq;
using aweXpect.Core;

namespace aweXpect.Architecture.Tests;

public sealed class NestedSubjectTests
{
	[Test]
	public async Task Constructors_ShouldHandOutIThatSubjectInsteadOfIThat()
		=> await That(Packages.PublicTypes(Packages.All).Public.Constructors())
			.DoNotHaveParameter<Delegate>().Matching(p => HandsOutIThat(p.ParameterType))
			.Because("a nested subject is an IThatSubject<T>, so that the type checks are available");

	[Test]
	public async Task Methods_ShouldHandOutIThatSubjectInsteadOfIThat()
		=> await That(Packages.PublicTypes(Packages.All).Public.Methods())
			.DoNotHaveParameter<Delegate>().Matching(p => HandsOutIThat(p.ParameterType))
			.Because("a nested subject is an IThatSubject<T>, so that the type checks are available");

	private static bool HandsOutIThat(Type delegateType)
		=> delegateType.GetMethod("Invoke")?.GetParameters().Any(parameter
			=> parameter.ParameterType.IsGenericType
			   && parameter.ParameterType.GetGenericTypeDefinition() == typeof(IThat<>)) == true;
}
