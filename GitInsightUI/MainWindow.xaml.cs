using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitInsight;
using LibGit2Sharp;

namespace GitInsightUI
{
 

    public partial class MainWindow : Window
    {
        
        public string TabItem1Icon { get; }
        public string TabItem2Icon { get; }
        
        public MainWindow()
        {
            TabItem1Icon = ImagePathBuilder.Build("TabIcons/Sample_User_Icon.png");
            TabItem2Icon = ImagePathBuilder.Build("TabIcons/date.png");
            DataContext = this;
            InitializeComponent();
        }
        
    }
}