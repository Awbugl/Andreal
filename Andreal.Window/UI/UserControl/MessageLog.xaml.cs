using System.Collections.Specialized;
using System.Windows.Controls;
using Andreal.Window.Common;

namespace Andreal.Window.UI.UserControl;

internal partial class MessageLog
{
    public MessageLog()
    {
        InitializeComponent();

        List.ItemsSource = Program.Messages;
        Program.Messages.CollectionChanged += OnMessagesChanged;
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var b = List.GetBindingExpression(ItemsControl.ItemsSourceProperty);
        b?.UpdateTarget();

        if (List.Items.MoveCurrentToLast())
        {
            List.ScrollIntoView(List.Items.CurrentItem);
            List.UpdateLayout();
        }
    }
}
