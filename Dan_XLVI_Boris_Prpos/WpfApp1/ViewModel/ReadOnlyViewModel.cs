using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1.ViewModel
{
    class ReadOnlyViewModel : ViewModelBase
    {
        ReadOnly ro = new ReadOnly();
        Entity2 context = new Entity2();

        public ReadOnlyViewModel(ReadOnly roOpen)
        {
            ro = roOpen;
        }

        private List<tblEmploye> listEmploye;
        public List<tblEmploye> ListEmploye
        {
            get
            {
                return context.tblEmployes.ToList();
            }
            set
            {
                listEmploye = value;
                OnPropertyChanged("ListEmploye");
            }
        }
        private tblEmploye employe;
        public tblEmploye Employe
        {
            get
            {
                return employe;
            }
            set
            {
                employe = value;
                OnPropertyChanged("Employe");
            }
        }
        private ICommand close;
        public ICommand Close
        {
            get
            {
                if (close == null)
                {
                    close = new RelayCommand(param => CloseExecute(), param => CanCloseExecute());
                }
                return close;
            }
        }
        private void CloseExecute()
        {
            ro.Close();
        }
        private bool CanCloseExecute()
        {
            return true;
        }
        private List<tblEmploye> EmpList()
        {
            List<tblEmploye> list = new List<tblEmploye>();

            list = context.tblEmployes.ToList();

            return list;
        }
    }
}
