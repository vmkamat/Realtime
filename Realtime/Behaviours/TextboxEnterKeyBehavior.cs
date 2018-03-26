using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Realtime.Behaviours
{
    public class TextboxEnterKeyBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var property = TextBox.TextProperty;

            if (property == null) return;

            var binding = BindingOperations.GetBindingExpression(AssociatedObject, property);
            binding?.UpdateSource();
        }

    }
}