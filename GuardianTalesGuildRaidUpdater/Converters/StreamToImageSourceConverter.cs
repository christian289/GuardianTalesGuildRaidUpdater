namespace GuardianTalesGuildRaidUpdater.Converters
{
    public class StreamToImageSourceConverter : ConverterMarkupExtension<StreamToImageSourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not Stream)
                return null;

            Stream imageStream = (Stream)value;
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.StreamSource = imageStream;
            bitmap.EndInit();

            return bitmap;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
