﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Biller.UI.DocumentView.Contextual 
{
    public class DocumentEditViewModel : Core.Utils.PropertyChangedHelper, Biller.UI.Interface.ITabContentViewModel
    {
        /// <summary>
        /// Use this constructor if you want to show the document creation introduction.
        /// </summary>
        /// <param name="parentViewModel">The parent ViewModel</param>
        public DocumentEditViewModel(DocumentView.DocumentTabViewModel parentViewModel)
        {
            ContextualTabGroup = parentViewModel.ContextualTabGroup;
            this.ParentViewModel = parentViewModel;
            LinkedDocuments = new Core.Models.DocumentFolderModel();
            DocumentEditRibbonTabItem = new DocumentEditRibbonTabItem(this) { DataContext = this };
            DocumentEditTabHolder = new DocumentEditTabHolder() { DataContext = this };
            DocumentFolderControl = new DocumentFolder(this) { DataContext = this };
            DisplayedTabContent = DocumentFolderControl;
            EditMode = true;
            EditContentTabs = new ObservableCollection<UIElement>();
        }

        /// <summary>
        /// Use this constructor if you have a document loaded or you have already created an empty existing document. The UI shows the edit tabs.
        /// </summary>
        /// <param name="parentViewModel">The parent ViewModel</param>
        /// <param name="document">The loaded document</param>
        /// <param name="editEnabled">Determines wheter you can change the <see cref="Document"/>s ID.</param>
        public DocumentEditViewModel(DocumentView.DocumentTabViewModel parentViewModel, Core.Document.Document document, bool editEnabled)
        {
            ContextualTabGroup = parentViewModel.ContextualTabGroup;
            this.ParentViewModel = parentViewModel;
            LinkedDocuments = new Core.Models.DocumentFolderModel();
            DocumentEditRibbonTabItem = new DocumentEditRibbonTabItem(this) { DataContext = this };
            DocumentEditTabHolder = new DocumentEditTabHolder() { DataContext = this };
            DocumentFolderControl = new DocumentFolder(this) { DataContext = this };
            DisplayedTabContent = DocumentEditTabHolder;
            this.Document = document;
            EditMode = editEnabled;
            EditContentTabs = new ObservableCollection<UIElement>();
            DocumentEditRibbonTabItem.ShowDocumentControls();
        }

        public DocumentView.DocumentTabViewModel ParentViewModel { get; private set; }

        public Fluent.RibbonContextualTabGroup ContextualTabGroup { get; private set; }

        public DocumentEditRibbonTabItem DocumentEditRibbonTabItem { get; private set; }

        public DocumentEditTabHolder DocumentEditTabHolder { get; private set; }

        public DocumentFolder DocumentFolderControl { get; private set; }

        public Core.Models.DocumentFolderModel LinkedDocuments { get { return GetValue(() => LinkedDocuments); } private set { SetValue(value); } }

        public bool EditMode { get { return GetValue(() => EditMode); } private set { SetValue(value); } }

        public ObservableCollection<UIElement> EditContentTabs { get; private set; }

        public Fluent.RibbonTabItem RibbonTabItem { get { return DocumentEditRibbonTabItem; } }

        public Core.Document.Document Document
        {
            get { return GetValue(() => Document); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Instance to the page which is showed with the according <see cref="RibbonTabItem"/>.
        /// </summary>
        public System.Windows.UIElement TabContent
        {
            get { return new DocumentEditTabContent(this) { DataContext = this }; ; }
        }

        /// <summary>
        /// Link to the current content of <see cref="DocumentEditTabContent"/>'s ItemsControl.
        /// </summary>
        public System.Windows.UIElement DisplayedTabContent
        {
            get { return GetValue(() => DisplayedTabContent); }
            set { SetValue(value); }
        }

        public async Task ReceiveInternalDocumentCreation(object sender, string DocumentType)
        {
            var factory = ParentViewModel.GetFactory(DocumentType);
            Document = factory.GetNewDocument();
            foreach (var tab in factory.GetEditContentTabs())
            {
                EditContentTabs.Add(tab);
            }
            ExportClass = ParentViewModel.ParentViewModel.SettingsTabViewModel.GetPreferedExportClass(Document);
            await LoadData();
            await ParentViewModel.ParentViewModel.Database.UpdateTemporaryUsedDocumentID("", Document.DocumentID, Document.DocumentType);
            DisplayedTabContent = DocumentEditTabHolder;
            EditMode = true;
            ParentViewModel.LetObserverWatch(this);
        }

        public async Task ReceiveCloseCommand()
        {
            DocumentEditRibbonTabItem.Focus();
            if (Document != null)
                await ParentViewModel.ParentViewModel.Database.UpdateTemporaryUsedDocumentID(Document.DocumentID, "", Document.DocumentType);
            await ParentViewModel.ReceiveCloseEditControl(this);
            var handler = DocumentClosed;
            if (handler != null)
                handler(this, null);
        }

        public async Task LoadData()
        {
            if (Document != null && EditMode == true)
                this.Document.DocumentID = (await ParentViewModel.ParentViewModel.Database.GetNextDocumentID(this.Document.DocumentType)).ToString();

            if (Document != null)
            {
                // Loads the DocumentFolder containing the current Document.
                var list = from Core.Models.DocumentFolderModel folder in ParentViewModel.ParentViewModel.SettingsTabViewModel.DocumentFolder where folder.Documents.Contains(new Core.Document.PreviewDocument(this.Document.DocumentType) { DocumentID = this.Document.DocumentID }) select folder;
                if (list.Count() > 0)
                    LinkedDocuments = list.First();
            }
        }

        public void ReceiveData(object data)
        {
            if (data is Core.Document.PreviewDocument)
            {
                //Load DocumentFolder from SettingsTabViewModel and replace the current one.
                var list = from Core.Models.DocumentFolderModel folder in ParentViewModel.ParentViewModel.SettingsTabViewModel.DocumentFolder where folder.Documents.Contains(data as Core.Document.PreviewDocument) select folder;
                if (list.Count() > 0)
                    LinkedDocuments = list.First();
            }
            else
            {
                var factory = ParentViewModel.GetFactory(Document.DocumentType);
                factory.ReceiveData(data, Document);
            }
        }

        public async Task SaveDocument()
        {
            EventHandler handler = BeforeDocumentSaved;
            if (handler != null)
                handler(this, null);

            await ParentViewModel.SaveOrUpdateDocument(Document);
            
            handler = DocumentSaved;
            if (handler != null)
                handler(this, null);
        }

        public Core.Customers.Customer PreviewCustomer { get { return GetValue(() => PreviewCustomer); } set { SetValue(value); } }

        public Core.Articles.Article PreviewArticle { get { return GetValue(() => PreviewArticle); } set { SetValue(value); } }

        public Core.Interfaces.IExport ExportClass { get { return GetValue(() => ExportClass); } set { SetValue(value); } }

        public event EventHandler BeforeDocumentSaved;
        public event EventHandler DocumentSaved;
        public event EventHandler DocumentClosed;
    }
}
