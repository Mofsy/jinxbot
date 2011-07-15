using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JinxBot.Controls
{
    /// <summary>
    /// Interaction logic for WpfDisplay.xaml
    /// </summary>
    public partial class WpfDisplay : UserControl
    {
        public WpfDisplay()
        {
            InitializeComponent();
        }

        public FlowDocument Document
        {
            get { return this.viewer.Document; }
        }

        public FlowDocumentScrollViewer Viewer
        {
            get { return this.viewer; }
        }
    }
}
