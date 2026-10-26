using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Internal.Tests.Options;

public sealed class DependencyOptionsTests
{
	[Test]
	public async Task DependencyCyclesOptions_ExcludingSubNamespaces_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		DependencyCyclesOptions sut = new("Root");

		DependencyCyclesOptions result = sut.ExcludingSubNamespaces();

		await That(sut.ExcludeSubNamespaces).IsFalse();
		await That(result.ExcludeSubNamespaces).IsTrue();
		await That(result.SliceRoot).IsEqualTo("Root");
	}

	[Test]
	public async Task NamespaceDependencyOptions_ExcludingSubNamespaces_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		NamespaceDependencyOptions sut = new(["Foo",]);

		NamespaceDependencyOptions result = sut.ExcludingSubNamespaces();

		await That(sut.Matches("Foo.Bar")).IsTrue();
		await That(result.Matches("Foo.Bar")).IsFalse();
	}

	[Test]
	public async Task NamespaceDependencyOptions_OrOn_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		NamespaceDependencyOptions sut = new(["Foo",]);

		NamespaceDependencyOptions result = sut.OrOn(["Bar",]);

		await That(sut.Matches("Bar")).IsFalse();
		await That(result.Matches("Bar")).IsTrue();
		await That(result.Describe()).IsEqualTo("namespace \"Foo\" or \"Bar\"");
	}

	[Test]
	public async Task NamespaceDependencyOptions_OrOn_WhenANamespaceIsInvalid_ShouldThrowArgumentException()
	{
		NamespaceDependencyOptions sut = new(["Foo",]);

		void Act()
			=> sut.OrOn(["Bar.",]);

		await That(Act).Throws<ArgumentException>()
			.WithMessage("The namespaces must not end with a dot (sub-namespaces are matched automatically).");
		await That(sut.Describe()).IsEqualTo("namespace \"Foo\"");
	}

	[Test]
	public async Task TypeDependencyOptions_OrOn_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		TypeDependencyOptions sut = new(typeof(int));

		TypeDependencyOptions result = sut.OrOn(typeof(string));

		await That(sut.Matches(typeof(string))).IsFalse();
		await That(result.Matches(typeof(string))).IsTrue();
	}
}
