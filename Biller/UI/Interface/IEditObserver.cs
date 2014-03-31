﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biller.UI.Interface
{
    public interface IEditObserver
    {
        void ReceiveArticleEditViewModel(Biller.UI.ArticleView.Contextual.ArticleEditViewModel articleEditViewModel);

        void ReceiveCustomerEditViewModel(Biller.UI.CustomerView.Contextual.CustomerEditViewModel customerEditViewModel);

        void ReceiveDocumentEditViewModel(Biller.UI.DocumentView.Contextual.DocumentEditViewModel documentEditViewModel);
    }
}