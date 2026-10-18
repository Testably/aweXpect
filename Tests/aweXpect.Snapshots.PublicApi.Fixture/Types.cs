using System;
using System.Collections.Generic;

namespace aweXpect.Snapshots.PublicApi.Fixture;

public abstract class Album : IComparable<Album>
{
	public const int MaxTracks = 99;
	public static readonly Album? Empty = null;

	protected Album(string title)
	{
		Title = title;
	}

	public string Title { get; }
	public string? Note { get; set; }
	public string? Cover { [return: Marker("getter")] get; [Marker("setter")] set; }
	public string this[int track] => Title;

	public event EventHandler? Played;

	public int CompareTo(Album? other) => 0;
	public abstract IEnumerable<string> Tracks();
	public virtual void Play(params IEnumerable<string> tracks) => Played?.Invoke(this, EventArgs.Empty);
	protected internal virtual bool TryGetTrack(int number, out string? track, ref int position, in DateTime at)
	{
		track = null;
		return false;
	}

	internal static void Hidden() => throw new NotSupportedException();

	public static bool operator ==(Album? left, Album? right) => false;
	public static bool operator !=(Album? left, Album? right) => true;
	public static bool operator <(Album? left, Album? right) => false;
	public static bool operator <=(Album? left, Album? right) => true;
	public static bool operator >(Album? left, Album? right) => false;
	public static bool operator >=(Album? left, Album? right) => true;
	public static explicit operator string(Album album) => album.Title;

	public override bool Equals(object? obj) => false;
	public override int GetHashCode() => 0;

	protected class Edition
	{
		public int Year { get; init; }
	}

	public sealed class Track
	{
		public int Number { get; init; }
	}
}

[Flags]
public enum Genres : byte
{
	None = 0,
	Rock = 1,
	Jazz = 2,
	All = Rock | Jazz,
	Pop = 4,
}

public enum Mood : sbyte
{
	Sad = -1,
	Neutral = 0,
	Happy = 1,
}

[AttributeUsage(AttributeTargets.All)]
public sealed class MarkerAttribute(string name) : Attribute
{
	public string Name { get; } = name;
	public int[]? Numbers { get; set; }
	public AttributeTargets Targets { get; set; }
	public double Threshold { get; set; }
	public Type? Type { get; set; }
}

public interface IPlayer<in TAlbum> where TAlbum : Album
{
	string Name { get; }
	void Play(TAlbum album, bool shuffle = false);
}

#if NET
public interface IParser<TSelf> where TSelf : IParser<TSelf>
{
	static abstract TSelf Default { get; }
	static abstract TSelf Parse(string text);
	static virtual bool CanParse(string text) => true;
}
#endif

public delegate TResult Converter<in TSource, out TResult>(TSource source) where TSource : notnull;

[return: Marker("delegate")]
public delegate ref int Counter();

public record Artist(string Name, Genres Genres = Genres.Rock);

public readonly record struct Rating(int Stars);

public readonly struct Duration
{
	public Duration(int seconds)
	{
		Seconds = seconds;
	}

	public int Seconds { get; }
#if NET
	public static ReadOnlySpan<char> Format(scoped ReadOnlySpan<char> pattern) => default;
#endif
}

public struct Volume
{
	public Volume()
	{
		Level = 5;
	}

	public int Level { get; }
}

public static class Library
{
	public static T? Find<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : class, new() => null;
	public static void Reset<T>(ref T value) where T : unmanaged => value = default;

	[Obsolete("Use Find instead.")]
	public static object? Search(string query) => null;

	[return: Marker("result")]
	public static object? Lookup(string key) => null;

	[Marker("arguments", Numbers = new[] { 1, 2, }, Targets = AttributeTargets.Class | AttributeTargets.Struct,
		Type = typeof(Album))]
	public static void Annotated() => throw new NotSupportedException();
}
