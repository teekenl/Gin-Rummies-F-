using System;
using System.Windows.Data;

namespace QUT
{
    class VisibleCardConverter : IValueConverter
    {
        private String GetName(Type t, int tag)
        {
            foreach (var field in t.GetFields())
                if ((int)field.GetValue(null) == tag)
                    return field.Name;
            return null;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var card = (Cards.Card)value;
            var rank = GetName(typeof(Cards.Rank.Tags), card.rank.Tag);
            var suit = GetName(typeof(Cards.Suit.Tags), card.suit.Tag);
            var name = rank.ToLower() + "_of_" + suit.ToLower();
            return @"../Resources/" + name + ".png";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
