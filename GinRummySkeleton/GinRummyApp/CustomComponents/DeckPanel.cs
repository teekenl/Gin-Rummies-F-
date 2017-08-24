using System.Windows;
using System.Windows.Controls;

namespace QUT
{
    class DeckPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in this.Children)
                element.Measure(availableSize);

            return new Size(200,200);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int i = 0;
            foreach (FrameworkElement element in this.Children)
                element.Arrange(new Rect(i++, 0, element.DesiredSize.Width, element.DesiredSize.Height));
            return finalSize;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            int i = 0;
            foreach (FrameworkElement element in this.Children)
                if (element != null)
                    Panel.SetZIndex(element, i++);
        }
    }
}
