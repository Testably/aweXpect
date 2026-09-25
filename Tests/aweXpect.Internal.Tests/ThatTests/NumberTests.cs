namespace aweXpect.Internal.Tests.ThatTests;

public sealed class NumberTests
{
	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_byte(byte subject)
	{
		byte expected = subject == 0 ? (byte)1 : (byte)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_decimal(decimal subject)
	{
		decimal expected = subject == 0 ? 1 : 0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_double(double subject)
	{
		double expected = subject == 0.0 ? 1.0 : 0.0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_float(float subject)
	{
		float expected = subject == 0.0F ? 1.0F : 0.0F;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_int(int subject)
	{
		int expected = subject == 0 ? 1 : 0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_long(long subject)
	{
		long expected = subject == 0 ? 1 : 0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_sbyte(sbyte subject)
	{
		sbyte expected = subject == 0 ? (sbyte)1 : (sbyte)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_short(short subject)
	{
		short expected = subject == 0 ? (short)1 : (short)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_uint(uint subject)
	{
		uint expected = subject == 0 ? 1 : (uint)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_ulong(ulong subject)
	{
		ulong expected = subject == 0 ? 1 : (ulong)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}

	[Theory]
	[AutoData]
	public async Task ShouldSupportValues_ushort(ushort subject)
	{
		ushort expected = subject == 0 ? (ushort)1 : (ushort)0;

		async Task Act()
			=> await That(subject).IsEqualTo(expected);

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that subject
			              is equal to {Formatter.Format(expected)},
			              but it was {Formatter.Format(subject)}, which differs by
			              """).AsPrefix();
	}
}
