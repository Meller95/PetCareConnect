using System;
using Microsoft.Data.SqlClient;

public class DB_Connection
{
    // Credentials
    private static readonly string _connectionString = 
        "Server=tcp:petcareconnectdb.database.windows.net,1433;" +
        "Initial Catalog=PetCareConnectDB;" +
        "Persist Security Info=False;" +
        "User ID=petcareconnect;" +
        "Password=MikkelMaltheH2;" +
        "MultipleActiveResultSets=False;" +
        "Encrypt=True;" +
        "TrustServerCertificate=False;" +
        "Connection Timeout=30;";

    public static SqlConnection GetConnection()
    {
        try
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("SQL Error: " + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("General Error: " + ex.Message);
            return null;
        }
    }
}
