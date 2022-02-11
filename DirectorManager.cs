using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OleDBDataManager;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using System.Data;

namespace DLRMovies
{
    class DirectorManager
    {
        private LogMngr lm = LogMngr.GetInstance();
        private DataManager dm = DataManager.GetInstance();
        private MovieManager mm = MovieManager.GetInstance();
        private static DirectorManager dmMngr = null;
        private ArrayList alResults = new ArrayList();
        private int currentDirID = 0;
        private string imdbDir = "";

        #region Parameters  

        public string ImdbDir
        {
            set { imdbDir = value; }
            get { return imdbDir; }
        }
        public int CurrentDirID
        {
            set { currentDirID = value; }
            get { return currentDirID; }
        }
        #endregion

        private DirectorManager()
        {
            // this constructor is private to force the calling program to use GetInstance()
        }

        public static DirectorManager GetInstance()
        {
            if (dmMngr == null)
            {
                CreateInstance();
            }
            return dmMngr;
        }

        private static void CreateInstance()
        {
            Mutex configMutex = new Mutex();
            configMutex.WaitOne();
            dmMngr = new DirectorManager();
            configMutex.ReleaseMutex();
        }

        public ArrayList GetMovieDirector()
        {
            int movieID = mm.CurrentMovieID;
            string query = "SELECT NAME FROM DIRECTORS JOIN MOVIEDIRECTOR MD ON MD.DID = DIRECTORS.ID " +
                            "WHERE MD.MID = " + movieID;

            alResults = RunDSQuery(query);
            return alResults;
        }

        private ArrayList RunDSQuery(string query)
        {
            alResults.Clear();
            DataSet dsResults = new DataSet();
            dsResults = dm.RunDSQuery(query);
            object[] rowData;
            string colData = "";
            string outResults = "";
            try
            {
                int colCount = dsResults.Tables[0].Columns.Count;
                for (int x = 0; x < dsResults.Tables[0].Rows.Count; x++)
                {
                    colData = dsResults.Tables[0].Rows[x].ItemArray[0].ToString();
                    outResults = colData;                        
                    alResults.Add(outResults.Trim());
                    //outResults += Environment.NewLine;
                }
       //         alResults.Add(outResults.Trim());
            }
            catch (Exception ex)
            {
                lm.LogEntry(query + Environment.NewLine + "Is not a valid statement.");
                alResults.Add("There is a problem with your query.");
                return alResults;
            }
            return alResults;
        }

        public ArrayList GetDirectorList()
        {
            string query = "SELECT NAME FROM DIRECTORS ORDER BY NAME";
            alResults = dm.RunQuery(query, 1);            
            return alResults;
        }

        public ArrayList CheckResults(string currentDirector)
        {
            string query = "SELECT NAME FROM DIRECTORS WHERE NAME LIKE '%" + currentDirector + "%' ORDER BY NAME";
            return dm.RunQuery(query, 1);
        }

        public ArrayList GetDirectorDetails(string currentDirector)
        {
            string query = "SELECT * FROM DIRECTORS WHERE NAME = '" + currentDirector + "'";
            alResults = dm.RunQuery(query, 3);
            return alResults;
        }
        public ArrayList GetDirectorMovies()
        {
            string query = "SELECT TITLE FROM MOVIE " +
                            "JOIN MOVIEDIRECTOR MD ON MD.MID = MOVIE.ID " +
                            "WHERE MD.DID = " + currentDirID + " ORDER BY TITLE";
            return dm.RunQuery(query, 1);
        }
    }
}
