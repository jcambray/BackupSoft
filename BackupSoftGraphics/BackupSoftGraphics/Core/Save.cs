using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Configuration;




namespace clientbackup
{
    public class Save
    {

        private BackgroundWorker asyncWorker;
        public BackgroundWorker AsyncWorker
        {
            get { return asyncWorker; }
        }
        private const long coeff = 1073741824;
        private const long coeffMo = 1048576;
        public Configuration c {get; set;}
        private String saveRoot;
        public string SaveRoot {
            get { return saveRoot;}
            set { saveRoot = value;}
        }
        public DateTime nextSave { get; set; }
        public TimeSpan RemainingTime
        {
            get { return nextSave - DateTime.Now; }
        }

        List<FolderSaveItem> listSaveItems;

        public List<FolderSaveItem> Items
        {
            get
            { return listSaveItems; }
            set { listSaveItems = value; }
        }

        public Save()
        {
            this.c = new Configuration();
            ConfigureAsyncWorker();
            nextSave = InitNextSave();
            saveRoot = GetSaveRoot();
        }

        private void LoadFolders()
        {
            listSaveItems = CreateFolderSaveItems(Serialization.deserializeXML("folders.xml"));
        }

        public void ExecuteSave(Object sender, DoWorkEventArgs e)
        {
            try
            {
                LoadFolders();
                if(!EnoughSpaceAvailable())
                {
                    Log.Write("Sauvegarde annulée: espace disque insuffisant.");
                    return;
                }
                CheckSaveNumber();
                SendSaveStartMail();
                var tempSaveRoot = SaveRoot + ".tmp";
                Directory.CreateDirectory(tempSaveRoot);
                listSaveItems.ForEach(I => I.Copy(tempSaveRoot));         
                RenameSaveRoot(tempSaveRoot);
                UpdateLastSaveDate();
            }
            catch(Exception ex)
            {
                Log.Write(ex.Message);
            }
        }

        private void RenameSaveRoot(String tempSaveRoot)
        {
            Directory.Move(tempSaveRoot, SaveRoot);
        }

        private void UpdateLastSaveDate()
        {
            Serialization.serializeLastSaveDate(DateTime.Now);
        }

        public static void restartComputer()
        {
            System.Diagnostics.ProcessStartInfo restart = new System.Diagnostics.ProcessStartInfo("shutdown.exe", "-r -t 60");
            System.Diagnostics.Process.Start(restart);
            Application.Exit();
        }

 
        public bool isDirectory(string path)
        {
            FileInfo f = new FileInfo(path);
            DirectoryInfo di = new DirectoryInfo(path);
            return f.Attributes == FileAttributes.Directory;
        }

        //public void copyFiles(string s, BackgroundWorker bgw)
        //{
        //    try
        //    {
        //        List<string> excludedFiles = (List<string>)Serialization.deserializeXML("files.xml");
        //        string[] files = Directory.GetFiles(@"C:\" + s);
        //        foreach (string filePath in files)
        //        {
        //            this.fichierCopie = new DirectoryInfo(filePath).Name;
        //            string fileName;
        //            FileInfo fi = new FileInfo(filePath);
        //            fileName = fi.Name;
        //            if (!File.Exists(this.saveRoot + @".tmp\" + this.toSavedFilePathFormat(s) + @"\" + fileName))
        //            {
        //                if (!this.estUnRaccourci(filePath))
        //                {
        //                    bool ok = true;

        //                    foreach (string str in excludedFiles)
        //                    {
        //                        if (str == fileName)
        //                        {
        //                            ok = false;
        //                        }
        //                    }
        //                    if (ok)
        //                    {
        //                        File.Copy(filePath, this.saveRoot + @".tmp\" + this.toSavedFilePathFormat(s) + @"\" + fileName, true);
        //                        this.nbfichierscopie++;
        //                        this.volumeFichiers += fi.Length;
        //                        bgw.ReportProgress(nbfichierscopie);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.write("erreur: ");
        //        Log.write(e.Message);
        //        Log.write("\n");
        //    }
        //}

        //public void copySubDirectories(string savedDirPath, BackgroundWorker bgw)
        //{
        //    try
        //    {
        //        string[] subDirectories = Directory.GetDirectories(@"C:\" + savedDirPath);
        //        foreach (string subDir in subDirectories)
        //        {
        //            DirectoryInfo d = new DirectoryInfo(subDir);
        //            string savedDirPath2 = this.toSavedFilePathFormat(savedDirPath);
        //            savedDirPath2 = this.saveRoot + @".tmp" + @"\" + this.toSavedFilePathFormat(savedDirPath) + @"\" + d.Name;
        //            if (!Directory.Exists(savedDirPath2))
        //            {
        //                Directory.CreateDirectory(savedDirPath2);
        //            }
        //            else
        //            {
        //                Directory.Delete(savedDirPath2, true);
        //                Directory.CreateDirectory(savedDirPath2);
        //            }
        //            string formatedSubdir = subDir.Remove(0, 3);
        //            this.copySubDirectories(formatedSubdir, bgw);
        //            this.copyFiles(savedDirPath + @"\" + d.Name, bgw);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        public string GetSaveRoot()
        {
            return ConfigurationManager.AppSettings["path"] + @"\" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
        }

        //public char verifieSiTerminee()
        //{
        //    char ok = '0';
        //    DateTime dt = Serialization.deserializeLastSaveDate(false);
        //    //si le fichier de sauvegarde final éxiste et le backgroundWorker est inactif (sauvegarde terminé)
        //    //sinon si le fichier de sauvegarde temporaire éxiste et que le backgroundworker est inactif (sauvegarde incomplète)
        //    //sinon si le fichier de sauvegarde temporaire éxiste et que le backgroundworker est actif (sauvegarde en cours)
        //    if (Directory.Exists(ConfigurationManager.AppSettings["path"] + @"\" + dt.Day + "." + dt.Month + "." + dt.Year) && this.bgwk == null)
        //    {
        //        ok = '2';
        //    }
        //    else
        //        if (Directory.Exists(ConfigurationManager.AppSettings["path"] + @"\" + dt.Day + "." + dt.Month + "." + dt.Year + ".tmp") && this.bgwk == null)
        //        {
        //            ok = '0';
        //        }
        //        else
        //            if (Directory.Exists(ConfigurationManager.AppSettings["path"] + @"\" + dt.Day + "." + dt.Month + "." + dt.Year + ".tmp") && this.bgwk != null)
        //            {
        //                ok = '1';
        //            }

        //    Serialization.serializeEtatDerniereSave(ok);

        //    return ok;
        //}

        //public char getEstTerminee()
        //{
        //    return this.estTerminée;
        //}

        //public void setEstTerminee()
        //{
        //    this.estTerminée = this.verifieSiTerminee();
        //}

        //public string getNomFichierCopie()
        //{
        //    return this.fichierCopie;
        //}

        //public int getNbFichiersCopie()
        //{
        //    return this.nbfichierscopie;
        //}

        //public int calculNbFichierACopier()
        //{
        //    List<string> listRepertoires = Serialization.deserializeXML("folders.xml");
        //    int nbfichiers = 0;
        //    foreach (string rep in listRepertoires)
        //    {
        //        try
        //        {
        //            nbfichiers += Directory.GetFiles(@"C:\" + rep, ".", SearchOption.AllDirectories).Length;
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    return nbfichiers;
        //}

        public bool estUnRaccourci(string path)
        {
            bool ok = false;
            if (new FileInfo(path).Extension == ".lnk")
            { ok = true; }
            return ok;
        }

        //public string toSavedFilePathFormat(string s)
        //{
        //    string formated;
        //    char[] c = new char[5];
        //    char c1 = '\\';
        //    c[0] = c1;
        //    formated = s.Split(c, 3)[2];
        //    return formated;
        //}



        //supprime la sauvegarde la plus ancienne tant que la nombre de sauvegardes en plus grand que le nbre de sauvegardes à conserver

        public void CheckSaveNumber()
        {
            string[] directories = Directory.GetDirectories(this.c.path);
            int nbSaves = Directory.GetDirectories(this.c.path).Length;
            while (nbSaves > this.c.nbSaves)
            {
                DateTime dt = Directory.GetCreationTime(directories[0]);
                for (int i = 0; i < directories.Length; i++)
                {
                    DateTime dateCreation = Directory.GetCreationTime(directories[i]);
                    if (dt.CompareTo(dateCreation) > 0)
                    {
                        dt = dateCreation;
                    }
                }
                foreach (string s in directories)
                {
                    if (dt == Directory.GetCreationTime(s))
                    {
                        DirectoryInfo dir = new DirectoryInfo(s);
                        foreach (DirectoryInfo info in dir.GetFileSystemInfos())
                        {
                            if ((info.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                info.Attributes = FileAttributes.Normal;
                            }
                        }
                        Directory.Delete(s, true);
                    }
                }
                nbSaves = Directory.GetDirectories(this.c.path).Length;
                directories = Directory.GetDirectories(this.c.path);
            }

        }

        public long EspaceDispo()
        {
            DriveInfo di = new DriveInfo(new DirectoryInfo(c.path).Root.Name);
            long gigaOctet = di.AvailableFreeSpace / coeff;
            return gigaOctet;
        }

        public long EspaceTotal()
        {
            DriveInfo di = new DriveInfo(new DirectoryInfo(c.path).Root.Name);
            long gigaOctet = di.TotalSize / coeff;
            return gigaOctet;
        }

        public bool checkSaveConditions()
        {
            DateTime lastSave = Serialization.deserializeLastSaveDate(false);
            TimeSpan elapsedTime = DateTime.Now - lastSave;
            return verifieSiTempsEstEcoule(RemainingTime);
        }

        public DateTime InitNextSave()
        {
            DateTime controlDate = Serialization.deserializeLastSaveDate(true);
            if (controlDate.Year == 2000)
            {
                //planifie la prochaine sauvegarde ce jour-ci
                var inter = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,c.heure,c.minute,0);
                nextSave = inter;
                return nextSave;
            }
 
            //planifie la prochaine sauvegarde en fonction des paramètres de configuration
            nextSave = controlDate.AddDays(c.period).AddHours(-controlDate.Hour + c.heure).AddMinutes(-controlDate.Minute + c.minute).AddSeconds(-controlDate.Second);
            
            //si la date de prochaine sauvegarde est dépassée ou si la dernière sauvegarde est incomplète ou introuvable
            //if ((nextSave < DateTime.Now) || verifieSiTerminee() == '0')
            if(nextSave < DateTime.Now)
            {
                nextSave = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, c.heure, c.minute, 0);
                if (nextSave.CompareTo(DateTime.Now) < 0)
                {
                    nextSave.AddDays(1);
                    //this.lbDateProchaineSauvegarde1.Text = this.nextSave.ToShortDateString() + " à " + this.nextSave.ToShortTimeString();
                    Log.Write("Date de sauvegarde dépassée ou sauvegarde incomplete, réinitialisation de la date de la prochaine sauvegarde, nouvelle valeur: " + this.nextSave.ToString());
                    return nextSave;
                }
            }
            return nextSave;
        }

        public bool verifieSiTempsEstEcoule(TimeSpan t)
        {
            bool ok = false;
            if (t.Days == 0 && t.Hours == 0 && t.Minutes == 0 && t.Seconds == 0)
            {
                ok = true;
            }
            return ok;
        }

        public void ajouteTemps(TimeSpan t)
        {
            this.RemainingTime.Add(t);
        }

        //si la date de la prochaine sauvegarde est antérieure à la date actuelle, reporte la sauvegarde à la date actuelle
        public void checkNextSaveDate()
        {
            if (this.nextSave < DateTime.Now)
            {
                this.nextSave = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this.c.heure, this.c.minute, 0);
                //this.lbDateProchaineSauvegarde1.Text = this.nextSave.ToShortDateString() + " à " + this.nextSave.ToShortTimeString();
            }
            Log.Write("Date de sauvegarde dépassée, réinitialisation de la date de la prochaine sauvegarde, nouvelle valeur: " + this.nextSave.ToString());
        }

        public void SetNextSave(DateTime date)
        {
            nextSave = date;
        }

        public bool  IsInExcludedFilesList(string file, List<string> fileList)
        {
            IEnumerable<string> notAllowedFile = from notallowedFilePath in fileList
                                                 where file.Equals(notallowedFilePath)
                                                 select notallowedFilePath;
            return IsExcluded(notAllowedFile);
        }

        public bool IsExcluded(IEnumerable<string> list)
        {
            if (list.Count(s => s.Equals(s)) > 0)
                return true;
            else
                return false;
        }

        //Estimation de l'espace disque nécessaire pour la sauvegarde
        public long GetRequiredDiskSpace()
        {
            long space = 0;
            listSaveItems.ForEach(X => {
                space += X.Size;
            });
            return space;
        }


        //vérifie si l'espace disque restant sur la cible de la sauvegarde est suffisant
        public bool EnoughSpaceAvailable()
        {
            return GetRequiredDiskSpace() < EspaceDispo();
        }

        private void SendSaveStartMail()
        {
            Mailer m = new Mailer(this);
            m.sendNotificationDebut();
            Log.notifieDebutSauvegarde();
        }

        private void CreatesaveDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            if (Directory.Exists(this.saveRoot))
            {
                Directory.Delete(this.saveRoot, true);
            }
            Directory.CreateDirectory(path);
        }

        private List<FolderSaveItem> CreateFolderSaveItems(List<string> foldersList)
        {
            var list = new List<FolderSaveItem>();
            foldersList.ForEach(S => list.Add(new FolderSaveItem(S)));
            var fileList = Serialization.deserializeXML("files.xml");
            
            list.ForEach(F =>
            {
                F.Folder.GetFiles().ToList().ForEach(FI =>
                    {
                        if (fileList.Count(X => X == FI.FullName) == 0)
                            F.AddItem(new FileSaveItem(FI.FullName));
                    });
            });

            return list;
        }

        public void StartAsyncWork()
        {
            asyncWorker = new BackgroundWorker();
            asyncWorker.DoWork += ExecuteSave;
            asyncWorker.WorkerReportsProgress = true;
            asyncWorker.WorkerSupportsCancellation = true;
            asyncWorker.RunWorkerAsync();
        }

        private void  ConfigureAsyncWorker()
        {
            
        }

        public long RequiredSpace()
        {
            long space = 0;
            listSaveItems.ForEach(I => space += I.Size);
            return space;
        }
            
      }
  }


