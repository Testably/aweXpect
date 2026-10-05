using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core.TimeSystem;
using aweXpect.Results;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationContextTests
{
	[Fact]
	public async Task CanStoreMultipleValueInParallel()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", "foo-value");
		context.Store("bar", "bar-value");

		context.TryReceive("foo", out string? fooResult);
		await That(fooResult).IsEqualTo("foo-value");
		context.TryReceive("bar", out string? barResult);
		await That(barResult).IsEqualTo("bar-value");
	}

	[Fact]
	public async Task ReleaseMaterializations_ShouldAlsoReleaseTheSourcesOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = attempt.UseMaterializedEnumerable<int>(source).First();

		await context.ReleaseMaterializations();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Fact]
	public async Task ResolvePendingReasons_ShouldAlsoResolveTheReasonsOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		AsyncBecauseReason reason = new(Task.FromResult<string?>("of a"));
		attempt.ResolveOnFailure(reason);

		await context.ResolvePendingReasons();

		await That(reason.ToString()).IsEqualTo(", because of a")
			.Because("the failure message of the evaluation is created from the result of its last attempt");
	}

	[Fact]
	public async Task StartAttempt_ShouldKeepTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};

		Context attempt = await context.StartAttempt();

		await That(attempt.TimeSystem).IsSameAs(timeSystem)
			.Because("a repeated check in another attempt measures and waits like the evaluation");
	}

	[Fact]
	public async Task StartAttempt_ShouldReleaseTheMaterializedSourcesAndReturnAnEmptyContext()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = context.UseMaterializedEnumerable<int>(source).First();
		context.Store("foo", "foo-value");

		Context attempt = await context.StartAttempt();

		await That(source.DisposeCount).IsEqualTo(1);
		await That(attempt).IsNotSameAs(context)
			.Because("each attempt is a separate evaluation");
		await That(attempt.TryReceive("foo", out string? _)).IsFalse()
			.Because("another attempt starts with an empty context");
	}

	[Fact]
	public async Task StartCheck_ShouldKeepTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};

		Context check = await context.StartCheck(null);

		await That(check.TimeSystem).IsSameAs(timeSystem)
			.Because("a repeated check within another check measures and waits like the evaluation");
	}

	[Fact]
	public async Task Store_AfterAnItemEvaluation_ShouldNotReceiveTheValuesOfTheItem()
	{
		Context context = new();
		ManualExpectationBuilder<string> builder = new();
		builder.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s));

		await builder.IsMetBy("a", context, CancellationToken.None);

		await That(context.TryReceive(SharesValueConstraint<string>.Key, out string? _)).IsFalse()
			.Because("the values stored for an item are only visible while the item is evaluated");
	}

	[Fact]
	public async Task Store_InAMemberOfWhich_ShouldNotReceiveTheValuesOfTheSubject()
	{
		Pair sut = new("a", "b");
		IThat<Pair> that = That(sut);

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((_, _) => new SharesValueConstraint<Pair>(p => p.First))
				.ForWhich<Pair, string>(p => p.Second, " which ")
				.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s)));

		await That(Act).DoesNotThrow()
			.Because("the member is evaluated with its own stored values");
	}

	[Fact]
	public async Task Store_InAMemberOfWhose_ShouldNotReceiveTheValuesOfTheSubject()
	{
		Pair sut = new("a", "b");
		IThat<Pair> that = That(sut);

		async Task Act()
			=> await new AndOrWhoseResult<Pair, IThat<Pair>>(that.Get().ExpectationBuilder
					.AddConstraint((_, _) => new SharesValueConstraint<Pair>(p => p.First)), that)
				.Whose(p => p.Second, s => s.Get().ExpectationBuilder
					.AddConstraint((_, _) => new SharesValueConstraint<string?>(x => x!)));

		await That(Act).DoesNotThrow()
			.Because("the member is evaluated with its own stored values");
	}

	[Fact]
	public async Task Store_InItemEvaluations_ShouldNotShareTheValuesBetweenTheItems()
	{
		Context context = new();
		ManualExpectationBuilder<string> builder = new();
		builder.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s));

		ConstraintResult first = await builder.IsMetBy("a", context, CancellationToken.None);
		ConstraintResult second = await builder.IsMetBy("b", context, CancellationToken.None);

		await That(first.Outcome).IsEqualTo(Outcome.Success);
		await That(second.Outcome).IsEqualTo(Outcome.Success)
			.Because("the second item must not receive the value stored for the first one");
	}

	[Fact]
	public async Task TimeSystem_ShouldDefaultToTheRealTimeSystem()
	{
		Context context = new();

		await That(context.TimeSystem).IsSameAs(RealTimeSystem.Instance);
	}

	[Fact]
	public async Task UseMaterializedEnumerable_InAnItemEvaluation_ShouldShareTheMaterializationOfTheEvaluation()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		IEnumerable<int> materialized = context.UseMaterializedEnumerable<int>(source);
		_ = materialized.First();
		MaterializesConstraint constraint = new(source);
		ManualExpectationBuilder<int> builder = new();
		builder.AddConstraint((_, _) => constraint);

		await builder.IsMetBy(1, context, CancellationToken.None);
		await context.ReleaseMaterializations();

		await That(constraint.Materialized).IsSameAs(materialized)
			.Because("the items share the materialized collections of the evaluation");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Fact]
	public async Task WhenNotStoredPreviously_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Fact]
	public async Task WhenTypeDoesNotMatch_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", 42);

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Fact]
	public async Task WhenTypeMatches_ShouldReturnTrue()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", "bar");

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsTrue();
		await That(fooResult).IsEqualTo("bar");
	}

	private static async Task<IEvaluationContext> GetSut()
	{
#pragma warning disable aweXpect0001
		ThatBoolSubject that = That(true);
#pragma warning restore aweXpect0001
		MyContextConstraint constraint = new();
		await new AndOrResult<bool, IExpectThat<bool>>(
			((IExpectThat<bool>)that).ExpectationBuilder
				.AddConstraint((_, _) => constraint),
			that);

		return constraint.Context!;
	}

	private sealed class MaterializesConstraint(IEnumerable<int> source) : IContextConstraint<int>
	{
		public IEnumerable<int>? Materialized { get; private set; }

		public ConstraintResult IsMetBy(int actual, IEvaluationContext context)
		{
			Materialized = context.UseMaterializedEnumerable(source);
			return new DummyConstraintResult<int>(Outcome.Success, actual, "materializes the source");
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("materializes the source");
	}

	private sealed class MyContextConstraint : IContextConstraint<bool>
	{
		public IEvaluationContext? Context { get; private set; }

		/// <inheritdoc />
		public ConstraintResult IsMetBy(bool actual, IEvaluationContext context)
		{
			Context = context;
			return new DummyConstraintResult<bool>(Outcome.Success, actual, "");
		}

		/// <inheritdoc />
		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}

	private sealed class Pair(string first, string second)
	{
		public string First { get; } = first;
		public string Second { get; } = second;
	}

	/// <summary>
	///     Stores the selected value, or compares it with the value that is already stored, like a constraint that caches
	///     what it parsed from the subject for the following constraints.
	/// </summary>
	private sealed class SharesValueConstraint<T>(Func<T, string> selector) : IContextConstraint<T>
	{
		public const string Key = "SharesValue";

		public ConstraintResult IsMetBy(T actual, IEvaluationContext context)
		{
			string value = selector(actual);
			if (!context.TryReceive(Key, out string? stored))
			{
				context.Store(Key, value);
				stored = value;
			}

			return new DummyConstraintResult<T>(stored == value ? Outcome.Success : Outcome.Failure, actual,
				"shares the value", $"it received {stored}");
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("shares the value");
	}
}
