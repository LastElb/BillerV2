﻿using Biller.Data.Customers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Biller.UI.CustomerView.Contextual
{
    public class CustomerContextualViewModel : Data.Utils.PropertyChangedHelper, UI.Interface.ITabContentViewModel
    {
        public CustomerContextualViewModel(CustomerTabViewModel parentViewModel, int ID = -1)
        {
            ContextualTabGroup = parentViewModel.ContextualTabGroup;
            Customer = new Data.Customers.Customer();
            TabItem = new ContextualTabItem(this) { DataContext = this };
            tabContent = new ContextualTabContent() { DataContext = this };
            ParentViewModel = parentViewModel;
            ContextualTabList = new ObservableCollection<TabItem>();
            ContextualTabList.Add(new EditView.EditViewTabItem());
            if (ID != -1)
            {
                Customer.CustomerID = ID.ToString();
            }
            EditMode = false;
        }

        public CustomerContextualViewModel(CustomerTabViewModel parentViewModel, Customer customer)
        {
            ContextualTabGroup = parentViewModel.ContextualTabGroup;
            Customer = customer;
            TabItem = new ContextualTabItem(this) { DataContext = this };
            tabContent = new ContextualTabContent() { DataContext = this };
            ParentViewModel = parentViewModel;
            ContextualTabList = new ObservableCollection<TabItem>();
            ContextualTabList.Add(new EditView.EditViewTabItem());
            EditMode = true;
        }

        public Fluent.RibbonContextualTabGroup ContextualTabGroup { get; private set; }

        public CustomerTabViewModel ParentViewModel { get; private set; }
        private ContextualTabItem TabItem { get; set; }
        private ContextualTabContent tabContent { get; set; }

        public ObservableCollection<TabItem> ContextualTabList { get { return GetValue(() => ContextualTabList); } set { SetValue(value); } }

        public Fluent.RibbonTabItem RibbonTabItem
        {
            get { return TabItem; }
        }

        public System.Windows.UIElement TabContent
        {
            get { return tabContent; }
        }

        public async Task LoadData()
        {
            
        }

        public async Task ReceiveCloseCommand()
        {
            TabItem.Focus(); // For MVVM //
            await ParentViewModel.ParentViewModel.Database.UpdateTemporaryUsedCustomerID(Customer.CustomerID, "");
            ParentViewModel.ReceiveCloseArticleControl(this);
        }

        public async Task ReceiveSaveCommand()
        {
            TabItem.Focus(); // For MVVM //
            await ParentViewModel.SaveOrUpdateCustomer(Customer);
        }

        public ObservableCollection<Data.Utils.PaymentMethode> PaymentMethodes { get { return ParentViewModel.ParentViewModel.SettingsTabViewModel.PaymentMethodes; } }

        public Data.Customers.Customer Customer { get { return GetValue(() => Customer); } set { SetValue(value); } }

        public void ReceiveData(object data)
        {
            if (data is Customer)
            {
                (data as Customer).CustomerID = this.Customer.CustomerID;
                this.Customer = (data as Customer);
            }
        }

        public bool EditMode { get; private set; }
    }
}
