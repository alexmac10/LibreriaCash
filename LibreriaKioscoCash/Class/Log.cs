using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    class Log
    {
        public static Log instance = null;
        private string routeFile;
        private string routeDirectory;

        private Log()
        {
            this.setPathApp();
        }

        public static Log GetInstance()
        {
            if (instance == null)
            {
                instance = new Log();
            }

            return instance;
        }

        private void setPathApp()
        {
            int pointInitial, length;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            pointInitial = path.IndexOf(@"\") + 1;
            length = path.Length - pointInitial;
            path = path.Substring(pointInitial, length);
            this.routeFile = path;
        }

        public void registerLogAction(string message)
        {
            message = "Action : " + message;
            this.saveMessegeInFile(message, @"\LogKioskCash.log");
        }

        public void registerLogError(string message, string code)
        {
            message = "Error (" + code + "): " + message;
            this.saveMessegeInFile(message, @"\LogKioskCashError.log");
        }

        private void saveMessegeInFile(string message, string file)
        {
            MemoryStream ms = new MemoryStream();
            DateTime date = DateTime.Today;
            String hour = DateTime.Now.ToLongTimeString();

            this.setRouteDirectory();
            file = this.routeDirectory + file;
            FileStream fileStream = new FileStream(file, FileMode.Append);
            StreamWriter writeFile = new StreamWriter(fileStream);

            writeFile.WriteLine("Log Date :" + date.ToString("dd_MM_yyyy") + " " + hour);
            writeFile.WriteLine(message);
            writeFile.WriteLine("  ");

            writeFile.Close();
            fileStream.Close();
        }

        private void setRouteDirectory()
        {
            this.routeDirectory = this.routeFile + @"\LogsLibreryKioskCash";

            if (!Directory.Exists(this.routeDirectory))
            {
                DirectoryInfo directory = Directory.CreateDirectory(this.routeDirectory);
            }
        }
    }
}
