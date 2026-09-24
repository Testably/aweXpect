// The usings of a test project with implicit usings and the `using aweXpect;` from "Getting started". Every other
// namespace has to be shown on the page, as a reader could not guess it.
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
global using aweXpect;
global using Scaffold;

namespace Scaffold;

public class Album(string title = "")
{
	public string Title { get; } = title;
}

// Like the one on the "Object" page, where the collection pages link to for a custom comparer.
public class AlbumComparer : IEqualityComparer<object>
{
	public new bool Equals(object? x, object? y) => (x as Album)?.Title == (y as Album)?.Title;
	public int GetHashCode(object obj) => obj.GetHashCode();
}

public class Track
{
	public string? Title { get; set; }
	public bool IsPlayed { get; set; }
	public int PlayCount { get; set; }
}

public class CustomException : Exception
{
	public CustomException(string message, Exception? innerException = null, int hResult = 0)
		: base(message, innerException)
	{
		HResult = hResult;
	}
}

public class Person;

public class Student : Person
{
	public List<string> Courses { get; } = [];
}

public interface INotification;

public class UserCreatedNotification : INotification;

public class UserDeletedNotification : INotification
{
	public int UserId { get; set; }
}
