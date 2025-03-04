﻿#pragma checksum "..\..\ReceiptPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "014906B52F00FE6B7A8E7C7E2C9A97D14C47AC91D4154973B0BF3916DF928DB1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CakeShopProject;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CakeShopProject {
    
    
    /// <summary>
    /// ReceiptPage
    /// </summary>
    public partial class ReceiptPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 52 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox paging;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock viewTotalPages;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox itemPerPage;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button searchButton;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock keywordPlaceholderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox keywordTextBox;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\ReceiptPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid receipt;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CakeShopProject;component/receiptpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ReceiptPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 12 "..\..\ReceiptPage.xaml"
            ((CakeShopProject.ReceiptPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 35 "..\..\ReceiptPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.backButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 48 "..\..\ReceiptPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.previousButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.paging = ((System.Windows.Controls.ComboBox)(target));
            
            #line 52 "..\..\ReceiptPage.xaml"
            this.paging.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.paging_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.viewTotalPages = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            
            #line 60 "..\..\ReceiptPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.nextButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.itemPerPage = ((System.Windows.Controls.ComboBox)(target));
            
            #line 67 "..\..\ReceiptPage.xaml"
            this.itemPerPage.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.itemPerPage_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.searchButton = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\ReceiptPage.xaml"
            this.searchButton.Click += new System.Windows.RoutedEventHandler(this.searchButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.keywordPlaceholderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.keywordTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 91 "..\..\ReceiptPage.xaml"
            this.keywordTextBox.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 91 "..\..\ReceiptPage.xaml"
            this.keywordTextBox.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            
            #line 91 "..\..\ReceiptPage.xaml"
            this.keywordTextBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.keywordTextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.receipt = ((System.Windows.Controls.DataGrid)(target));
            
            #line 95 "..\..\ReceiptPage.xaml"
            this.receipt.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.receipt_MouseUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

