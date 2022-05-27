using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Net;

namespace LogicielNettoyagePc
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;
      
        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            checkActu();
            UpToDate.Content += " " + Version;
            getDate();


        }

        // Retrouve les infos d'un fichier txt du serveur web indiqué
        public void checkActu()
        {
            string url = "http://localhost/siteweb/actu.txt";
            using (WebClient client = new WebClient())
            {
                string actu = client.DownloadString(url);
                
                if(actu != String.Empty)
                {
                    Actus.Content = actu;
                    Actus.Visibility = Visibility.Visible;
                    ActusBandeau.Visibility = Visibility.Visible;
                }
            }
        }

        // Verifie la version du logiciel selon le fichier Version.txt
        public void checkVersion()
        {
            string url = "http://localhost/siteweb/version";
            using (WebClient client = new WebClient())
            {
                string v = client.DownloadString(url);
                if( Version != v)
                {
                    MessageBox.Show("Mettre à jour ?", "Mise à jour ?", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK);
                    UpToDate.Content = "Version " + v;
                    Version = v;
                }
                else
                {
                    MessageBox.Show("Votre logiciel est à jour !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Calcul de la taille d'un dossier 
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }
        
        // Méthode pour vider les fichiers et dossiers temporaires
        public void ClearTempData(DirectoryInfo dir)
        {
            foreach(FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                } 
                catch (Exception ex)
                {
                    continue;
                }
            }

            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(di.FullName);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        private void Button_Maj_Click(object sender, RoutedEventArgs e)
        {
            checkVersion();
        }

        
        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A faire : Créer une page historique", "TODO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        // Lancement de la méthode de nettoyage des fichiers temporaires
        private void Button_Cleaning_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "Nettoyage en cours...";

            Clipboard.Clear();
            try
            {
                ClearTempData(winTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur " + ex.Message);
            }
            btnClean.Content = "Nettoyage terminé !";
            Titre.Content = "Nettoyage effectué !";
            Espace.Content = "0 Mb";
        }
        
        // Renvoi vers le site indiqué
        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("http://www.google.fr")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lancement de la méthode AnalyseFolder au moment du click 
        private void Button_Analyse_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }
        
        //Analyse un dossier
        public void AnalyseFolders()
        {
            Console.WriteLine("Début de l'analyse...");
            DateTime todayDate = new DateTime();
            todayDate = DateTime.Now;
            
            long totalSize = 0;

            try
            {
            totalSize += DirSize(winTemp) / 1000000;
            totalSize += DirSize(appTemp) / 1000000;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            Espace.Content = totalSize + " Mb";
            Titre.Content = "Analyse effectué";
            Date.Content = todayDate.ToString("Le dd/MM/yyyy à HH") + "h" + todayDate.ToString("mm");
            SaveDate(); 
        }

        // Sauvegarde la date dans le fichier Date.txt
        public void SaveDate()
        {
            string Date = DateTime.Now.ToString("Le dd/MM/yyyy à HH") + "h" + DateTime.Now.ToString("mm");
            File.WriteAllText("Date.txt", Date);
        }

        
        public void getDate()
        {
            string LastDate = File.ReadAllText("Date.txt");
            if(LastDate != string.Empty)
            {
                Date.Content = LastDate;
            }
        }
    }
}
