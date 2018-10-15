using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDA.Dell.Contarct
{
    public class DBHelper
    {
        public enum DatabaseType
        {
            DEE = 0,
            DFE = 1,
            LKM = 2,
            DEELOG = 3,
            DFELOG = 4,
            DPD = 5,
            DominoReporting = 6,
            DF = 7
        }

        /// <summary>
        /// Delegate for Async invoke
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public delegate int DelegateExecuteNonQuery(string commandText, params OracleParameter[] commandParameters);

        //[ThreadStatic()]
        private static string _connectionString = null;

        public static string ConnectionString
        {
            get { return DBHelper._connectionString; }
            set { DBHelper._connectionString = value; }
        }
        public static OracleConnection GetOracleConnection(DatabaseType dbt)
        {
            string conStr = GetConnectionString(dbt);
            OracleConnectionStringBuilder ociString = new OracleConnectionStringBuilder(conStr);
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = ociString.ConnectionString;

            return conn;

        }
        public static string GetConnectionString(DatabaseType dbt)
        {
            switch (dbt)
            {
                case DatabaseType.DEE:
                    return GetDecryptedConnectionString("DEE");
                case DatabaseType.DFE:
                    return GetConnectionString("DFE");
                case DatabaseType.LKM:
                    return GetDecryptedConnectionString("LKM");
                case DatabaseType.DEELOG:
                    return GetDecryptedConnectionString("DEELOG");
                case DatabaseType.DFELOG:
                    return GetConnectionString("DFELOG");
                case DatabaseType.DPD:
                    return GetDecryptedConnectionString("DPD");
                case DatabaseType.DominoReporting:
                    return GetDecryptedConnectionString("DominoReporting");
                case DatabaseType.DF:
                    return GetDecryptedConnectionString("DF");
                default:
                    throw new ApplicationException("Unrecognized database type in GetConnectionString.");

            }
        }

        public static void SetConnectionString(DatabaseType dbt)
        {
            switch (dbt)
            {
                case DatabaseType.DEE:
                    ConnectionString = GetDecryptedConnectionString("DEE");
                    break;
                case DatabaseType.DFE:
                    ConnectionString = GetConnectionString("DFE");
                    break;
                case DatabaseType.LKM:
                    ConnectionString = GetDecryptedConnectionString("LKM");
                    break;
                case DatabaseType.DEELOG:
                    ConnectionString = GetDecryptedConnectionString("DEELOG");
                    break;
                case DatabaseType.DFELOG:
                    ConnectionString = GetConnectionString("DFELOG");
                    break;
                case DatabaseType.DPD:
                    ConnectionString = GetDecryptedConnectionString("DPD");
                    break;
                case DatabaseType.DominoReporting:
                    ConnectionString = GetDecryptedConnectionString("DominoReporting");
                    break;
                default:
                    throw new ApplicationException("Unrecognized database type in GetConnectionString.");

            }
        }

        public static string GetConnectionString(string settingName)
        {
            string strConSource;
            //strConSource = ConfigurationManager.ConnectionStrings[settingName].ConnectionString;
            strConSource = DecryptUtility.GetValue("DBConStr");

            strConSource = ConfigurationManager.ConnectionStrings[settingName].ConnectionString;

            if (String.IsNullOrEmpty(strConSource))
            {
                throw new ApplicationException("Unable to locate connection string for" + settingName);
            }
            return strConSource;
        }


        public static bool IsSqlOk(string returnedValue)
        {
            return string.Compare(returnedValue, "SQL_OK", true) == 0 ? true : false;
        }
        public static string GetErrorCodeValue(string returnedValue)
        {
            return string.Compare(returnedValue, "SQL_OK", true) == 0 ? "SQL_OK" : returnedValue;
        }
        public static string GetDecryptedConnectionString(string settingName)
        {
            string strConSource;

            switch (settingName)
            {
                case "DEE":
                    strConSource = DecryptUtility.GetValue("DEEConstr");
                    break;
                case "LKM":
                    strConSource = DecryptUtility.GetValue("LKMConstr");
                    break;
                case "DEELOG":
                    strConSource = DecryptUtility.GetValue("DEELOGConstr");
                    break;
                case "DPD":
                    strConSource = DecryptUtility.GetValue("DPDConstr");
                    break;
                case "DominoReporting":
                    strConSource = DecryptUtility.GetValue("DominoReportingConstr");
                    break;
                case "DF":
                    strConSource = DecryptUtility.GetValue("DFConstr");
                    break;
                default:
                    throw new ApplicationException("Unrecognized database type in GetConnectionString.");
            }

            if (String.IsNullOrEmpty(strConSource))
            {
                throw new ApplicationException("Unable to locate connection string for" + settingName);
            }
            return strConSource;
        }
        public static void CloseConnection(OracleConnection oci)
        {
            if (oci != null && oci.State == ConnectionState.Broken)
            {
                OracleConnection.ClearPool(oci);
                oci.Close();
            }
            else
                if (oci != null && oci.State != ConnectionState.Closed)
                oci.Close();
        }
        #region Private utility methods & constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new OracleProvider()".
        //private OracleClient() { }
        /// <summary>
        /// This method is used to attach array's of OracleParameters to an OracleCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of OracleParameters tho be added to command</param>
        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of OracleParameters.
        /// </summary>
        /// <param name="commandParameters">array of OracleParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                //do nothing if we get no data
                return;
            }

            // we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            //iterate through the OracleParameters, assigning the values from the corresponding position in the 
            //value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the OracleCommand to be prepared</param>
        /// <param name="connection">a valid OracleConnection, on which to execute this command</param>
        /// <param name="transaction">a valid OracleTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void PrepareCommand(OracleConnection connection, OracleCommand command, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or Oracle statement)
            command.CommandText = commandText;


            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }




        public static OracleCommand GetCommand(string commandName, DatabaseType dbt)
        {
            // GetConnection string	
            using (OracleConnection ociConnection = GetOracleConnection(dbt))
            {
                try
                {
                    ociConnection.Open();
                }
                catch (OracleException oex)
                {
                    throw new Exception("Error Opening an Oracle Connection", oex.InnerException);
                }

                OracleCommand cmd = ociConnection.CreateCommand();
                cmd.CommandText = commandName;
                cmd.CommandType = CommandType.StoredProcedure;

                return cmd;
            }
        }
        public static DataSet CreateDataSet(OracleCommand cmd)
        {
            OracleDataAdapter dAdapt = new OracleDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                dAdapt.Fill(ds);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                if (cmd != null)
                    CloseConnection(cmd.Connection);
            }

            return ds;
        }
        public static int ExecuteNonQuery(OracleCommand cmd)
        {
            int result;
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                if (cmd != null)
                    CloseConnection(cmd.Connection);
            }

            return result;
        }
        public static OracleDataReader ExecuteReader(OracleCommand cmd)
        {
            OracleDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                if (cmd != null)
                    CloseConnection(cmd.Connection);
            }

            return reader;
        }

        /// <summary>
        /// Creates an Oracle Input Parameter attributes
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="ot">Oracle type</param>
        /// <param name="value">Parameter Value</param>
        /// <param name="paramSize">Parameter Size</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddInParameter(string paramName, OracleDbType paramType, object paramValue, int paramSize)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType, paramSize);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.Input;
            return cmdParam;
            //return new OracleParameter(paramName, paramType, paramSize, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, paramValue);
        }

        /// <summary>
        /// Creates an Oracle Input Parameter attributes(Overload)
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="ot">Oracle type</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddInParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.Input;
            return cmdParam;
            //return new OracleParameter(paramName, paramType, paramSize, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, paramValue);
        }

        public static OracleParameter AddAssociativeArrayInParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.Input;
            cmdParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            return cmdParam;
            //return new OracleParameter(paramName, paramType, paramSize, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, paramValue);
        }

        public static OracleParameter AddAssociativeArrayOutParameter(string paramName, OracleDbType paramType, int size)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Direction = ParameterDirection.Output;
            cmdParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            cmdParam.Size = size;
            return cmdParam;
        }

        public static OracleParameter AddAssociativeArrayOutParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.Output;
            cmdParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            return cmdParam;
            //return new OracleParameter(paramName, paramType, paramSize, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, paramValue);
        }

        /// <summary>
        /// Creates an Oracle Output Parameter attributes
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="ot">Oracle type</param>
        /// <param name="value">Parameter Value</param>
        /// <param name="paramSize">Parameter Size</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddOutParameter(string paramName, OracleDbType paramType, int paramSize)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType, paramSize);
            cmdParam.Direction = ParameterDirection.Output;
            return cmdParam;
        }

        /// <summary>
        /// Creates an Oracle Output Parameter attributes
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="ot">Oracle type</param>
        /// <param name="value">Parameter Value</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddOutParameter(string paramName, OracleDbType paramType)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Direction = ParameterDirection.Output;
            return cmdParam;
        }

        /// <summary>
        /// Creates an Oracle InputOuput Parameter
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="paramType">Oracle Type</param>
        /// <param name="paramValue">Parameter Value</param>
        /// <param name="paramSize">Parameter Size</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddInOutParameter(string paramName, OracleDbType paramType, object paramValue, int paramSize)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType, paramSize);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.InputOutput;
            return cmdParam;
        }

        /// <summary>
        /// Creates an Oracle InputOuput Parameter
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="paramType">Oracle Type</param>
        /// <param name="paramValue">Parameter Value</param>
        /// <param name="paramSize">Parameter Size</param>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddInOutParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.InputOutput;
            return cmdParam;
        }


        /// <summary>
        /// Creates an Oracle Return Parameter
        /// </summary>
        /// <returns>Returns an OracleParameter object</returns>
        public static OracleParameter AddReturnParameter()
        {
            OracleParameter cmdParam = new OracleParameter("RetVal", OracleDbType.Int32);
            cmdParam.Direction = ParameterDirection.ReturnValue;
            return cmdParam;
        }

        public static void AddCache(string cacheName, object objectToCache, double cacheDurationInSeconds)
        {

        }

        public static object GetCache(string cacheName)
        {
            return new object();
        }

        public static void RemoveCache(string cacheName)
        {

        }

        #endregion private utility methods & constructors
        #region ExecuteNonQuery

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery("PublishOrders");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery("PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();
                    //call the overload that takes a connection in place of the connection string
                    return ExecuteNonQuery(cn, commandText, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }
        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery("PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <param name="isHub">Determine the call is to Affinty or Hub</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string commandText, bool isHub, params OracleParameter[] commandParameters)
        {

            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();
                    //call the overload that takes a connection in place of the connection string
                    return ExecuteNonQuery(cn, commandText, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute an OracleCommand (that returns no resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "SaveOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(connection, cmd, CommandType.StoredProcedure, commandText, commandParameters);

                //finally, execute the command.
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(connection);
            }
        }

        #endregion ExecuteNonQuery
        #region ExecuteDataSet

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("GetOrders");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset("GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataset(commandText, CommandType.StoredProcedure, commandParameters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string commandText, CommandType commandType, params OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteDataset(cn, commandText, commandType, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleConnection connection, string commandText, CommandType commandType, params OracleParameter[] commandParameters)
        {
            OracleTransaction transaction = null;
            try
            {

                //Transaction added for storing global temporary Table
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(connection, cmd, commandType, commandText, commandParameters);

                //create the DataAdapter & DataSet
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();

                //fill the DataSet using default values for DataTable names, etc.
                //Fix for fortify adding name of Table as Table.Additional result sets are named by appending integral values to the specified table name ( for example, "Table", "Table1", "Table2", and so on. ).
                da.Fill(ds, "Table");

                //Transaction added for storing global temporary Table
                transaction.Commit();

                //return the dataset
                return ds;
            }
            catch (OracleException)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();

                CloseConnection(connection);
            }
        }


        /// <summary>
        /// Execute an OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataset(connection, commandText, CommandType.StoredProcedure, commandParameters);
        }

        #endregion ExecuteDataSet
        #region ExecuteReader

        /// <summary>
        /// this enum is used to indicate weather the connection was provided by the caller, or created by OracleClient, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum OracleConnectionOwnership
        {
            /// <summary>Connection is owned and managed by OracleClient</summary>
            Internal,
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader("GetOrders");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <returns>an OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteReader(commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader("GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string commandText, OracleParameter[] commandParameters)
        {
            OracleConnection cn = new OracleConnection(ConnectionString);
            try
            {
                //create & open an OraclebConnection

                cn.Open();


                //call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(cn, commandText, commandParameters);
            }
            catch (OracleException)
            {

                //if we fail to return the OracleDataReader, we need to close the connection ourselves
                if (cn.State == ConnectionState.Open)
                    cn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader("GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string commandText,
            OracleParameter[] commandParameters, CommandBehavior cmdBehavior)
        {
            OracleConnection cn = new OracleConnection(ConnectionString);
            try
            {
                //create & open an OraclebConnection

                cn.Open();


                //call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(cn, commandText, commandParameters, cmdBehavior);
            }
            catch (OracleException)
            {

                //if we fail to return the OracleDataReader, we need to close the connection ourselves
                if (cn.State == ConnectionState.Open)
                    cn.Close();
                throw;
            }
        }



        public static void CloseReader(OracleDataReader reader)
        {
            if (reader != null)
            {
                if (!reader.IsClosed)
                    reader.Close();
                reader.Dispose();
                reader = null;
            }
        }

        /// <summary>
        /// Create and prepare an OracleCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">a valid OracleConnection, on which to execute this command</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <returns>OracleDataReader containing the results of the command</returns>
        private static OracleDataReader ExecuteReader(OracleConnection connection, string commandText, OracleParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(connection, cmd, CommandType.StoredProcedure, commandText, commandParameters);

            //create a reader
            OracleDataReader dr;

            dr = cmd.ExecuteReader((CommandBehavior)((int)CommandBehavior.CloseConnection));

            return (OracleDataReader)dr;

        }

        /// <summary>
        /// Create and prepare an OracleCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">a valid OracleConnection, on which to execute this command</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
        /// <param name="commandParameters">an array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <returns>OracleDataReader containing the results of the command</returns>
        private static OracleDataReader ExecuteReader(OracleConnection connection, string commandText, OracleParameter[] commandParameters, CommandBehavior cmdBehavior)
        {
            //create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(connection, cmd, CommandType.StoredProcedure, commandText, commandParameters);

            //create a reader
            OracleDataReader dr;

            dr = cmd.ExecuteReader((CommandBehavior)((int)cmdBehavior));

            return (OracleDataReader)dr;

        }

        #endregion ExecuteReader
        #region ExecuteScalar

        /// <summary>
        /// Execute an OracleCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar("GetOrderCount");
        /// </remarks>
        /// <param name="commandText">the stored procedure name or T-Oracle command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string commandText)
        {
            //pass through the call providing null for the set of OracleParameters
            return ExecuteScalar(commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar("GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandText">the stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteScalar(cn, commandText, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }
        /// <summary>
        /// New method added to call function
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteReturnScalar(string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                object result = null;
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    ExecuteScalar(cn, commandText, commandParameters);
                    foreach (OracleParameter orapara in commandParameters)
                    {
                        if (orapara.Direction == ParameterDirection.ReturnValue)
                        {

                            result = orapara.Value;
                        }
                    }


                    return result;
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute an OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid OracleConnection</param>
        /// <param name="commandText">the stored procedure name or T-OleDb command</param>
        /// <param name="commandParameters">an array of OracleParameters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(connection, cmd, CommandType.StoredProcedure, commandText, commandParameters);

                //execute the command & return the results
                return cmd.ExecuteScalar();
            }
            finally
            {
                CloseConnection(connection);
            }


        }

        #endregion ExecuteScalar

        public static DataSet ExecuteTransactionCommand(OracleCommand cmd)
        {
            DataSet ds = new DataSet();
            // try
            //{

            int numrows = cmd.ExecuteNonQuery();

            // Dynamically get any non-cursor output parameters and load them into dataset
            ds.Tables.Add("UpdateInformation");
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                if ((cmd.Parameters[i].Direction == ParameterDirection.Output || cmd.Parameters[i].Direction == ParameterDirection.InputOutput) && (cmd.Parameters[i].OracleDbType != OracleDbType.RefCursor))
                {
                    ds.Tables[0].Columns.Add(cmd.Parameters[i].ParameterName.ToString());
                }
            }
            DataRow dr = ds.Tables[0].NewRow();
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                if ((cmd.Parameters[i].Direction == ParameterDirection.Output || cmd.Parameters[i].Direction == ParameterDirection.InputOutput) && (cmd.Parameters[i].OracleDbType != OracleDbType.RefCursor))
                {
                    dr[cmd.Parameters[i].ParameterName.ToString()] = cmd.Parameters[i].Value.ToString();
                }
            }

            ds.Tables[0].Rows.Add(dr);

            return ds;

            /*}
            catch (Exception ex)
            {
                throw new Exception("Error occurred in CommonUtilities method ExecuteTransactionCommand: ", ex);
            }*/
        }

        public static T FromDB<T>(object value) { return value == DBNull.Value ? default(T) : (T)value; }

        public static int FromDBDecimalToInt(object value)
        {
            return (value.GetType() == typeof(System.Decimal)) ? Convert.ToInt32(FromDB<Decimal>(value)) : 0;
        }




        public static object ToDB<T>(T value) { return value == null ? (object)DBNull.Value : value; }

        /// <summary>
        /// Added only for Integration Testing Purpose
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        internal static object ExecuteScalarInlineText(string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteScalarInlineText(cn, commandText, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        /// <summary>
        /// Added only for Integration Testing Purpose
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private static object ExecuteScalarInlineText(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(connection, cmd, CommandType.Text, commandText, commandParameters);

                //execute the command & return the results
                return cmd.ExecuteScalar();
            }
            finally
            {
                CloseConnection(connection);
            }


        }

        /// <summary>
        ///  Added only for Integration Testing Purpose
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        internal static void ExecuteNonQueryInlineText(string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    ExecuteNonQueryInlineText(cn, commandText, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        /// <summary>
        /// Added only for Integration Testing Purpose
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        private static void ExecuteNonQueryInlineText(OracleConnection connection, string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(connection, cmd, CommandType.Text, commandText, commandParameters);

                //execute the command & return the results
                cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(connection);
            }
        }

        /// <summary>
        /// Can be reused for all database publisher
        /// </summary>
        /// <param name="ProcName"></param>
        /// <param name="cmdparms"></param>
        /// <returns></returns>
        public static DataSet CreateConnAndCmd(string ProcName, OracleParameter[] cmdparms, DatabaseType databaseType)
        {
            DataSet datasetData = new DataSet();
            try
            {
                using (OracleConnection conn = DBHelper.GetOracleConnection(databaseType))
                {
                    using (OracleCommand cmd = new OracleCommand(ProcName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(cmdparms);
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        conn.Open();
                        adapter.Fill(datasetData);
                    }

                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                }
            }
            catch (OracleException)
            {
                throw;
            }
            return datasetData;
        }

        public static bool ExecuteNonQuery(string procName, OracleParameter[] cmdParams, DatabaseType databaseType)
        {
            bool status = false;
            using (OracleConnection oraConn = DBHelper.GetOracleConnection(databaseType))
            {
                OracleCommand _oraCmd = new OracleCommand(procName, oraConn);
                _oraCmd.CommandType = CommandType.StoredProcedure;
                _oraCmd.Parameters.AddRange(cmdParams);
                oraConn.Open();
                int result = _oraCmd.ExecuteNonQuery();
                if (cmdParams[cmdParams.Count() - 1].Value.ToString() == "SUCCESS")
                {
                    status = true;
                }
            }
            return status;
        }

        public static object GetParamValue(string param)
        {
            OracleParameter DBNullVal = new OracleParameter();
            DBNullVal.Value = DBNull.Value;
            return param == DateTime.MinValue.ToString() ? DBNullVal.Value : (string.IsNullOrEmpty(param) ? DBNullVal.Value : param);
        }


        public static int ExecuteNonQueryArray(string commandText, int arrayLength, OracleParameter[] commandParameters)
        {
            try
            {
                //create & open an OracleConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(ConnectionString))
                {
                    cn.Open();
                    //call the overload that takes a connection in place of the connection string
                    return ExecuteNonQueryArray(cn, commandText, arrayLength, commandParameters);
                }
            }
            catch (OracleException)
            {
                throw;
            }
        }

        private static int ExecuteNonQueryArray(OracleConnection connection, string commandText, int arrayLength, params OracleParameter[] commandParameters)
        {
            try
            {
                //create a command and prepare it for execution
                OracleCommand cmd = new OracleCommand();
                PrepareCommandArray(connection, cmd, CommandType.StoredProcedure, commandText, commandParameters, arrayLength);

                //finally, execute the command.
                return cmd.ExecuteNonQuery();

            }
            finally
            {
                CloseConnection(connection);
            }
        }

        public static void PrepareCommandArray(OracleConnection connection, OracleCommand command, CommandType commandType, string commandText, OracleParameter[] commandParameters, int arrayLength)
        {
            PrepareCommand(connection, command, commandType, commandText, commandParameters);
            command.ArrayBindCount = arrayLength;
        }

        public static OracleParameter AddInParameterArray(string paramName, OracleDbType paramType, object paramValue, int arrayLength)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType);
            cmdParam.Value = paramValue;
            cmdParam.Direction = ParameterDirection.Input;
            cmdParam.ArrayBindSize = new int[arrayLength];
            return cmdParam;
            //return new OracleParameter(paramName, paramType, paramSize, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, paramValue);
        }
        public static OracleParameter AddOutParameterArray(string paramName, OracleDbType paramType, int paramSize, int arrayLength)
        {
            OracleParameter cmdParam = new OracleParameter(paramName, paramType, paramSize);
            cmdParam.ArrayBindSize = new int[arrayLength];
            cmdParam.Direction = ParameterDirection.Output;
            return cmdParam;
        }


        public static List<T> CreateArrayBindItemList<T>(T item, int itemLength)
        {
            List<T> itemList = new List<T>(itemLength);
            for (int order = 0; order < itemLength; order++)
                itemList.Add(item);
            return itemList;
        }
    }
}
