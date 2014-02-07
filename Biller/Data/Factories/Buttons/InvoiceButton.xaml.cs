﻿using System;
using System.Collections.Generic;
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

namespace Biller.Data.Factories.Buttons
{
    /// <summary>
    /// Interaktionslogik für InvoiceButton.xaml
    /// </summary>
    public partial class InvoiceButton : Fluent.Button
    {
        public InvoiceButton()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UI.OrderView.Contextual.OrderEditViewModel)
            {
                await (DataContext as UI.OrderView.Contextual.OrderEditViewModel).ReceiveInternalDocumentCreation(this, "Invoice");
                (DataContext as UI.OrderView.Contextual.OrderEditViewModel).OrderEditRibbonTabItem.ShowDocumentControls();
            }
        }
    }
}
