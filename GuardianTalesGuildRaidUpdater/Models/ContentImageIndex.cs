using System.Collections.Concurrent;
using System.Windows.Resources;

namespace GuardianTalesGuildRaidUpdater.Models
{
    public partial class ContentImageIndex : ObservableObject
    {
        [ObservableProperty]
        public int currentIdx;
        [ObservableProperty]
        public bool newImageInput;

        public ConcurrentQueue<int> IdxQueue { get; set; }
        public List<ContentImage> Images { get; set; }

        public ContentImageIndex()
        {
            IdxQueue = new ConcurrentQueue<int>();
            string[] list = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contents"));
            Images = new List<ContentImage>(list.Length);
            
            for (int i = 0; i < list.Length; i++)
            {
                Images.Add(new ContentImage(i, Path.GetFileName(list[i])));
            }

            CurrentIdx = 0;
            NewImageInput = true;
        }
    }

    public class ContentImage
    {
        public int Index { get; init; }
        public string FileName { get; init; }
        public Uri Uri { get; init; }
        public StreamResourceInfo ContentInfo { get; init; }

        public ContentImage(int idx, string fileName)
        {
            Index = idx;
            FileName = fileName;
            Uri = new Uri($"/Contents/{fileName}", UriKind.Relative);
            ContentInfo = Application.GetContentStream(Uri);
        }
    }
}
