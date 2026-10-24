using System.Threading;

namespace aweXpect.Core.Tests.Core;

public sealed class MatchEnvironmentTests
{
	[Test]
	public async Task Constructor_WhenContextIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = new MatchEnvironment(null!, CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("context").And
			.WithMessage("The 'context' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Default_ShouldDescribeAComparisonOutsideAnEvaluation()
	{
		MatchEnvironment environment = default;

		await That(environment.Context).IsNotNull()
			.Because("a matcher must always receive a context, also outside an evaluation");
		await That(environment.Context).IsSameAs(MatchEnvironment.WithoutEvaluation.Context);
		await That(environment.Evaluation).IsNull();
		await That(environment.CancellationToken).IsEqualTo(CancellationToken.None);
	}

	[Test]
	public async Task WhenCreatedForAnEvaluation_ShouldKeepItsContextAndCancellation()
	{
		using CancellationTokenSource cts = new();
		aweXpect.Core.EvaluationContext.EvaluationContext context = new();

		MatchEnvironment environment = new(context, cts.Token);

		await That(environment.Context).IsSameAs(context);
		await That(environment.Evaluation).IsSameAs(context);
		await That(environment.CancellationToken).IsEqualTo(cts.Token);
	}
}
