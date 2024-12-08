using System.Data;
using MySql.Data.MySqlClient;

namespace LUMI.DataAccess;

public class DBController {
    private readonly static MySqlConnection _mySqlConnection = 
        new("Server=localhost;Port=3306;Database=lumi;User=root;Password=root;"); 
    
    public static MySqlConnection GetConnection() => _mySqlConnection;

    public static async Task OpenConnectionAsync() {
        if(_mySqlConnection.State == ConnectionState.Closed)
            await _mySqlConnection.OpenAsync();
    }
    
    public static async Task CloseConnectionAsync() {
        if(_mySqlConnection.State == ConnectionState.Open)
            await _mySqlConnection.CloseAsync();
    }
}