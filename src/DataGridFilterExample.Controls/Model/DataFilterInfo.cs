using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DataGridFilterExample.Controls.Model
{
    public class DataFilterInfo : INotifyPropertyChanged, IDataFilterInfo
    {
        private string _UUID;
        public string UUID
        {
            get { return _UUID; }
            set { _UUID = value; RaisePropertyChanged(nameof(UUID)); }
        }

        public List<object> SearchOptionList { get; set; } = new List<object>();
        public Dictionary<string, string> SearchOptionDic { get; set; } = new Dictionary<string, string>();

        private object _SearchOption;
        public object SearchOption
        {
            get { return _SearchOption; }
            set { _SearchOption = value; RaisePropertyChanged(nameof(SearchOption)); }
        }

        private string _SearchValue;
        public string SearchValue
        {
            get { return _SearchValue; }
            set { _SearchValue = value; RaisePropertyChanged(nameof(SearchValue)); }
        }

        private int _SelectedOperator;
        public int SelectedOperator
        {
            get { return _SelectedOperator; }
            set { _SelectedOperator = value; RaisePropertyChanged(nameof(SelectedOperator)); }
        }

        private bool _IsShowOperator;
        public bool IsShowOperator
        {
            get { return _IsShowOperator; }
            set { _IsShowOperator = value; RaisePropertyChanged(nameof(IsShowOperator)); }
        }

        private bool _IsShowTitle;
        public bool IsShowTitle
        {
            get { return _IsShowTitle; }
            set { _IsShowTitle = value; RaisePropertyChanged(nameof(IsShowTitle)); }
        }

        #region Event
        public event Func<object, bool> ValidationCheckEvent;
        #endregion

        public DataFilterInfo(List<string> columns, Func<object, bool> valueCheckEvent, string uuid, bool isShowOperator = false)
        {
            if (columns == null)
                return;

            if (uuid != null)
                UUID = uuid;

            ShowTitle(!isShowOperator);

            SearchOptionList = columns.Cast<object>().ToList();
            SearchOption = columns[0].ToString();
            SearchValue = string.Empty;
            ValidationCheckEvent += valueCheckEvent;
        }

        public DataFilterInfo(Dictionary<string, string> columnsDic, Func<object, bool> valueCheckEvent, string uuid, bool isShowOperator = false)
        {
            if (columnsDic == null)
                return;

            if (uuid != null)
                UUID = uuid;

            ShowTitle(!isShowOperator);

            SearchOptionDic = columnsDic;
            SearchOption = columnsDic.First();
            SearchValue = string.Empty;
            ValidationCheckEvent += valueCheckEvent;
        }

        public DataFilterInfo(Type searchOptions, string searchValue, Func<object, bool> valueCheckEvent)
        {
            SearchOptionList = Enum.GetValues(searchOptions).Cast<object>().ToList();
            SearchOption = searchOptions.GetEnumValues().GetValue(0);
            SearchValue = searchValue;
            ValidationCheckEvent += valueCheckEvent;
        }

        #region Method
        public virtual bool GetValidationCheck(object dataInfo)
        {
            bool validation = false;
            if (ValidationCheckEvent != null)
                validation = ValidationCheckEvent.Invoke(dataInfo);

            var propertyInfo = dataInfo.GetType().GetProperty(((KeyValuePair<string, string>)SearchOption).Value);
            if (propertyInfo.GetValue(dataInfo).ToString().Contains(SearchValue))
                validation = true;

            return validation;
        }

        public void ShowTitle(bool isShow)
        {
            IsShowTitle = isShow;
            if (isShow)
            {
                IsShowOperator = false;
            }
            else
            {
                IsShowOperator = true;
            }
        }
        #endregion

        #region RaisePropertyChanged
        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}