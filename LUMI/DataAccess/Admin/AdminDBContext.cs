using MySql.Data.MySqlClient;

namespace LUMI.DataAccess.Admin;

public class AdminDBContext {
    public static async Task<bool> IsEqualAsnyc(Models.Admin admin) {
        var connection = DBController.GetConnection();

        try {
            await DBController.OpenConnectionAsync();
            
            string checkQuery = "SELECT COUNT(*) FROM `admins` WHERE `email` = @Email AND `password` = @Password;";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", admin.Email);
            checkCmd.Parameters.AddWithValue("@Password", admin.Password);
            
            var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (exists) {
                Console.WriteLine("Admin exists in the database.");
                return true;
            } else {
                Console.WriteLine("Invalid email or password.");
                return false;
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
        finally {
            await DBController.CloseConnectionAsync();
        }
    }
    
    public static async Task<Models.Admin?> GetAdminDataAsnyc(Models.Admin admin) {
        var connection = DBController.GetConnection();

        try {
            await DBController.OpenConnectionAsync();
            
            string checkQuery = "SELECT FROM `admins` WHERE `email` = @Email";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", admin.Email);
            
            using var reader = await checkCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                return new Models.Admin {
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Password = reader.GetString(reader.GetOrdinal("password"))
                };
            } else {
                return null;
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
        finally {
            await DBController.CloseConnectionAsync();
        }
    }
}