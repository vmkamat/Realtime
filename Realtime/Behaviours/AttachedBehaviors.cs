using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Realtime.Behaviours
{
    public class AttachedBehaviors 
    {
        public static readonly DependencyProperty AllowNotificationOnTargetUpdateProperty =
            DependencyProperty.RegisterAttached("AllowNotificationOnTargetUpdate", typeof(bool),
                typeof(AttachedBehaviors), new PropertyMetadata(false, PropChanged));


        public static bool GetAllowNotificationOnTargetUpdate(DependencyObject obj)
        {
            return (bool) obj.GetValue(AllowNotificationOnTargetUpdateProperty);
        }

        public static void SetAllowNotificationOnTargetUpdate(DependencyObject obj, object value)
        {
            obj.SetValue(AllowNotificationOnTargetUpdateProperty, value);
        }

        private static void PropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement fe)) return;

            var bindingExpression = fe.GetBindingExpression(TextBlock.TextProperty);
            if (bindingExpression == null) return;

            var oldBinding = bindingExpression.ParentBinding;
            var newBinding = new Binding(oldBinding.Path.Path)
            {
                NotifyOnTargetUpdated = (bool) e.NewValue,
                StringFormat = oldBinding.StringFormat
            };

            fe.SetBinding(TextBlock.TextProperty, newBinding);
        }
    }
}