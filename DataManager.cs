using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using OleDBDataManager;
using System.Threading;


namespace DLRMovies
{
    class DataManager
    {
        #region Class Variables
        private string connectStr = "";
        private bool goodToGo = true;
        private NameValueCollection ConfigData = null;
        private LogMngr lm = LogMngr.GetInstance();
        protected ODMDataFactory ODMDataSetFactory = new ODMDataFactory();
        private static DataManager dataMngr = null;
        #region Parameters          
        #endregion
        #endregion

        private DataManager()
        {
            // this constructor is private to force the calling program to use GetInstance()
            
        }

        public static DataManager GetInstance()
        {
            if (dataMngr == null)
            {
                CreateInstance();
            }
            return dataMngr;
        }

        private static void CreateInstance()
        {            
            Mutex configMutex = new Mutex();
            configMutex.WaitOne();
            dataMngr = new DataManager();
            configMutex.ReleaseMutex();
        }

        public int RunNonQuery(string sqlQuery)
        {
            int error = 0;
            ArrayList alResults = new ArrayList();
            ODMRequest Request = new ODMRequest();
            Request.ConnectString = connectStr;
            Request.CommandType = CommandType.Text;
            Request.Command = sqlQuery;
            try
            {
                ODMDataSetFactory.ExecuteNonQuery(ref Request);
            }
            catch (Exception ex)
            {
                lm.LogEntry("RunQuery Problem with " + Environment.NewLine + sqlQuery);
                error = 1;
            }            
            return error;
        }

        public ArrayList RunQuery(string sqlQuery, int colCount)
        {
            ArrayList alResults = new ArrayList();
            ODMRequest Request = new ODMRequest();
            Request.ConnectString = connectStr;
            Request.CommandType = CommandType.Text;
            Request.Command = sqlQuery;
            try
            {
                alResults = ODMDataSetFactory.ExecuteDataReader(ref Request, colCount);
            }
            catch (Exception ex)
            {
                lm.LogEntry("RunQuery Problem with " + Environment.NewLine + sqlQuery);
            }
            return alResults;
        }                 

        //public void DBUpdate()
        //{
        //}

        public DataSet RunDSQuery(string sqlQuery)
        {
            DataSet dsResults = new DataSet();
            ODMRequest Request = new ODMRequest();
            Request.ConnectString = connectStr;
            Request.CommandType = CommandType.Text;
            Request.Command = sqlQuery;
            try
            {
                dsResults = ODMDataSetFactory.ExecuteDataSetBuild(ref Request);
            }
            catch (Exception ex)
            {
                lm.LogEntry("RunQuery Problem " + ex.Message);
            }
            return dsResults;
        }

        public void DBWrite()
        {//FULL UPDATE -- The Full Update track has been simplified so that methods distinctly written for the Full track aren't necessary

            //PatChrgChanges pcc = new PatChrgChanges();
            
            //pcc.DollarLimits = dollarLimits;
            //pcc.MultiplierValu = multiplierValu;
            //pcc.Verbose = verbose;
            //pcc.Debug = debug;
            //foreach (string loc in locations)
            //{
            //    if (loc == "mpous")
            //        pcc.SQLSelect = BuildMPOUSSelectString();
            //    else
            //        pcc.SQLSelect = BuildHEMM_UWMSelectString(xpnse_accnt[loc].ToString());

            //    pcc.ConnectString = GetConnectString(loc);
            //    pcc.PrevCostTable = prevCostTable;
            //    pcc.Location = loc;
            //    pcc.SetNewPatientCharges();
            //}            
        }       
             
        public string GetConnectString()
        {
            ConfigData = (NameValueCollection)ConfigurationManager.GetSection("MovieCollection");
            connectStr = ConfigData.Get("connect");
            return connectStr;
        }       

        //private string GetTitleList()
        //{
        //    string select = "SELECT TITLE FROM MOVIE";
        //    return select;
        //}
    }
}
