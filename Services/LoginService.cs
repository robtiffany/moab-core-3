using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class LoginService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.LoginController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public LoginService(IConfiguration config, ILogger<Controllers.LoginController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.LoginResponse Login(string emailAddress, string password)
        {
            Models.LoginResponse loginResponse = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT id, security_token FROM users WHERE email_address = @email_address AND password = @password";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@email_address", NpgsqlTypes.NpgsqlDbType.Text, emailAddress);
                        command.Parameters.AddWithValue("@password", NpgsqlTypes.NpgsqlDbType.Text, password);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    loginResponse = new Models.LoginResponse();

                                    loginResponse.Id = Guid.Parse(reader["id"].ToString());
                                    loginResponse.SecurityToken = Guid.Parse(reader["security_token"].ToString());
                                }
                            }
                        }
                    }
                }
                return loginResponse;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving id and security token");
                return loginResponse;
            }
        }

    }
}
