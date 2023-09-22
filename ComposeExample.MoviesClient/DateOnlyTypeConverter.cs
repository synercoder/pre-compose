using System.ComponentModel;
using System.Globalization;

namespace ComposeExample.MoviesClient
{
    public class DateOnlyTypeConverter : TypeConverter
    {
        private string ToIsoString(DateOnly source) 
            => source.ToString("O");

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
            {
                return DateOnly.Parse(str);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateOnly typedValue)
            {
                return ToIsoString(typedValue);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
