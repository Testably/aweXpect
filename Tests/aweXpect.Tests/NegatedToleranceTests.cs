namespace aweXpect.Tests;

public sealed class NegatedToleranceTests
{
#if NET8_0_OR_GREATER
	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Test]
	[Arguments("IsNotGreaterThan", -3, true)]
	[Arguments("IsNotGreaterThan", -2, true)]
	[Arguments("IsNotGreaterThan", -1, false)]
	[Arguments("IsNotGreaterThanOrEqualTo", -3, true)]
	[Arguments("IsNotGreaterThanOrEqualTo", -2, false)]
	[Arguments("IsNotGreaterThanOrEqualTo", -1, false)]
	[Arguments("IsNotLessThan", 1, false)]
	[Arguments("IsNotLessThan", 2, true)]
	[Arguments("IsNotLessThan", 3, true)]
	[Arguments("IsNotLessThanOrEqualTo", 1, false)]
	[Arguments("IsNotLessThanOrEqualTo", 2, false)]
	[Arguments("IsNotLessThanOrEqualTo", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}

#if NET8_0_OR_GREATER
	[Test]
	[Arguments("IsNotAfter", -3, true)]
	[Arguments("IsNotAfter", -2, true)]
	[Arguments("IsNotAfter", -1, false)]
	[Arguments("IsNotOnOrAfter", -3, true)]
	[Arguments("IsNotOnOrAfter", -2, false)]
	[Arguments("IsNotOnOrAfter", -1, false)]
	[Arguments("IsNotBefore", 1, false)]
	[Arguments("IsNotBefore", 2, true)]
	[Arguments("IsNotBefore", 3, true)]
	[Arguments("IsNotOnOrBefore", 1, false)]
	[Arguments("IsNotOnOrBefore", 2, false)]
	[Arguments("IsNotOnOrBefore", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
#endif

	[Test]
	[Arguments("IsNotGreaterThan", -3, true)]
	[Arguments("IsNotGreaterThan", -2, true)]
	[Arguments("IsNotGreaterThan", -1, false)]
	[Arguments("IsNotGreaterThanOrEqualTo", -3, true)]
	[Arguments("IsNotGreaterThanOrEqualTo", -2, false)]
	[Arguments("IsNotGreaterThanOrEqualTo", -1, false)]
	[Arguments("IsNotLessThan", 1, false)]
	[Arguments("IsNotLessThan", 2, true)]
	[Arguments("IsNotLessThan", 3, true)]
	[Arguments("IsNotLessThanOrEqualTo", 1, false)]
	[Arguments("IsNotLessThanOrEqualTo", 2, false)]
	[Arguments("IsNotLessThanOrEqualTo", 3, true)]
	[Arguments("IsNotBetween", -3, true)]
	[Arguments("IsNotBetween", -2, false)]
	[Arguments("IsNotBetween", -1, false)]
	[Arguments("IsNotBetween", 1, false)]
	[Arguments("IsNotBetween", 2, false)]
	[Arguments("IsNotBetween", 3, true)]
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

		Exception? negationException = await Catch.ExceptionAsync(negation);
		Exception? inverseException = await Catch.ExceptionAsync(inverse);

		await That(negationException is null).IsEqualTo(expectSuccess)
			.Because("the tolerance widens the unnegated expectation and so narrows its negation");
		await That(negationException?.Message).IsEqualTo(inverseException?.Message)
			.Because("the written negation must decide and report like DoesNotComplyWith");
	}
}
