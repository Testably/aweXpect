using System.Diagnostics;
using System.Linq;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Delegates;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WithinTests
	{
		[Fact]
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
				.All().Satisfy(type => type.GetMember(nameof(ThatDelegateThrows<Exception>.Within)).Length == 0)
				.Because("Within limits the whole Throws expectation and must not read like a further condition");
		}

		[Fact]
		public async Task WhenDelegateExceedsTheDuration_ShouldCancelTheCancellationToken()
		{
			Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);
			Stopwatch sw = new();

			async Task Act()
				=> await That(@delegate).Throws<MyException>().Within(50.Milliseconds());

			sw.Start();
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             throws a MyException within 0:00.050,
				             but it *
				             """).AsWildcard();
			sw.Stop();

			await That(sw.Elapsed).IsLessThan(10.Seconds())
				.Because("the elapsed duration must cancel the token instead of awaiting the delegate");
		}

		[Fact]
		public async Task WhenTimeoutIsANamedArgument_ShouldSucceed()
		{
			Action @delegate = () => throw new MyException();

			async Task Act()
				=> await That(@delegate).Throws<MyException>().Within(timeout: 1.Seconds());

			await That(Act).DoesNotThrow();
		}
	}
}
