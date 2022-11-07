namespace GuardianTalesGuildRaidUpdater.DataTemplateSelectors
{
    public class ViewModelLocator : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/22e0938a-70bb-4f13-8162-aa8590029439/getting-null-item-in-select-template-of-datatemplateselector?forum=wpf
            // DataTemplateSelector는 Content 속성이 초기화 될 때 SelectTemplate 메서드가 한 번 호출되는데 이 때 item에는 반드시 null 값이 들어오므로, 기본값이나 null을 return 해야만 한다.
            // 바인딩이 되는 시점에 다시 SelectTemplate 호출될 때 item에 Content가 아까 초기화되어 값이 있으므로 item은 null이 아니다.
            if (item is null)
            {
                return base.SelectTemplate(item, container);
            }

            var name = item.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type is not null)
            {
                DataTemplate template = MakeDataTemplate(type, item.GetType());

                return template;
            }
            else
            {
                var template = NotFoundDataTemplate(name);

                return template;
            }
        }

        private DataTemplate MakeDataTemplate(Type viewType, Type viewModelType)
        {
            DataTemplate targetView = new()
            {
                VisualTree = new FrameworkElementFactory(viewType),
                DataType = viewModelType
            };

            return targetView;
        }

        private DataTemplate NotFoundDataTemplate(string viewName)
        {
            FrameworkElementFactory panelFactory = new(typeof(Grid));
            FrameworkElementFactory textBlockFactory = new(typeof(TextBlock));
            textBlockFactory.SetValue(TextBlock.TextProperty, $"Not Found: {viewName}");

            panelFactory.AppendChild(textBlockFactory);

            DataTemplate targetView = new()
            {
                VisualTree = panelFactory,
            };

            return targetView;
        }
    }
}
