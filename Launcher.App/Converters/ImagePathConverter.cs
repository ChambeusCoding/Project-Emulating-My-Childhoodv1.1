using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

public class StringToImageConverter : IValueConverter
{
    public static readonly StringToImageConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path)
        {
            try
            {
                if (path.StartsWith("avares://"))
                {
                    return new Bitmap(path);
                }

                if (File.Exists(path))
                {
                    return new Bitmap(path);
                }

                return new Bitmap("avares://Launcher.App/Assets/placeholder.png");
            }
            catch
            {
                return new Bitmap("avares://Launcher.App/Assets/placeholder.png");
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}