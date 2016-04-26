using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AgentSmith.Options
{

    public class IntegerTextBox : TextBox 
    {

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(IntegerTextBox), new PropertyMetadata(default(int), OnValueChanged));

        public class ValueChangedEventArgs : EventArgs
        {
            public int OldValue;

            public int NewValue;

        }

        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);

        public event ValueChangedEventHandler ValueChanged;

        public IntegerTextBox()
        {
            PreviewTextInput += OnPreviewTextInput;
            this.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(TextBoxPasting));
            TextChanged += OnTextChanged;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            textCompositionEventArgs.Handled = !IsTextAllowed(textCompositionEventArgs.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[0-9]+"); //regex that matches allowed text
            return regex.IsMatch(text);
        }

        // Use the DataObject.Pasting Handler 
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            string txt = (string)GetValue(TextProperty);
            int val;
            if (!int.TryParse(txt, out val)) val = 0;
            SetValue(ValueProperty, val);
        }

        private static void OnValueChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            IntegerTextBox theTextBox = sender as IntegerTextBox;
            if (theTextBox == null) throw new ArgumentException("Sender should be an IntegerTextBox");

            theTextBox.SetValue(TextProperty, args.NewValue.ToString());
            if (theTextBox.ValueChanged != null)
            {
                ValueChangedEventArgs newArgs = new ValueChangedEventArgs()
                    { OldValue = (int)args.OldValue, NewValue = (int)args.NewValue };
                theTextBox.ValueChanged(sender, newArgs);
            }
        }

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                int oldValue = (int)GetValue(ValueProperty);
                SetValue(ValueProperty, value);
            }
        }

    }
}