using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.Model;
using WpfApp1.ViewModel;
using WpfApp1.View;

namespace Zadatak_1.ViewModel
{
    class ModifyManagerViewModel : ViewModelBase
    {
        ModifyMenager modifyMenagerWindow;
        Entity context = new Entity();
        CreateManagerViewModel cmvm = new CreateManagerViewModel();

        public ModifyManagerViewModel(ModifyMenager mmOpen)
        {
            modifyMenagerWindow = mmOpen;
            Employe = new tblEmploye();
            ListEmploye = EmpList();
        }
        #region Properties
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
        #endregion
        private ICommand save;
        public ICommand Save
        {
            get
            {
                if (save == null)
                {
                    save = new RelayCommand(param => SaveExecute(), param => CanSaveExecute());
                }
                return save;
            }
        }
        private void SaveExecute()
        {
            try
            {
                tblEmploye newEmploye = new tblEmploye();
                newEmploye.FirstName = Employe.FirstName;
                newEmploye.Surname = Employe.Surname;
                newEmploye.JMBG = Employe.JMBG;
                newEmploye.Salary = Convert.ToInt32(Employe.Salary);
                newEmploye.Account = Employe.Account;
                newEmploye.Pasword = Employe.Pasword;
                newEmploye.Username = Employe.Username;
                newEmploye.Position = Employe.Position;
                newEmploye.Email = Employe.Email;
                string birthdate = cmvm.CalculateBirth(Employe.JMBG);


                if (KeyCheck(newEmploye.JMBG) == true && PasswordCheck(newEmploye.Pasword)==true && UsernameCheck(newEmploye.Username)==true && NumbersOnly(newEmploye.JMBG)==true)
                {
                    newEmploye.DateOfBirth = DateTime.ParseExact(cmvm.CalculateBirth(Employe.JMBG), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    context.tblEmployes.Add(newEmploye);
                    context.SaveChanges();

                    MessageBox.Show("Employe is created");

                    ListEmploye = EmpList();
                }
                else
                {
                    MessageBox.Show("JMBG or password or username already exist");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException 
                        MessageBox.Show(message);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

        }
        private bool CanSaveExecute()
        {
            if (String.IsNullOrEmpty(Employe.FirstName) || String.IsNullOrEmpty(Employe.Surname) || String.IsNullOrEmpty(Employe.JMBG) || String.IsNullOrEmpty(Employe.Account) || Employe.JMBG.Length<13 || String.IsNullOrEmpty(Employe.Salary.ToString()) || String.IsNullOrEmpty(Employe.Position) || String.IsNullOrEmpty(Employe.Username) || String.IsNullOrEmpty(Employe.Pasword) || String.IsNullOrEmpty(Employe.Email))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private ICommand delete;
        public ICommand Delete
        {
            get
            {
                if (delete==null)
                {
                    delete = new RelayCommand(param => DeleteExecute(), param => CanDeleteExecute());
                }
                return delete;
            }
        }
        private void DeleteExecute()
        {
            try
            {
                using (Entity context = new Entity())
                {
                    tblEmploye employeToDelete = (from x in context.tblEmployes where x.EmployeID == Employe.EmployeID select x).First();

                    List<int> foreignKeysList = new List<int>();
                    List<tblManager> managersList = context.tblManagers.ToList();

                    for (int i = 0; i < managersList.Count; i++)
                    {
                        foreignKeysList.Add(managersList[i].EmployeID.GetValueOrDefault());
                    }
                    MessageBoxResult mbr = MessageBox.Show("Are you sure you want to delete employe?", "Delete confirmation", MessageBoxButton.YesNo);

                    if (mbr == MessageBoxResult.Yes)
                    {
                        if (foreignKeysList.Contains(employeToDelete.EmployeID))
                        {
                            tblManager managerToDelete = (from x in context.tblManagers where x.EmployeID == Employe.EmployeID select x).First();
                            context.tblManagers.Remove(managerToDelete);
                        }
                        context.tblEmployes.Remove(employeToDelete);
                        context.SaveChanges();
                        MessageBox.Show("Employe is deleted");
                        ListEmploye = EmpList();
                        Employe = new tblEmploye();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
}
        private bool CanDeleteExecute()
        {
            if (Employe != null)
            {
                return true;
            }
            else
            {
                return false;
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
            modifyMenagerWindow.Close();
        }
        private bool CanCloseExecute()
        {
            return true;
        }
        private bool KeyCheck(string jmbg)
        {
            List<tblEmploye> list = context.tblEmployes.ToList();

            List<string> keyes = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                keyes.Add(list[i].JMBG);
            }

            if (keyes.Contains(jmbg))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool PasswordCheck(string pas)
        {
            List<tblEmploye> list = context.tblEmployes.ToList();

            List<string> paswords = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                paswords.Add(list[i].Pasword);
            }

            if (paswords.Contains(pas))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool UsernameCheck(string user)
        {
            List<tblEmploye> list = context.tblEmployes.ToList();
            List<string> usernames = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                usernames.Add(list[i].Username);
            }

            if (usernames.Contains(user))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool NumbersOnly(string input)
        {
           
            char[] array = input.ToCharArray();

            int counter = 0;
            //there must be 13 characaters
            for (int i = 0; i < array.Length; i++)
            {
                if (Char.IsDigit(array[i]))
                {
                    counter++;
                }
            }
            //first and thirs number must be correct
            if (Convert.ToInt32(array[0].ToString()) < 4 && Convert.ToInt32(array[3].ToString()) <3 && Convert.ToInt32(array[2].ToString())<2 &&( Convert.ToInt32(array[4].ToString())==0 || Convert.ToInt32(array[4].ToString()) == 9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<tblEmploye> EmpList()
        {
            List<tblEmploye> list = new List<tblEmploye>();

            list = context.tblEmployes.ToList();

            return list;
        }

    }
}
