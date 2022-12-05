using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridFilterExample.Controls.Model
{
    public interface IDataFilterInfo
    {
        List<object> SearchOptionList { get; set; }
        object SearchOption { get; set; }
        string SearchValue { get; set; }
        bool GetValidationCheck(object a);
    }
}
