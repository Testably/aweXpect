using System;
using System.Runtime.CompilerServices;
#if NET
using System.Diagnostics.CodeAnalysis;
#endif

namespace aweXpect.Snapshots.PublicApi.Fixture;

internal interface ISecret;

public class Vault : ISecret, IDisposable
{
	public const Genres Favorite = Genres.Jazz;
	public const Genres Mixed = Genres.Rock | Genres.Pop;

	private static readonly int Origin = 0;

	public volatile int Openings;

	public string? Label { get; protected set; }
	public string? Code { get; private set; }
	public string? Key { internal get; set; }

	[IndexerName("Slot")]
	public string this[string name] => name;

	public static ref readonly int Peek() => ref Origin;
	public static void Filter(Genres genres = Genres.Rock | Genres.Pop) => throw new NotSupportedException();
	public static @class Echo<@class>(@class value) => value;
	public void Dispose() => Openings = 0;
	public sealed override string ToString() => "vault";
}

public ref struct Cursor
{
	public int Position;
}

public unsafe struct Buffer
{
	public fixed byte Data[16];
}

#if NET
public class Listener
{
	public Listener()
	{
	}

	[SetsRequiredMembers]
	public Listener(string name)
	{
		Name = name;
	}

	public required string Name { get; init; }
}

[AttributeUsage(AttributeTargets.All)]
public sealed class TagAttribute<T> : Attribute;

public interface IRecorder
{
	bool IsRecording => false;

	[Tag<int>]
	void Record();
}
#endif
