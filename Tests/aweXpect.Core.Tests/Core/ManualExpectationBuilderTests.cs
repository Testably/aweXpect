using System.Text;
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
		ManualExpectationBuilder<int> sut = new();
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
		ManualExpectationBuilder<int> sut = new();
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
		ManualExpectationBuilder<int> sut = new();
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
		ManualExpectationBuilder<int> sut = new();
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
	public async Task AppendExpectation_ShouldNotAppendReasons()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason("of a");
		StringBuilder sb = new();

		sut.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("is foo")
			.Because("the reasons follow the whole expectation that the expectations are nested in");
	}

	[Fact]
	public async Task AppendReasons_ShouldAppendAllReasons()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason("of a");
		sut.AddReason("because of b");
		StringBuilder sb = new();

		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEqualTo(", because of a, because of b");
	}

	[Fact]
	public async Task AppendReasons_ShouldOmitReasonsThatMustBeAwaited()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder sb = new();

		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEmpty()
			.Because("an asynchronous reason is only included once it is resolved");
	}

	[Fact]
	public async Task Equals_BothNull_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null, null);

		await That(result).IsTrue();
	}

	[Fact]
	public async Task Equals_FirstNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null, sut);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_ObjectNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_SecondNull_ShouldBeFalse()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(sut, null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_WithSelf_ShouldBeTrue()
	{
		ManualExpectationBuilder<int> sut = new();

		bool result = sut.Equals(sut, sut);

		await That(result).IsTrue();
	}

	[Fact]
	public async Task GetHashCode_DifferentConstraint_ShouldNotBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
		sut2.ForWhich<int, int>(x => x)
			.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsNotEqualTo(sut2.GetHashCode());
	}

	[Fact]
	public async Task GetHashCode_SameConstraint_ShouldBeEqual()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
		sut2.AddConstraint((_, _, _) => new DummyConstraint("foo"));

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode());
	}

	[Fact]
	public async Task GetHashCode_WithParameter_ShouldUseHashCodeFromParameter()
	{
		ManualExpectationBuilder<int> sut1 = new();
		sut1.AddConstraint((_, _, _) => new DummyConstraint("foo"));
		ManualExpectationBuilder<int> sut2 = new();
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
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint(expectationText));

		await That(sut.GetResultVerb()).IsEqualTo(expected)
			.Because("the result refers back to the expectation with a pro-verb that has to match its head verb");
	}

	[Fact]
	public async Task IsMet_ShouldThrowNotSupportedException()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		async Task Act() => await sut.IsMet(
			new ExpectationNode(), null!, new TimeSystemMock(), null, CancellationToken.None);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Use IsMetBy for ManualExpectationBuilder.");
	}

	[Fact]
	public async Task IsMetBy_FailingConstraint_ShouldReturnFailure()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
	}

	[Fact]
	public async Task IsMetBy_SucceedingConstraint_ShouldReturnSuccess()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true));

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
	}

	[Fact]
	public async Task IsMetBy_WhenConstraintFails_ShouldNotApplyReasonButResolveIt()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => false, "is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder expectation = new();
		StringBuilder reasons = new();

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);
		result.AppendExpectation(expectation);
		sut.AppendReasons(reasons);

		await That(expectation.ToString()).IsEqualTo("is foo")
			.Because("the reasons follow the whole expectation that the expectations are nested in");
		await That(reasons.ToString()).IsEqualTo(", because of a");
	}

	[Fact]
	public async Task IsMetBy_WhenReasonIsResolvedAndConstraintSucceeds_ShouldNotApplyReason()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ => true, "is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		await sut.PrepareExpectation(null!, CancellationToken.None);
		StringBuilder sb = new();

		ConstraintResult result = await sut.IsMetBy(1, null!, CancellationToken.None);
		result.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("is foo");
	}

	[Fact]
	public async Task PrepareExpectation_ShouldNotEvaluateTheConstraints()
	{
		bool isEvaluated = false;
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _) => new DummyConstraint<int>(_ =>
		{
			isEvaluated = true;
			return true;
		}));

		await sut.PrepareExpectation(null!, CancellationToken.None);

		await That(isEvaluated).IsFalse();
	}

	[Fact]
	public async Task PrepareExpectation_ShouldResolveReasonsThatMustBeAwaited()
	{
		ManualExpectationBuilder<int> sut = new();
		sut.AddConstraint((_, _, _) => new DummyConstraint("is foo"));
		sut.AddReason(Task.FromResult<string?>("of a"));
		StringBuilder sb = new();

		await sut.PrepareExpectation(null!, CancellationToken.None);
		sut.AppendReasons(sb);

		await That(sb.ToString()).IsEqualTo(", because of a");
	}

	[Fact]
	public async Task Subject_ShouldBeEmpty()
	{
		ManualExpectationBuilder<int> sut = new();

		await That(sut.Subject).IsEmpty();
	}
}
