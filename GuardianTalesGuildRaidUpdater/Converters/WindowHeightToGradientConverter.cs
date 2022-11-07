namespace GuardianTalesGuildRaidUpdater.Converters
{
    public class WindowHeightToGradientConverter : ConverterMarkupExtension<WindowHeightToGradientConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double dValue) &&
                double.TryParse(parameter.ToString(), out double dParam))
            {
                if (dValue <= dParam)
                {
                    return value;
                }

                double result = dValue - dParam;

                return result;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
