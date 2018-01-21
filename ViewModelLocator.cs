using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockHelper
{
    public static class ViewModelLocator
    {
        private static MainViewModel _mainViewModel;

        public static MainViewModel MainViewModel
        {
            get
            {
                if (_mainViewModel != null)
                    return _mainViewModel;

                _mainViewModel = new MainViewModel();
                return _mainViewModel;
            }
        }
    }
}
