namespace aweXpect.Tests;

public sealed class NegatedToleranceTests
{
#if NET8_0_OR_GREATER
	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForDateOnly_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateOnly unexpected = new(2020, 1, 10);
		DateOnly subject = unexpected.AddDays(offset);
		TimeSpan tolerance = 2.Days();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForDateTime_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateTime unexpected = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
		DateTime subject = unexpected.AddSeconds(offset);
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForDateTimeOffset_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateTimeOffset unexpected = new(2020, 1, 1, 12, 0, 0, TimeSpan.Zero);
		DateTimeOffset subject = unexpected.AddSeconds(offset);
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForNullableDateOnly_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateOnly unexpected = new(2020, 1, 10);
		DateOnly? subject = unexpected.AddDays(offset);
		TimeSpan tolerance = 2.Days();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForNullableDateTime_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateTime unexpected = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
		DateTime? subject = unexpected.AddSeconds(offset);
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForNullableDateTimeOffset_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		DateTimeOffset unexpected = new(2020, 1, 1, 12, 0, 0, TimeSpan.Zero);
		DateTimeOffset? subject = unexpected.AddSeconds(offset);
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForNullableTimeOnly_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		TimeOnly unexpected = new(12, 0);
		TimeOnly? subject = unexpected.Add(offset.Seconds());
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Theory]
	[InlineData("IsNotGreaterThan", -3, true)]
	[InlineData("IsNotGreaterThan", -2, true)]
	[InlineData("IsNotGreaterThan", -1, false)]
	[InlineData("IsNotGreaterThanOrEqualTo", -3, true)]
	[InlineData("IsNotGreaterThanOrEqualTo", -2, false)]
	[InlineData("IsNotGreaterThanOrEqualTo", -1, false)]
	[InlineData("IsNotLessThan", 1, false)]
	[InlineData("IsNotLessThan", 2, true)]
	[InlineData("IsNotLessThan", 3, true)]
	[InlineData("IsNotLessThanOrEqualTo", 1, false)]
	[InlineData("IsNotLessThanOrEqualTo", 2, false)]
	[InlineData("IsNotLessThanOrEqualTo", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForNullableTimeSpan_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		TimeSpan unexpected = 10.Seconds();
		TimeSpan? subject = unexpected + offset.Seconds();
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotGreaterThan" => async () => await That(subject).IsNotGreaterThan(unexpected).Within(tolerance),
			"IsNotGreaterThanOrEqualTo" => async () => await That(subject).IsNotGreaterThanOrEqualTo(unexpected).Within(tolerance),
			"IsNotLessThan" => async () => await That(subject).IsNotLessThan(unexpected).Within(tolerance),
			"IsNotLessThanOrEqualTo" => async () => await That(subject).IsNotLessThanOrEqualTo(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotGreaterThan" => async () => await That(subject).DoesNotComplyWith(it => it.IsGreaterThan(unexpected).Within(tolerance)),
			"IsNotGreaterThanOrEqualTo" => async () => await That(subject).DoesNotComplyWith(it => it.IsGreaterThanOrEqualTo(unexpected).Within(tolerance)),
			"IsNotLessThan" => async () => await That(subject).DoesNotComplyWith(it => it.IsLessThan(unexpected).Within(tolerance)),
			"IsNotLessThanOrEqualTo" => async () => await That(subject).DoesNotComplyWith(it => it.IsLessThanOrEqualTo(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Theory]
	[InlineData("IsNotAfter", -3, true)]
	[InlineData("IsNotAfter", -2, true)]
	[InlineData("IsNotAfter", -1, false)]
	[InlineData("IsNotOnOrAfter", -3, true)]
	[InlineData("IsNotOnOrAfter", -2, false)]
	[InlineData("IsNotOnOrAfter", -1, false)]
	[InlineData("IsNotBefore", 1, false)]
	[InlineData("IsNotBefore", 2, true)]
	[InlineData("IsNotBefore", 3, true)]
	[InlineData("IsNotOnOrBefore", 1, false)]
	[InlineData("IsNotOnOrBefore", 2, false)]
	[InlineData("IsNotOnOrBefore", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForTimeOnly_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		TimeOnly unexpected = new(12, 0);
		TimeOnly subject = unexpected.Add(offset.Seconds());
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotAfter" => async () => await That(subject).IsNotAfter(unexpected).Within(tolerance),
			"IsNotOnOrAfter" => async () => await That(subject).IsNotOnOrAfter(unexpected).Within(tolerance),
			"IsNotBefore" => async () => await That(subject).IsNotBefore(unexpected).Within(tolerance),
			"IsNotOnOrBefore" => async () => await That(subject).IsNotOnOrBefore(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsAfter(unexpected).Within(tolerance)),
			"IsNotOnOrAfter" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(unexpected).Within(tolerance)),
			"IsNotBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsBefore(unexpected).Within(tolerance)),
			"IsNotOnOrBefore" => async () => await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Theory]
	[InlineData("IsNotGreaterThan", -3, true)]
	[InlineData("IsNotGreaterThan", -2, true)]
	[InlineData("IsNotGreaterThan", -1, false)]
	[InlineData("IsNotGreaterThanOrEqualTo", -3, true)]
	[InlineData("IsNotGreaterThanOrEqualTo", -2, false)]
	[InlineData("IsNotGreaterThanOrEqualTo", -1, false)]
	[InlineData("IsNotLessThan", 1, false)]
	[InlineData("IsNotLessThan", 2, true)]
	[InlineData("IsNotLessThan", 3, true)]
	[InlineData("IsNotLessThanOrEqualTo", 1, false)]
	[InlineData("IsNotLessThanOrEqualTo", 2, false)]
	[InlineData("IsNotLessThanOrEqualTo", 3, true)]
	[InlineData("IsNotBetween", -3, true)]
	[InlineData("IsNotBetween", -2, false)]
	[InlineData("IsNotBetween", -1, false)]
	[InlineData("IsNotBetween", 1, false)]
	[InlineData("IsNotBetween", 2, false)]
	[InlineData("IsNotBetween", 3, true)]
	public async Task ForTimeSpan_ShouldBeTheExactInverseOfTheExpectation(string method, int offset, bool expectSuccess)
	{
		TimeSpan unexpected = 10.Seconds();
		TimeSpan subject = unexpected + offset.Seconds();
		TimeSpan tolerance = 2.Seconds();

		Func<Task> negation = method switch
		{
			"IsNotGreaterThan" => async () => await That(subject).IsNotGreaterThan(unexpected).Within(tolerance),
			"IsNotGreaterThanOrEqualTo" => async () => await That(subject).IsNotGreaterThanOrEqualTo(unexpected).Within(tolerance),
			"IsNotLessThan" => async () => await That(subject).IsNotLessThan(unexpected).Within(tolerance),
			"IsNotLessThanOrEqualTo" => async () => await That(subject).IsNotLessThanOrEqualTo(unexpected).Within(tolerance),
			"IsNotBetween" => async () => await That(subject).IsNotBetween(unexpected).And(unexpected).Within(tolerance),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
		Func<Task> inverse = method switch
		{
			"IsNotGreaterThan" => async () => await That(subject).DoesNotComplyWith(it => it.IsGreaterThan(unexpected).Within(tolerance)),
			"IsNotGreaterThanOrEqualTo" => async () => await That(subject).DoesNotComplyWith(it => it.IsGreaterThanOrEqualTo(unexpected).Within(tolerance)),
			"IsNotLessThan" => async () => await That(subject).DoesNotComplyWith(it => it.IsLessThan(unexpected).Within(tolerance)),
			"IsNotLessThanOrEqualTo" => async () => await That(subject).DoesNotComplyWith(it => it.IsLessThanOrEqualTo(unexpected).Within(tolerance)),
			"IsNotBetween" => async () => await That(subject).DoesNotComplyWith(it => it.IsBetween(unexpected).And(unexpected).Within(tolerance)),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};

		Exception? negationException = await Record.ExceptionAsync(negation);
		Exception? inverseException = await Record.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
}
