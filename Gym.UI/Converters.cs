using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Gym.UI
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "True";
        public string FalseValue { get; set; } = "False";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueValue : FalseValue;
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToCommandConverter : IValueConverter
    {
        public ICommand? TrueCommand { get; set; }
        public ICommand? FalseCommand { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueCommand : FalseCommand;
            return FalseCommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MembershipTypeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string membershipType)
            {
                return membershipType switch
                {
                    "12 حصة" => "12 حصة",
                    "شهر" => "شهر",
                    "3 شهور" => "3 شهور", 
                    "12 Sessions" => "12 Sessions",
                    "1 Month" => "1 Month",
                    "3 Months" => "3 Months",
                    _ => membershipType
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToFlowDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRightToLeft && isRightToLeft)
                return FlowDirection.RightToLeft;
            
            return FlowDirection.LeftToRight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is FlowDirection flowDirection && flowDirection == FlowDirection.RightToLeft;
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToColorConverter : IValueConverter
    {
        public static BooleanToColorConverter Instance { get; } = new BooleanToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) : new SolidColorBrush(Color.FromRgb(239, 68, 68));
            return new SolidColorBrush(Color.FromRgb(148, 163, 184));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToStatusTextConverter : IValueConverter
    {
        public static BooleanToStatusTextConverter Instance { get; } = new BooleanToStatusTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? "نشط" : "غير نشط";
            return "غير محدد";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Two-way converter for DateOnly <-> string in format dd/MM/yyyy
    public class DateOnlyToStringConverter : IValueConverter
    {
        private static readonly string[] Formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly d)
            {
                return d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                s = s.Trim();
                if (string.IsNullOrEmpty(s))
                    return Binding.DoNothing; // ignore empty edits

                if (DateOnly.TryParseExact(s, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;

                // If user typed with slashes but swapped order (MM/dd/yyyy), attempt manual swap when obvious
                // Split and reorder if first part > 12 (likely day already) but parse failed
                var parts = s.Replace('-', '/').Split('/');
                if (parts.Length == 3 && int.TryParse(parts[0], out int p0) && int.TryParse(parts[1], out int p1) && int.TryParse(parts[2], out int p2))
                {
                    if (p0 > 12 && p1 <= 12) // user wrote dd/MM but culture parse expected otherwise
                    {
                        var reordered = $"{parts[0]}/{parts[1]}/{parts[2]}"; // already dd/MM/yyyy
                        if (DateOnly.TryParseExact(reordered, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date2))
                            return date2;
                    }
                }
            }
            return Binding.DoNothing; // keep previous value if invalid
        }
    }

    // Converter for calculating price based on service type and duration
    public class ServiceTypeToPriceConverter : IMultiValueConverter
    {
        public static ServiceTypeToPriceConverter Instance { get; } = new ServiceTypeToPriceConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is string serviceType)
            {
                return serviceType switch
                {
                    "مشاية" => values[1] is int duration ? (decimal)(1.5 * duration) : 0m,
                    "ميزان" => 5m,
                    "InBody" => 10m,
                    _ => 0m
                };
            }
            return 0m;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for null DateTime values - shows empty string if DateTime is default/null
    public class NullDateTimeConverter : IValueConverter
    {
        public static NullDateTimeConverter Instance { get; } = new NullDateTimeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Check if it's a default DateTime (which would be 01/01/0001)
                if (dateTime == default(DateTime) || dateTime.Year <= 1)
                {
                    return string.Empty;
                }
                return dateTime.ToString("dd/MM/yyyy HH:mm");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
