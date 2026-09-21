using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public class ManualExpectationBuilderTests
{
	[Fact]
	public async Task AddAsyncContextValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new(null);
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyAsyncContextConstraint<int>(_
				=> Task.FromResult<ConstraintResult>(new DummyConstraint<int>(_ => true)));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Fact]
	public async Task AddAsyncValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new(null);
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyAsyncConstraint<int>(_
				=> Task.FromResult<ConstraintResult>(new DummyConstraint<int>(_ => true)));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Fact]
	public async Task AddContextValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new(null);
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyContextConstraint<int>(_ => new DummyConstraint<int>(_ => true));
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Fact]
	public async Task AddValueConstraint_ShouldAllowGettingExpectationBuilder()
	{
		ManualExpectationBuilder<int> sut = new(null);
		ExpectationBuilder? expectationBuilder = null;
		sut.AddConstraint((e, _, _) =>
		{
			expectationBuilder = e;
			return new DummyConstraint("");
		});

		await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(expectationBuilder).IsSameAs(sut);
	}

	[Fact]
	public async Task Equals_BothNull_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new(null);

		bool result = sut.Equals(null, null);

		await That(result).IsTrue();
	}

	[Fact]
	public async Task Equals_FirstNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new(null);

		bool result = sut.Equals(null, sut);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_ObjectNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new(null);

		bool result = sut.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_SecondNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new(null);

		bool result = sut.Equals(sut, null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_WithSelf_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new(null);

		bool result = sut.Equals(sut, sut);

		await That(result).IsTrue();
	}

	[Fact]
	public async Task GetHashCode_DifferentConstraint_ShouldNotBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new(null);
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new(null);
		sut2.ForWhich<int, int>(x => x)
			.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsNotEqualTo(sut2.GetHashCode());
	}

	[Fact]
	public async Task GetHashCode_SameConstraint_ShouldBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new(null);
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new(null);
		sut2.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode());
	}

	[Fact]
	public async Task GetHashCode_WithParameter_ShouldUseHashCodeFromParameter()
	{
		ManualExpectationBuilder<int> sut1 = new(null);
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new(null);
		sut2.ForWhich<int, int>(x => x)
			.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode(sut1));
	}

	[Theory]
	[InlineData("is equal to 1", "were")]
	[InlineData("are equal to 1", "were")]
	[InlineData("was equal to 1", "were")]
	[InlineData("were equal to 1", "were")]
	[InlineData("starts with \"a\"", "did")]
	[InlineData("has length 3", "did")]
	[InlineData("", "were")]
	public async Task GetResultVerb_ShouldUseDoSupportForEveryVerbButBe(string expectationText, string expected)
	{
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _, _) => new DummyConstraint(expectationText));

		await That(sut.GetResultVerb()).IsEqualTo(expected)
			.Because("the result refers back to the expectation with a pro-verb that has to match its head verb");
	}

	[Fact]
	public async Task IsMet_ShouldThrowNotSupportedException()
	{
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		async Task Act() => await sut.IsMet(
			new ExpectationNode(), null!, new TimeSystemMock(), null, CancellationToken.None);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Use IsMetBy for ManualExpectationBuilder!");
	}

	[Fact]
	public async Task IsMetBy_FailingConstraint_ShouldReturnFailure()
	{
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
	}

	[Fact]
	public async Task IsMetBy_SucceedingConstraint_ShouldReturnSuccess()
	{
		ManualExpectationBuilder<int> sut = new(null);
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
	}

	[Fact]
	public async Task Subject_ShouldBeEmpty()
	{
		ManualExpectationBuilder<int> sut = new(null);

		await That(sut.Subject).IsEmpty();
	}
}
