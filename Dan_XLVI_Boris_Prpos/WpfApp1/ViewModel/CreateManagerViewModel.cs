using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly object locker = new object();
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
            worker.DoWork += WorkerOnDoWork;

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


               
                CreateManager cm = new CreateManager();
                cm.tbJMBG.Text = "";
                //method checks if jmbg already exists in database AND ONLY IF NOT  proceeds to save employe
                if (KeyCheck(newEmploye.JMBG) == true && PasswordCheck(newEmploye.Pasword)==true && UsernameCheck(newEmploye.Username)==true && NumbersOnly(newEmploye.JMBG)==true)
                {
                    newEmploye.DateOfBirth = DateTime.ParseExact(CalculateBirth(Employe.JMBG), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    context.tblEmployes.Add(newEmploye);

                    context.SaveChanges();

                    tblManager newManager = new tblManager();

                    newManager.EmployeID = newEmploye.EmployeID;
                    newManager.SectorID = Sector.SectorID;
                    newManager.LevelID = Level.LevelID;


                    context.tblManagers.Add(newManager);

                    context.SaveChanges();
                    MessageBox.Show("Manager is created.");
                    if (!worker.IsBusy)
                    {
                        worker.RunWorkerAsync();
                    }
                }
                else
                {
                    MessageBox.Show("JMBG or password or username already exists");
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

        public void WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {

            WriteToFile(Employe.FirstName, Employe.Surname);
        }

        public void WriteToFile(string name, string surname)
        {
            string path = @"../../Log.txt";
            lock (locker)
            {
                try
                {
                    Thread.Sleep(2500);
                    StreamWriter sw = new StreamWriter(path, true);
                    string timeLog = "[" + DateTime.Now.ToString("dd.MM.yyyy H:mm:ss") + "] ";
                    sw.WriteLine(timeLog + "Manager is created. Name:{0}, Surname:{1}", name, surname);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
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
            if (Convert.ToInt32(array[0].ToString()) < 4 && Convert.ToInt32(array[3].ToString()) < 3 && Convert.ToInt32(array[2].ToString()) < 2 && (Convert.ToInt32(array[4].ToString()) == 0 || Convert.ToInt32(array[4].ToString()) == 9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}

