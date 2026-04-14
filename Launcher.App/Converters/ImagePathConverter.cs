using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System; //Kept for learning reference
using System.Globalization;
using System.IO; //Kept for learning reference
using System.Net.Http; //Kept for learning reference

namespace Launcher.App.Converters;

public class StringToImageConverter : IValueConverter
{
    public static readonly StringToImageConverter Instance = new();

    private static readonly HttpClient _http = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            if (value is not string path || string.IsNullOrWhiteSpace(path))
                return LoadFallback();

            if (path.StartsWith("avares://"))
            {
                var uri = new Uri(path);
                using var stream = AssetLoader.Open(uri);
                return new Bitmap(stream);  // Bitmap takes ownership
            }

            if (path.StartsWith("http"))
            {
                try 
                {
                    var bytes = _http.GetByteArrayAsync(path).GetAwaiter().GetResult();  // Or make full async converter
                    using var ms = new MemoryStream(bytes);
                    return new Bitmap(ms);
                }
                catch { }
            }
            
            if (File.Exists(path))
            {
                return new Bitmap(path);
            }

            return LoadFallback();
        }
        catch
        {
            return LoadFallback();
        }
    }

    private Bitmap LoadFallback()
    {
        var uri = new Uri("avares://Launcher.App/Assets/placeholder.png");
        using var stream = AssetLoader.Open(uri);
        return new Bitmap(stream);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}