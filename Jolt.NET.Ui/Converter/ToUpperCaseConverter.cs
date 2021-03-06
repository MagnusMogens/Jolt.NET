﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Jolt.NET.UI.Converter
{
    public class ToUpperCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToUpperInvariant();
        }
    }
}
