using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Core.Tests.Results;

public sealed class NumberToleranceResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForNumberTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldSetTheTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		_ = sut.Within(1);

		await That(options.Value.Tolerance).IsEqualTo(1);
	}

	[Test]
	public async Task Within_WithRelativeTolerance_ShouldReplaceTheToleranceAndReturnTheResult()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);
		IWithinToleranceResult<NumberToleranceResult<int, IThat<int>>, int> result = sut.Within(1);

		NumberToleranceResult<int, IThat<int>> continuation =
			result.WithRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		await That(continuation).IsSameAs(result);
		await That(options.Value.ToleranceFor(200)).IsEqualTo(2);
		await That(options.Value.ToString()).IsEqualTo(" ± 1%");
	}

	[Test]
	public async Task WithSubject_ShouldBeOptionsProvider_ForNumberTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task WithSubject_Within_ShouldSetTheTolerance()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		_ = sut.Within(1);

		await That(options.Value.Tolerance).IsEqualTo(1);
	}

	[Test]
	public async Task WithSubject_Within_WithRelativeTolerance_ShouldReplaceTheToleranceAndReturnTheResult()
	{
		OptionsHolder<NumberTolerance<int>> options = new(new NumberTolerance<int>((a, b) => a - b));
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);
		IWithinToleranceResult<NumberToleranceResult<string, IThat<string>, int>, int> result = sut.Within(1);

		NumberToleranceResult<string, IThat<string>, int> continuation =
			result.WithRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		await That(continuation).IsSameAs(result);
		await That(options.Value.ToleranceFor(200)).IsEqualTo(2);
		await That(options.Value.ToString()).IsEqualTo(" ± 1%");
	}

	private static IThat<T> CreateSource<T>(T subject)
	{
#pragma warning disable aweXpect0001
		return That(subject);
#pragma warning restore aweXpect0001
	}

	private static NumberToleranceResult<T, IThat<T>> CreateSut<T>(T subject, OptionsHolder<NumberTolerance<T>> options)
#if NET8_0_OR_GREATER
		where T : struct, INumber<T>
#else
		where T : struct, IComparable<T>
#endif
	{
		IThat<T> source = CreateSource(subject);
		return new NumberToleranceResult<T, IThat<T>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}

	private static NumberToleranceResult<TType, IThat<TType>, TNumber> CreateSutWithSubject<TType, TNumber>(
		TType subject, OptionsHolder<NumberTolerance<TNumber>> options)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		IThat<TType> source = CreateSource(subject);
		return new NumberToleranceResult<TType, IThat<TType>, TNumber>(source.Get().ExpectationBuilder.AddConstraint(
				(it, _) => new DummyConstraint(it)),
			source,
			options);
	}
}
