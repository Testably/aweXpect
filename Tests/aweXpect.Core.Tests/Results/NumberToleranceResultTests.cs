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
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldReturnAResultThatExposesItselfAsResult()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		NumberToleranceResult<int, IThat<int>>.WithinResult result = sut.Within(1);

		await That(result).Is<IWithinToleranceResult<NumberToleranceResult<int, IThat<int>>, int>>()
			.Whose(x => x.Result, it => it.IsSameAs(result));
	}

	[Test]
	public async Task Within_ShouldReturnAResultWithRelativeToleranceOptions()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		NumberToleranceResult<int, IThat<int>>.WithinResult result = sut.Within(1);

		await That(result).Is<IOptionsProvider<IRelativeToleranceOptions<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldSetTheTolerance()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<int, IThat<int>> sut = CreateSut(2, options);

		_ = sut.Within(1);

		await That(options.Tolerance).IsEqualTo(1);
	}

	[Test]
	public async Task WithSubject_ShouldBeOptionsProvider_ForNumberTolerance()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		await That(sut).Is<IOptionsProvider<NumberTolerance<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task WithSubject_Within_ShouldReturnAResultThatExposesItselfAsResult()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		NumberToleranceResult<string, IThat<string>, int>.WithinResult result = sut.Within(1);

		await That(result).Is<IWithinToleranceResult<NumberToleranceResult<string, IThat<string>, int>, int>>()
			.Whose(x => x.Result, it => it.IsSameAs(result));
	}

	[Test]
	public async Task WithSubject_Within_ShouldReturnAResultWithRelativeToleranceOptions()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		NumberToleranceResult<string, IThat<string>, int>.WithinResult result = sut.Within(1);

		await That(result).Is<IOptionsProvider<IRelativeToleranceOptions<int>>>()
			.Whose(x => x.Options.Value, it => it.IsSameAs(options));
	}

	[Test]
	public async Task WithSubject_Within_ShouldSetTheTolerance()
	{
		NumberTolerance<int> options = new((a, b) => a - b);
		NumberToleranceResult<string, IThat<string>, int> sut = CreateSutWithSubject("foo", options);

		_ = sut.Within(1);

		await That(options.Tolerance).IsEqualTo(1);
	}

	private static IThat<T> CreateSource<T>(T subject)
	{
#pragma warning disable aweXpect0001
		return That(subject);
#pragma warning restore aweXpect0001
	}

	private static NumberToleranceResult<T, IThat<T>> CreateSut<T>(T subject, NumberTolerance<T> options)
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
			new OptionsSlot<NumberTolerance<T>>(options));
	}

	private static NumberToleranceResult<TType, IThat<TType>, TNumber> CreateSutWithSubject<TType, TNumber>(
		TType subject, NumberTolerance<TNumber> options)
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
			new OptionsSlot<NumberTolerance<TNumber>>(options));
	}
}
