using System.Collections.Generic;
using System.Threading;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class DefaultEqualityTests
{
	[Test]
	public async Task IsUsedBy_WhenOptionsAreOfAnUnknownType_ShouldReturnFalse()
	{
		IEqualityOptions<int> options = new UnknownOptions();

		bool result = DefaultEquality.IsUsedBy(options);

		await That(result).IsFalse()
			.Because("only options that are known to compare by their default equality may leave it to the subject");
	}

	[Test]
	public async Task IsUsedBy_WithAComparer_ShouldReturnFalse()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ItemEqualsMatchType<int>.Instance);
		options.Using(EqualityComparer<int>.Default);

		bool result = DefaultEquality.IsUsedBy(options.Value);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsUsedBy_WithItemEquality_ShouldReturnTrue()
	{
		bool result = DefaultEquality.IsUsedBy(ItemEqualsMatchType<int>.Instance);

		await That(result).IsTrue();
	}

	[Test]
	public async Task IsUsedBy_WithTheDefaultMatchType_ShouldReturnFalse()
	{
		bool result = DefaultEquality.IsUsedBy(ObjectMatchType<int>.Default);

		await That(result).IsFalse()
			.Because("an expectation that starts with the shared default never lets the comparer of the subject decide");
	}

	[Test]
	public async Task IsUsedBy_WithTheStartingToleranceMatchType_ShouldReturnTrue()
	{
		bool result = DefaultEquality.IsUsedBy(ObjectToleranceMatchTypeFactory.CreateInt());

		await That(result).IsTrue();
	}

	[Test]
	public async Task IsUsedBy_WithWithin_ShouldReturnFalse()
	{
		OptionsSlot<ObjectMatchType<int>> options = new(ObjectToleranceMatchTypeFactory.CreateInt());
		options.Within(1);

		bool result = DefaultEquality.IsUsedBy(options.Value);

		await That(result).IsFalse();
	}

	private sealed class UnknownOptions : IEqualityOptions<int>
	{
		public IEqualityEvaluator<int> ForEvaluation(IEvaluationContext context, CancellationToken cancellationToken)
			=> throw new NotSupportedException();
	}
}
