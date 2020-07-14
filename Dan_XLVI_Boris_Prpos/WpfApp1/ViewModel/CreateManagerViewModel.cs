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
    class CreateManagerViewModel : ViewModelBase
    {

        CreateManager createManagerWindow;
        Entity context = new Entity();
        public CreateManagerViewModel()
        {

        }
        public CreateManagerViewModel(CreateManager cmOpen)
        {
            createManagerWindow = cmOpen;
            SectorList = GetSectors();
            LevelList = GetLevels();
            Employe = new tblEmploye();
            Sector = new tblSector();
            Level = new tblLevel();
        }
        #region Properties
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
        private tblLevel level;
        public tblLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                OnPropertyChanged("Level");
            }
        }
        private List<tblLevel> levelList;
        public List<tblLevel> LevelList
        {
            get
            {
                return levelList;
            }
            set
            {
                levelList = value;
                OnPropertyChanged("LevelList");
            }
        }
        private tblSector sector;
        public tblSector Sector
        {
            get
            {
                return sector;
            }
            set
            {
                sector = value;
                OnPropertyChanged("Sector");
            }
        }
        private List<tblSector> sectorList;
        public List<tblSector> SectorList
        {
            get
            {
                return sectorList;
            }
            set
            {
                sectorList = value;
                OnPropertyChanged("SectorList");
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
                string birthdate = CalculateBirth(Employe.JMBG);

                newEmploye.DateOfBirth = DateTime.ParseExact(CalculateBirth(Employe.JMBG), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

               
                CreateManager cm = new CreateManager();
                cm.tbJMBG.Text = "";
                //method checks if jmbg already exists in database AND ONLY IF NOT  proceeds to save employe
                if (KeyCheck(newEmploye.JMBG) == true && PasswordCheck(newEmploye.Pasword)==true)
                {
                    context.tblEmployes.Add(newEmploye);

                    context.SaveChanges();

                    tblManager newManager = new tblManager();

                    newManager.EmployeID = newEmploye.EmployeID;
                    newManager.SectorID = Sector.SectorID;
                    newManager.LevelID = Level.LevelID;


                    context.tblManagers.Add(newManager);

                    context.SaveChanges();
                    MessageBox.Show("Manager is created.");

                }
                else
                {
                    MessageBox.Show("JMBG or password already exists");
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
            if (String.IsNullOrEmpty(Employe.FirstName) || String.IsNullOrEmpty(Employe.Surname) || String.IsNullOrEmpty(Employe.JMBG) || Employe.JMBG.Length < 13 || String.IsNullOrEmpty(Employe.Account) || String.IsNullOrEmpty(Employe.Salary.ToString()) || String.IsNullOrEmpty(Employe.Position) || String.IsNullOrEmpty(Employe.Username) || String.IsNullOrEmpty(Employe.Pasword) || String.IsNullOrEmpty(Employe.Email) || String.IsNullOrEmpty(Sector.SectorName) || String.IsNullOrEmpty(Level.LevelType))
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
            createManagerWindow.Close();
        }
        private bool CanCloseExecute()
        {
            return true;
        }
        #region Methods
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
        private List<tblSector> GetSectors()
        {
            List<tblSector> listOfSectors = new List<tblSector>();
            listOfSectors = context.tblSectors.ToList();
            return listOfSectors;
        }
        private List<tblLevel> GetLevels()
        {
            List<tblLevel> listOfLevels = new List<tblLevel>();
            listOfLevels = context.tblLevels.ToList();
            return listOfLevels;
        }
        public string CalculateBirth(string jmbg)
        {
            string needed = jmbg.Substring(0, 7);
            int milenium = Convert.ToInt32(needed.Substring(5, 1));
            int god = 0;
            if (milenium == 0)
            {
                god = 2;
            }
            else
            {
                god = 1;
            }
            string year = god + needed.Substring(4, 3);
            string month = needed.Substring(2, 2);
            string day = needed.Substring(0, 2);

            string complete = year + "-" + month + "-" + day;
            return complete;
        }
        #endregion
    }
}

