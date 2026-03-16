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
                // avares:// assets
                if (path.StartsWith("avares://"))
                {
                    return new Bitmap(path);
                }
                
                // Local file
                if (File.Exists(path))
                {
                    return new Bitmap(path);
                }
                
                // Fallback placeholder
                return new Bitmap("avares://Launcher.App/Assets/placeholder.png");
            }
            catch
            {
                // Silent fallback
                return new Bitmap("avares://Launcher.App/Assets/placeholder.png");
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}