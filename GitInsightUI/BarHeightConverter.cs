using System;
using System.Globalization;
using System.Windows.Data;

namespace GitInsightUI;

public class BarHeightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Console.WriteLine("HER ER VI");
        Console.WriteLine("commits:" + values[0]);
        Console.WriteLine("maxScreenHeight" + values[1]);
        Console.WriteLine("maxHighestCommitCount"+parameter);
       
        if (
            //Actual number of commits
            values[0] is int commits
            // Max height of our rectangle, according to available screen space
            && values[1] is double maxScreenHeight
            // Our output is the height of our rectangle, compared to the max height % of the screen
            && targetType == typeof(double)
            // e.g. 50 and a max commit of 75, we want 66% of the max screen height
            && parameter is int maxHighestCommitCount)
        {
            // Note: *0.98d, is a trick to make it fit within screen and not add a scrollbar.
            // ReSharper disable once HeapView.BoxingAllocation
            return commits / maxHighestCommitCount * maxScreenHeight * 0.98d;
        }

        throw new InvalidCastException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}