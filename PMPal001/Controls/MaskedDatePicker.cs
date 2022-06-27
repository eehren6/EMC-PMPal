using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PMPal.Controls
{
    public class MaskedDatePicker : DatePicker
    {
        protected DatePickerTextBox _datePickerTextBox;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _datePickerTextBox = this.Template.FindName("PART_TextBox", this) as DatePickerTextBox;
            if (_datePickerTextBox != null)
            {
                _datePickerTextBox.KeyDown += _datePickerTextBox_KeyDown;
                _datePickerTextBox.TextChanged += dptb_TextChanged;
                _datePickerTextBox.PreviewMouseDown += _datePickerTextBox_PreviewMouseDown;
                _datePickerTextBox.PreviewMouseLeftButtonUp += _datePickerTextBox_PreviewMouseLeftButtonUp;
            }
        }

        private void _datePickerTextBox_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_datePickerTextBox.Text.Length == 0)
            {
                _datePickerTextBox.Text = "__/__/____";
                _datePickerTextBox.SelectionStart = 0;
                _datePickerTextBox.SelectionLength = 2;
            }
            else
            {
                if (_datePickerTextBox.SelectionStart < 2 && _datePickerTextBox.Text.Length >= 2)
                {
                    _datePickerTextBox.SelectionStart = 0;
                    _datePickerTextBox.SelectionLength = 2;
                }
                if (_datePickerTextBox.SelectionStart > 3 && _datePickerTextBox.SelectionStart < 5 && _datePickerTextBox.Text.Length >= 5)
                {
                    _datePickerTextBox.SelectionStart = 3;
                    _datePickerTextBox.SelectionLength = 2;
                }
                if (_datePickerTextBox.SelectionStart > 5)
                {
                    _datePickerTextBox.SelectionStart = 6;
                    _datePickerTextBox.SelectionLength = _datePickerTextBox.Text.Length - _datePickerTextBox.SelectionStart;
                }
            }
        }

        private void _datePickerTextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void _datePickerTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == System.Windows.Input.Key.Back && _datePickerTextBox.SelectedText.Contains('/'))
            //    e.Handled = true;
        }

        private void _datePickerTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
        }

        private void _datePickerTextBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
        }

        private void dptb_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;

            var change = e.Changes.First();
           

            /* implementation presented below */
            if (_datePickerTextBox.Text.Length == 2 && change.AddedLength > 0)
            {
                _datePickerTextBox.Text = _datePickerTextBox.Text + "/__/____";// + "/01/" + DateTime.Now.Year.ToString();

            };
            if (_datePickerTextBox.Text.Length == 5 && change.AddedLength > 0)
            {
                e.Handled = true;
                _datePickerTextBox.Text = _datePickerTextBox.Text + "/____";

            };
            if (e.Changes.First().Offset == 1 || _datePickerTextBox.Text == "__/__/____") //_datePickerTextBox.Text.Length == 10 && )
            {
                _datePickerTextBox.SelectionStart = 3;
                _datePickerTextBox.SelectionLength = 2;
            }
            if (_datePickerTextBox.Text.Length == 10 && e.Changes.First().Offset == 4)
            {
                _datePickerTextBox.SelectionStart = 6;
                _datePickerTextBox.SelectionLength = 4;
            }

        }

    }
}
