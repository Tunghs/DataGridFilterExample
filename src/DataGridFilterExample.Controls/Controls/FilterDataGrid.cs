using DataGridFilterExample.Controls.Model;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataGridFilterExample.Controls.Controls
{
    [TemplatePart(Name = PART_ItemsControl, Type = typeof(ItemsControl))]
    [TemplatePart(Name = PART_FilterExpander, Type = typeof(Expander))]
    [TemplatePart(Name = PART_FilterClearBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_FilterDeleteAllBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_FilterAddBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_FilterScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_DataGridCountTB, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_SelectedCountTB, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_FilterSearchBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_ViewAllBtn, Type = typeof(Button))]
    [TemplatePart(Name = PART_DataGrid, Type = typeof(DataGrid))]

    public class FilterDataGrid : Control
    {
        const string PART_ItemsControl = "PART_ItemsControl";
        const string PART_FilterExpander = "PART_FilterExpander";
        const string PART_FilterClearBtn = "PART_FilterClearBtn";
        const string PART_FilterDeleteAllBtn = "PART_FilterDeleteAllBtn";
        const string PART_FilterAddBtn = "PART_FilterAddBtn";
        const string PART_FilterScrollViewer = "PART_FilterScrollViewer";
        const string PART_DataGridCountTB = "PART_DataGridCountTB";
        const string PART_SelectedCountTB = "PART_SelectedCountTB";
        const string PART_FilterSearchBtn = "PART_FilterSearchBtn";
        const string PART_ViewAllBtn = "PART_ViewAllBtn";
        const string PART_DataGrid = "PART_DataGrid";

        #region Dependency Property
        /// <summary>
        /// 데이터 그리드 ItemSource
        /// </summary>
        public IList DataGridCollection
        {
            get { return (IList)GetValue(DataGridCollectionProperty); }
            set { SetValue(DataGridCollectionProperty, value); }
        }

        /// <summary>Identifies the <see cref="DataGridCollection"/> dependency property.</summary>
        public static readonly DependencyProperty DataGridCollectionProperty
            = DependencyProperty.Register(nameof(DataGridCollection),
                                          typeof(IList),
                                          typeof(FilterDataGrid),
                                          new PropertyMetadata(null));

        /// <summary>
        /// 필터 정보 컬렉션
        /// </summary>
        public ObservableCollection<IDataFilterInfo> FilterCollection
        {
            get { return (ObservableCollection<IDataFilterInfo>)GetValue(FilterCollectionProperty); }
            set { SetValue(FilterCollectionProperty, value); }
        }

        /// <summary>Identifies the <see cref="FilterCollection"/> dependency property.</summary>
        public static readonly DependencyProperty FilterCollectionProperty
            = DependencyProperty.Register(nameof(FilterCollection),
                                          typeof(ObservableCollection<IDataFilterInfo>),
                                          typeof(FilterDataGrid),
                                          new PropertyMetadata(null));

        /// <summary>
        /// 필터링에 제외할 필터명
        /// </summary>
        public string ExclusionFilterNames
        {
            get { return (string)GetValue(ExclusionFilterNamesProperty); }
            set { SetValue(ExclusionFilterNamesProperty, value); }
        }

        /// <summary>Identifies the <see cref="ExclusionFilterNames"/> dependency property.</summary>
        public static readonly DependencyProperty ExclusionFilterNamesProperty
           = DependencyProperty.Register(nameof(ExclusionFilterNames),
                                         typeof(string),
                                         typeof(FilterDataGrid),
                                         new PropertyMetadata(null));

        /// <summary>
        /// DataGrid의 컬럼명
        /// </summary>
        public ObservableCollection<DataGridColumn> DataGridColumns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(DataGridColumnsProperty); }
            set { SetValue(DataGridColumnsProperty, value); }
        }

        /// <summary>Identifies the <see cref="DataGridColumns"/> dependency property.</summary>
        public static readonly DependencyProperty DataGridColumnsProperty
            = DependencyProperty.Register(nameof(DataGridColumns),
                                          typeof(ObservableCollection<DataGridColumn>),
                                          typeof(FilterDataGrid),
                                          new PropertyMetadata(null));

        public event Action<IList> DataGridSelectionChanged;
        #endregion


        public FilterDataGrid()
        {
            DataGridColumns = new ObservableCollection<DataGridColumn>();
        }

        #region Event
        event Func<object, bool> ValidationCheckEvent;
        public delegate void SelectionChangeRoutedEventHandler(object sender, ValidationCheckEventArgs args);

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }
        public static readonly RoutedEvent SelectionChangedEvent
            = EventManager.RegisterRoutedEvent("SelectionChanged",
                                               RoutingStrategy.Bubble,
                                               typeof(SelectionChangeRoutedEventHandler),
                                               typeof(FilterDataGrid));
        #endregion

        #region Fields
        protected ItemsControl itemsControl = null;
        protected Expander filterExpander = null;
        protected Button filterClearBtn = null;
        protected Button filterDeleteAllBtn = null;
        protected Button filterAddBtn = null;
        protected ScrollViewer filterScrollViewer = null;
        protected TextBlock dataGridCountTB = null;
        protected TextBlock selectedCountTB = null;
        protected Button filterSearchBtn = null;
        protected Button viewAllBtn = null;
        protected DataGrid dataGrid = null;

        private ICollectionView _CollectionView;
        private Dictionary<string, string> _ColumnsDic;
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            filterExpander = Template.FindName(PART_FilterExpander, this) as Expander;
            if (filterExpander != null)
            {
                // binding event
            }

            filterClearBtn = Template.FindName(PART_FilterClearBtn, this) as Button;
            if (filterClearBtn != null)
            {
                filterClearBtn.Click += FilterClearBtn_Click;
            }

            filterDeleteAllBtn = Template.FindName(PART_FilterDeleteAllBtn, this) as Button;
            if (filterDeleteAllBtn != null)
            {
                filterDeleteAllBtn.Click += FilterDeleteAllBtn_Click;
            }

            filterAddBtn = Template.FindName(PART_FilterAddBtn, this) as Button;
            if (filterAddBtn != null)
            {
                filterAddBtn.PreviewMouseDown += FilterAddBtn_PreviewMouseDown;
                filterAddBtn.PreviewMouseUp += FilterAddBtn_PreviewMouseUp;
            }

            filterScrollViewer = Template.FindName(PART_FilterScrollViewer, this) as ScrollViewer;
            if (filterScrollViewer != null)
            {
                // binding event
            }

            itemsControl = Template.FindName(PART_ItemsControl, this) as ItemsControl;
            if (itemsControl != null)
            {
                // binding event
            }

            dataGridCountTB = Template.FindName(PART_DataGridCountTB, this) as TextBlock;
            if (dataGridCountTB != null)
            {
                dataGridCountTB.Text = "0";
            }

            selectedCountTB = Template.FindName(PART_SelectedCountTB, this) as TextBlock;
            if (selectedCountTB != null)
            {
                selectedCountTB.Text = "0";
            }

            filterSearchBtn = Template.FindName(PART_FilterSearchBtn, this) as Button;
            if (filterSearchBtn != null)
            {
                filterSearchBtn.Click += FilterSearchBtn_Click;
            }

            viewAllBtn = Template.FindName(PART_ViewAllBtn, this) as Button;
            if (viewAllBtn != null)
            {
                viewAllBtn.Click += ViewAllBtn_Click;
            }

            dataGrid = Template.FindName(PART_DataGrid, this) as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                dataGrid.LoadingRow += DataGrid_LoadingRow;
            }

            SetDataGridColumns();
            FilterCollection = new ObservableCollection<IDataFilterInfo>();
        }

        #region Method
        private void SetDataGridColumns()
        {
            foreach (var columns in DataGridColumns)
            {
                dataGrid.Columns.Add(columns);
            }

            _ColumnsDic = new Dictionary<string, string>();
            string[] ExclusionFilterNameArr = ExclusionFilterNames.Replace(" ", "").Split(',');
            foreach (var columns in DataGridColumns)
            {
                if (ExclusionFilterNameArr.Any(x => x == columns.Header.ToString()))
                    continue;

                _ColumnsDic.Add(columns.Header.ToString(), columns.SortMemberPath);
            }
        }

        private void FilterClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilterCollection.Count == 0)
                return;

            foreach (var filter in FilterCollection)
            {
                filter.SearchValue = "";
            }
        }

        private void FilterDeleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilterCollection.Count == 0)
                return;

            FilterCollection.Clear();
            filterExpander.IsExpanded = false;
        }

        private void FilterAddBtn_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FilterCollection.Count > 10)
                return;

            if (FilterCollection.Count == 0)
                FilterCollection.Add(new DataFilterInfo(_ColumnsDic, null, GetGUID()));
            else
                FilterCollection.Add(new DataFilterInfo(_ColumnsDic, null, GetGUID(), true));

            filterExpander.IsExpanded = true;
            filterScrollViewer.ScrollToEnd();
        }

        private string GetGUID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        private void FilterAddBtn_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)itemsControl.ItemContainerGenerator.ContainerFromItem(itemsControl.Items[i]);
                var btn = c.ContentTemplate.FindName("PART_FilterMinusBtn", c) as Button;
                var tb = c.ContentTemplate.FindName("PART_FilterSearchTextBox", c) as TextBox;

                btn.Click += FilterMinusBtn_Click;
                tb.PreviewKeyDown += FilterSearchTextBox_PreviewKeyDown;
            }
        }

        private void FilterSearchTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                RunFiltering();
                e.Handled = true;
            }
        }

        private void RunFiltering(bool isAll = false)
        {
            _CollectionView = CollectionViewSource.GetDefaultView(DataGridCollection);
            if (isAll || FilterCollection.Count == 0)
                _CollectionView.Filter = new System.Predicate<object>(x => true);
            else
                _CollectionView.Filter = new System.Predicate<object>(OnFilterTriggered);

            CollectionViewSource.GetDefaultView(DataGridCollection).Refresh();
        }

        private bool OnFilterTriggered(object obj)
        {
            if (FilterCollection.Count == 1)
                return FilterCollection[0].GetValidationCheck(obj);

            List<bool> memoryList = new List<bool>();
            memoryList.Add((FilterCollection[0].GetValidationCheck(obj)));
            for (int index = 1; index < FilterCollection.Count; index++)
            {
                memoryList.Add(Convert.ToBoolean(((DataFilterInfo)FilterCollection[index]).SelectedOperator));
                memoryList.Add((FilterCollection[index].GetValidationCheck(obj)));
            }
            return ComputeLogicalOperation(memoryList);
        }

        /// <summary>
        /// Index가 홀수 = 값, 짝수 = 연산자
        /// 연산자 => False = AND, True = OR      
        /// </summary>
        /// <param name="memoryList">갑과 연산자로 이루어진 수식 메모리</param>
        /// <returns></returns>
        private bool ComputeLogicalOperation(List<bool> memoryList)
        {
            bool result = true;
            List<bool> resultOfANDOperationList = new List<bool>();
            for (int index = 0; index < memoryList.Count; index++)
            {
                if (index % 2 == 0)
                {
                    result = result && memoryList[index];
                    if (memoryList.Count == index + 1)
                        resultOfANDOperationList.Add(result);
                }
                else
                {
                    if (!memoryList[index])
                        continue;

                    resultOfANDOperationList.Add(result);
                    result = true;
                }
            }
            return resultOfANDOperationList.Contains(true);
        }

        private void FilterMinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var tag = (((Button)sender).Tag).ToString();
            if (tag == null)
                return;

            DeleteFilterCollectionItemByTag(tag);

            if (FilterCollection.Count > 0)
                FilterCollection.OfType<DataFilterInfo>().First().ShowTitle(true);
            else
                filterExpander.IsExpanded = false;

            e.Handled = true;
        }

        private void DeleteFilterCollectionItemByTag(string tag)
        {
            int index = 0;
            foreach (var filter in FilterCollection.OfType<DataFilterInfo>())
            {
                if (filter.UUID == tag)
                    break;

                index++;
            }
            FilterCollection.Remove(FilterCollection[index]);
        }

        private void FilterSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            RunFiltering();
        }

        private void ViewAllBtn_Click(object sender, RoutedEventArgs e)
        {
            RunFiltering(true);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCountTB.Text = dataGrid.SelectedItems.Count.ToString();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dataGridCountTB.Text = DataGridCollection.Count.ToString();
        }
        #endregion
    }

    public class ValidationCheckEventArgs : RoutedEventArgs
    {
        private readonly DataFilterInfo _CheckValidation;
        public DataFilterInfo CheckValidation
        {
            get { return _CheckValidation; }
        }

        public ValidationCheckEventArgs(RoutedEvent routedEvent, DataFilterInfo selectedIndex) : base(routedEvent)
        {
            this._CheckValidation = selectedIndex;
        }
    }
}
