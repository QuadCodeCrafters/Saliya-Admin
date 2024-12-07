using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernApp.Core
{
    public static class ClearHelper
    {
        public static void ClearTextBoxesAndComboBoxes(DependencyObject container)
        {
            if (container == null) return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);

                if (child is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (child is ComboBox comboBox)
                {
                    comboBox.SelectedIndex = -1; // Clear selection
                }
                else if (child is Image image)
                {
                    image.Source = null;
                }
                else
                {
                    // Recursively check child elements
                    ClearTextBoxesAndComboBoxes(child);
                }
            }
        }
    }
}
