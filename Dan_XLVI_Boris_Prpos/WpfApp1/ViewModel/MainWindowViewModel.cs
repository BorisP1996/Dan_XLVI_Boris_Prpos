using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        MainWindow main;
        Entity2 context = new Entity2();

        public MainWindowViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
        }

        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }
        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private ICommand login;
        public ICommand Login
        {
            get
            {
                if (login == null)
                {
                    login = new RelayCommand(param => LoginExecute(), param => CanLoginExecute());
                }
                return login;
            }
        }
        private void LoginExecute()
        {
            try
            {
                if (Password == "WPFadmin" && Username == "WPFadmin")
                {
                    CreateManager createManager = new CreateManager();
                    createManager.ShowDialog();
                }
                List<String> UsernameList = new List<string>();
                List<tblEmploye> EmployeList = context.tblEmployes.ToList();

                foreach (tblEmploye item in EmployeList)
                {
                    UsernameList.Add(item.Username);
                }

                List<string> PasswordList = new List<string>();
                foreach (tblEmploye item in EmployeList)
                {
                    PasswordList.Add(item.Pasword);
                }

                if (!PasswordList.Contains(Password) || !UsernameList.Contains(Username))
                {
                    MessageBox.Show("Username or password does not exist.");
                }
                else
                {
                    tblEmploye employeID1 = (from r in context.tblEmployes where r.Pasword == Password select r).First();
                    tblEmploye employeID2 = (from r in context.tblEmployes where r.Username == Username select r).First();

                    if (employeID1.EmployeID != employeID2.EmployeID)
                    {
                        MessageBox.Show("Parametres exist but they are not matched.");
                    }
                    //ulogovan ali ne zna se ko je
                    if (employeID1.EmployeID == employeID2.EmployeID)
                    {
                        List<int> employeInManagerList = new List<int>();
                        List<tblManager> managerList = context.tblManagers.ToList();
                        foreach (tblManager item in managerList)
                        {
                            employeInManagerList.Add(item.EmployeID.GetValueOrDefault());
                        }
                        //menager loged in
                        if (employeInManagerList.Contains(employeID1.EmployeID))
                        {
                            tblManager manager = (from r in context.tblManagers where r.EmployeID == employeID1.EmployeID select r).First();
                            //modify mendager
                            if (manager.LevelID == 1)
                            {
                                ModifyMenager ms = new ModifyMenager();
                                ms.ShowDialog();
                            }
                            //readonly menager
                            if (manager.LevelID == 2)
                            {
                                //ReadOnly ro = new ReadOnly();
                                //ro.ShowDialog();
                                MessageBox.Show("ReadOnly manager");
                            }
                        }
                        //user
                        if (!employeInManagerList.Contains(employeID1.EmployeID))
                        {
                            MessageBox.Show("User");
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }
        }
        private bool CanLoginExecute()
        {
            if (String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(Username))
            {
                return false;
            }
            else
            {
                return true;
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
            main.Close();
        }
        private bool CanCloseExecute()
        {
            return true;
        }
    }
}
