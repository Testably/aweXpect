using System.Linq;
using aweXpect.Chronology;
using aweXpect.Delegates;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class OnlyIfTests
	{
		[Test]
		public async Task WhenContinuingWithAndOrOr_ShouldNotBeOffered()
		{
			Type[] continuations = typeof(ThatDelegateThrows<Exception>).GetMethods()
				.SelectMany(method => new[]
				{
					method.ReturnType.GetProperty("And"), method.ReturnType.GetProperty("Or"),
				})
				.Where(property => property is not null)
				.Select(property => property!.PropertyType)
				.ToArray();

			await That(continuations).IsNotEmpty().And
				.All().Satisfy(type => type.GetMember(nameof(ThatDelegateThrows<Exception>.OnlyIf)).Length == 0)
				.Because("OnlyIf switches the whole Throws expectation and must not read like a further condition");
		}

		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task WhenOnlyIfIsSpecifiedOnce_ShouldApplyTheCondition(bool condition)
		{
			Action action = () =>
			{
				if (condition)
				{
					throw new NotSupportedException();
				}
			};

			async Task<Exception?> Act()
				=> await That(action).Throws().OnlyIf(condition);

			await That(Act).DoesNotThrow();
		}

		[Test]
		[Arguments(true, true)]
		[Arguments(true, false)]
		[Arguments(false, true)]
		[Arguments(false, false)]
		public async Task WhenOnlyIfIsSpecifiedTwice_ShouldThrowInvalidOperationException(bool first, bool second)
		{
			Action action = () => { };

			async Task<Exception?> Act()
				=> await That(action).Throws().OnlyIf(first).OnlyIf(second);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("OnlyIf cannot be specified more than once.")
				.Because("a second condition would silently replace the first one");
		}

		[Test]
		public async Task WhenOnlyIfIsSpecifiedTwice_WithWithinInBetween_ShouldThrowInvalidOperationException()
		{
			Action action = () => { };

			async Task<Exception?> Act()
				=> await That(action).Throws().OnlyIf(false).Within(5.Seconds()).OnlyIf(true);

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("OnlyIf cannot be specified more than once.")
				.Because("the continuation shares the condition of the expectation");
		}
	}
}
