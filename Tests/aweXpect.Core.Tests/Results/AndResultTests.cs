using aweXpect.Core.Extending;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public class AndResultTests
{
	[Test]
	public async Task And_ShouldReturnSubject()
	{
		int subject = 1;
		AndResult<IThat<int>> sut = CreateSut(subject);

		IThat<int> result = sut.And;

		async Task Act()
			=> await result.IsEqualTo(1);

		await That(Act).DoesNotThrow();
		await That(result.Get().ExpectationBuilder.GetRootNode()).Is<AndNode>();
	}

	[Test]
	public async Task Generic_And_ShouldReturnSubject()
	{
		int subject = 1;
		AndResult<int, IThat<int>> sut = CreateGenericSut(subject);

		IThat<int> result = sut.And;

		async Task Act()
			=> await result.IsEqualTo(1);

		await That(Act).DoesNotThrow();
		await That(result.Get().ExpectationBuilder.GetRootNode()).Is<AndNode>();
	}

	[Test]
	public async Task Generic_ShouldBeAwaitable()
	{
		int subject = 1;
		AndResult<int, IThat<int>> sut = CreateGenericSut(subject);

		int result = await sut;

		await That(result).IsEqualTo(subject);
	}

	[Test]
	public async Task ShouldBeAwaitable()
	{
		int subject = 1;
		AndResult<IThat<int>> sut = CreateSut(subject);

		async Task Act() => await sut;

		await That(Act).DoesNotThrow();
	}

	private static AndResult<IThat<T>> CreateSut<T>(T subject)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new AndResult<IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source);
	}

	private static AndResult<T, IThat<T>> CreateGenericSut<T>(T subject)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new AndResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source);
	}
}
