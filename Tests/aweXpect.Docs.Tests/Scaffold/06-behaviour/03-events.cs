global using static Snippets.Prelude;
using System.ComponentModel;

namespace Snippets;

internal static class Prelude
{
	public static Player player = new();
}

public class AlbumViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	public string Title { get; private set; } = "";

	public void Rename(string title)
	{
		Title = title;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
	}
}
