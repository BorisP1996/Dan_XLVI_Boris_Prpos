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

                newEmploye.DateOfBirth = DateTime.ParseExact(cmvm.CalculateBirth(Employe.JMBG), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (KeyCheck(newEmploye.JMBG) == true && PasswordCheck(newEmploye.Pasword)==true)
                {
                    context.tblEmployes.Add(newEmploye);
                    context.SaveChanges();

                    MessageBox.Show("Employe is created");
                }
                else
                {
                    MessageBox.Show("JMBG or password already exist");
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
            if (String.IsNullOrEmpty(Employe.FirstName) || String.IsNullOrEmpty(Employe.Surname) || String.IsNullOrEmpty(Employe.JMBG) || Employe.JMBG.Length < 13 || String.IsNullOrEmpty(Employe.Account) || String.IsNullOrEmpty(Employe.Salary.ToString()) || String.IsNullOrEmpty(Employe.Position) || String.IsNullOrEmpty(Employe.Username) || String.IsNullOrEmpty(Employe.Pasword) || String.IsNullOrEmpty(Employe.Email))
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

    }
}
