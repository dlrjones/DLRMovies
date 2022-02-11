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

namespace DLRMovies
{
    class CastManager
    {
        private LogMngr lm = LogMngr.GetInstance();
        private DataManager dm = DataManager.GetInstance();
        private MovieManager mm = MovieManager.GetInstance();
        private ArrayList alResults = new ArrayList();
        private static CastManager castMngr = null;
        private int currentCastID = 0;
        private string imdbCast = "";

        #region Parameters   
        public string ImdbCast
        {
            set { imdbCast = value; }
            get { return imdbCast; }
        }
        public int CurrentCastID
        {
            set { currentCastID = value; }
            get { return currentCastID; }
        }
        #endregion

        private CastManager()
        {
            // this constructor is private to force the calling program to use GetInstance()
        }

        public static CastManager GetInstance()
        {
            if (castMngr == null)
            {
                CreateInstance();
            }
            return castMngr;
        }

        private static void CreateInstance()
        {
            Mutex configMutex = new Mutex();
            configMutex.WaitOne();
            castMngr = new CastManager();
            configMutex.ReleaseMutex();
        }

        public ArrayList GetCastList()
        {
            int movieID = mm.CurrentMovieID;
                string query = "SELECT NAME FROM ACTORS JOIN CAST ON CAST.AID = ACTORS.ID " +
                                "WHERE CAST.MID = " + movieID + " ORDER BY NAME";
            alResults = dm.RunQuery(query, 1); ;
            return alResults;
        }

        public ArrayList GetActorList()
        {
            string query = "SELECT NAME, IMDB FROM ACTORS ORDER BY NAME";
            alResults = dm.RunQuery(query, 1);
            return alResults;
        }

        public ArrayList CheckResults(string currentActor)
        {
            string query = "SELECT NAME,IMDB FROM ACTORS WHERE NAME = '" + currentActor + "'";
            return dm.RunQuery(query, 1);
        }

        public ArrayList CheckMultiResults(string currentActor)
        {
            string query = "SELECT NAME FROM ACTORS WHERE NAME LIKE '%" + currentActor + "%'";
            return dm.RunQuery(query, 1);
        }

        public ArrayList GetActorDetails(string currentActor)
        {
            string query = "SELECT * FROM ACTORS WHERE NAME LIKE '%" + currentActor + "%'";
            alResults = dm.RunQuery(query, 3);
            return alResults;
        }

        public ArrayList GetActorMovies()
        {
            string query = "SELECT TITLE FROM MOVIE " +
                            "JOIN CAST ON CAST.MID = MOVIE.ID " +
                            "WHERE CAST.AID = " + currentCastID;
            return dm.RunQuery(query, 1);
        }
    }
}
