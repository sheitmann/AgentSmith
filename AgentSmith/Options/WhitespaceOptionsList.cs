using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AgentSmith.Options
{
    public class WhitespaceOptionsList : ListView 
    {
        public static readonly DependencyProperty WhitespaceOptionsProperty =
            DependencyProperty.Register("WhitespaceOptions", typeof(Dictionary<string, int>), typeof(WhitespaceOptionsList), new PropertyMetadata(null, OnWhitespaceOptionsChanged));


        private Dictionary<string, int> _options;

        public WhitespaceOptionsList()
        {
            _options = new Dictionary<string, int>();
            Items.Clear();
            Items.Add(new KeyValuePair<string, int>("SummaryTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("RemarksTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("ExampleTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("ParamTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("TypeParamTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("ItemTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("DescriptionTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("ExceptionTagOnNewLine", 0));
            Items.Add(new KeyValuePair<string, int>("PermissionTagOnNewLine", 0));
        }

        private void SetListValuesFromDictionary()
        {
            foreach (KeyValuePair<string, int> keyValuePair in _options)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    KeyValuePair<string, int> item = (KeyValuePair<string, int>)Items.GetItemAt(i);
                    if (item.Key.Equals(keyValuePair.Key))
                    {
                        Items.RemoveAt(i);
                        Items.Insert(i, keyValuePair);
                    }
                }
            }
        }

        private void SetDictionaryFromListValues()
        {
            foreach (KeyValuePair<string, int> keyValuePair in _options)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    KeyValuePair<string, int> item = (KeyValuePair<string, int>)Items.GetItemAt(i);
                    if (item.Key.Equals(keyValuePair.Key))
                    {
                        Items.RemoveAt(i);
                        Items.Insert(i, keyValuePair);
                    }
                }
            }
        }


        private static void OnWhitespaceOptionsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WhitespaceOptionsList theList = sender as WhitespaceOptionsList;
            if (theList == null) throw new ArgumentException("Sender should be a WhitespaceOptionsList");

            theList._options = (Dictionary<string, int>) args.NewValue;


        }
    }
}
