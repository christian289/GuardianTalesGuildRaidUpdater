namespace GuardianTalesGuildRaidUpdater.Converters
{
    public class UriToBitmapImageConverter : ConverterMarkupExtension<UriToBitmapImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(new Uri(value.ToString()));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
