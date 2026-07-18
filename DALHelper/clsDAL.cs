using ShineWebMobileAPI.BuisnessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace ShineWebMobileAPI.DALHelper
{
    public class clsDAL
    {
        SqlConnection aSqlconnection = new SqlConnection();
        SqlTransaction aSqlTransaction;
        public string Connectionstr(string CompanyCode)
        {
            string COns = Connection.GetConnectionString(CompanyCode);
            return clsEncryptDecrypt.Decrypt(COns);
        }
        public DataTable dl_ExecuteSqlQuery(string CompanyCode,string strquery)
        {
            DataTable dt_SqlQuery = new DataTable();
            try
            {
                using (aSqlconnection = new SqlConnection(Connectionstr(CompanyCode)))
                {
                    aSqlconnection.Open();
                    SqlDataAdapter dacmd = new SqlDataAdapter(strquery, aSqlconnection);
                    dacmd.Fill(dt_SqlQuery);
                    //aSqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt_SqlQuery;
        }
        public DataTable dl_ExecuteParamSP(string CompanyCode, string strSPName, params object[] parameters)
        {
            DataTable dt_returnvalue = new DataTable();
            int Errindex = 0;
            try
            {
                using (aSqlconnection = new SqlConnection(Connectionstr(CompanyCode)))
                {
                    aSqlconnection.Open();
                    SqlCommand cmd = new SqlCommand(strSPName, aSqlconnection);
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(cmd);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Errindex = i;
                        cmd.Parameters[i + 1].Value = parameters[i];
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt_returnvalue);
                    //aSqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt_returnvalue;
        }
        public DataSet dl_ExecuteParamSPDataset(string CompanyCode, string strSPName, params object[] parameters)
        {
            DataSet dt_returnvalue = new DataSet();
            int Errindex = 0;
            try
            {
                using (aSqlconnection = new SqlConnection(Connectionstr(CompanyCode)))
                {
                    aSqlconnection.Open();
                    SqlCommand cmd = new SqlCommand(strSPName, aSqlconnection);
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(cmd);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Errindex = i;
                        cmd.Parameters[i + 1].Value = parameters[i];
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt_returnvalue);
                    //aSqlconnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt_returnvalue;
        }
        public void dl_Transaction(string CompanyCode, int Action)
        {
            if (Action == 1)
            {
                aSqlconnection = new SqlConnection(Connectionstr(CompanyCode));
                aSqlconnection.Open();
                aSqlTransaction = aSqlconnection.BeginTransaction();
            }
            else if (Action == 2)
            {
                aSqlTransaction.Commit();
                aSqlconnection.Close();
            }
            else if (Action == 3)
            {
                aSqlTransaction.Rollback();
                aSqlconnection.Close();
            }
        }
        public DataTable dl_ManageTrans(string CompanyCode, string strStoredProc, params object[] obj)
        {
            try
            {
                DataTable dt_returnvalue = new DataTable();

                SqlCommand cmd = new SqlCommand(strStoredProc, aSqlconnection, aSqlTransaction);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(cmd);
                for (int i = 0; i < obj.Length; i++)
                {
                    if (cmd.Parameters[i + 1].SqlDbType != SqlDbType.Structured)
                    {
                        cmd.Parameters[i + 1].Value = obj[i];
                    }
                    else
                    {
                        string strTypeName = cmd.Parameters[i + 1].TypeName;
                        int nIndex = strTypeName.IndexOf(".");
                        cmd.Parameters[i + 1].TypeName = strTypeName.Substring(nIndex + 1);
                        cmd.Parameters[i + 1].Value = obj[i];
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt_returnvalue);
                return dt_returnvalue;
            }
            catch (Exception ex)
            {
                aSqlTransaction.Rollback();
                throw ex;
            }
        }
    }
}