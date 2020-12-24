using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class UserService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.UserController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public UserService(IConfiguration config, ILogger<Controllers.UserController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.UserResponse CreateUser(Models.UserRequest value, Guid organization)
        {
            Models.UserResponse response = new Models.UserResponse();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO users (id, first_name, last_name, description, email_address, password, security_token, organization, primary_user, role, created, created_by) " +
                    "VALUES (@id, @first_name, @last_name, @description, @email_address, @password, @security_token, @organization, @primary_user, @role, @created, @created_by)";

                //Create a new User Id UUID
                Guid userIdGuid = Guid.NewGuid();
                //Create a new Security Token UUID
                Guid securityTokenGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, userIdGuid);
                        command.Parameters.AddWithValue("@first_name", NpgsqlTypes.NpgsqlDbType.Text, value.FirstName);
                        command.Parameters.AddWithValue("@last_name", NpgsqlTypes.NpgsqlDbType.Text, value.LastName);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.Description);
                        command.Parameters.AddWithValue("@email_address", NpgsqlTypes.NpgsqlDbType.Text, value.EmailAddress);
                        command.Parameters.AddWithValue("@password", NpgsqlTypes.NpgsqlDbType.Text, value.Password);
                        command.Parameters.AddWithValue("@security_token", NpgsqlTypes.NpgsqlDbType.Uuid, securityTokenGuid);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@primary_user", NpgsqlTypes.NpgsqlDbType.Bigint, 0);
                        command.Parameters.AddWithValue("@role", NpgsqlTypes.NpgsqlDbType.Bigint, value.Role);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "user created";
                        response.Id = userIdGuid;
                        response.SecurityToken = securityTokenGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "user creation failed");
                response.Status = "error";
                response.Message = "user creation failed";
                response.Id = errorGuid;
                response.SecurityToken = errorGuid;
                return response;
            }
        }


        public List<Models.UserLimitedResponse> GetAllUsers(Guid organization)
        {
            List<Models.UserLimitedResponse> UserList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM users WHERE organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                //Initialize a User 
                                Models.UserLimitedResponse user = null;
                                //Create a List to hold multiple Users
                                UserList = new List<Models.UserLimitedResponse>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    user = new Models.UserLimitedResponse();

                                    user.Id = Guid.Parse(reader["id"].ToString());
                                    user.FirstName = Convert.ToString(reader["first_name"]).Trim();
                                    user.LastName = Convert.ToString(reader["last_name"]).Trim();
                                    user.Description = Convert.ToString(reader["description"]).Trim();
                                    user.EmailAddress = Convert.ToString(reader["email_address"]).Trim();
                                    user.Role = Convert.ToInt64(reader["role"]);
                                    user.Created = Convert.ToDateTime(reader["created"]);
                                    user.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                    //Add to List
                                    UserList.Add(user);
                                }
                            }
                        }
                    }
                }
                return UserList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving users");
                return UserList;
            }
        }


        public Models.UserLimitedResponse GetUser(Guid id, Guid organization)
        {
            Models.UserLimitedResponse user = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM users WHERE id = @id AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    user = new Models.UserLimitedResponse();

                                    user.Id = Guid.Parse(reader["id"].ToString());
                                    user.FirstName = Convert.ToString(reader["first_name"]).Trim();
                                    user.LastName = Convert.ToString(reader["last_name"]).Trim();
                                    user.Description = Convert.ToString(reader["description"]).Trim();
                                    user.EmailAddress = Convert.ToString(reader["email_address"]).Trim();
                                    user.Role = Convert.ToInt64(reader["role"]);
                                    user.Created = Convert.ToDateTime(reader["created"]);
                                    user.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving user");
                return user;
            }
        }






        public Models.Response DeleteUser(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM users WHERE id = @id AND organization = @organization AND primary_user <> 1";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();
                        int rows = command.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            //Return Success
                            response.Status = "success";
                            response.Message = "user deleted";
                            response.Id = id;
                        }
                        else
                        {
                            //Return Failure
                            response.Status = "error";
                            response.Message = "user deletion failed";
                            response.Id = id;
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "user deletion failed");
                response.Status = "error";
                response.Message = "user deletion failed";
                response.Id = id;
                return response;
            }
        }



    }
}
