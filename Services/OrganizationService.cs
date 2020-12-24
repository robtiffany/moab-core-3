using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class OrganizationService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.OrganizationController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public OrganizationService(IConfiguration config, ILogger<Controllers.OrganizationController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.OrganizationResponse Initialize(Models.OrganizationRequest value)
        {
            Models.OrganizationResponse response = new Models.OrganizationResponse();

            bool existingOrganization;
            Guid organization;
            Guid group;
            Guid user;
            Guid securityToken;
            Guid userIdGuid;

            try
            {
                //Test to see if an Organization of the same name already exists
                if (existingOrganization = TestForExistingOrganization(value))
                {
                    //Log Failure
                    response.Status = "error";
                    response.Message = "organization already exists";
                    response.Id = errorGuid;
                    response.SecurityToken = errorGuid;
                    return response;
                }
                else
                {
                    //Create a new User Id UUID
                    userIdGuid = Guid.NewGuid();

                    //Create a new Organization
                    if ((organization = CreateOrganization(value, userIdGuid)) != errorGuid)
                    {
                        if ((group = CreateGroup(organization, userIdGuid)) != errorGuid)
                        {
                            //Create a User with Creator Role priveleges within the new Organization
                            if ((user = CreateUser(value, organization, userIdGuid, out securityToken)) != errorGuid)
                            {

                                //Log Success
                                response.Status = "success";
                                response.Message = "initialization succeeded";
                                response.Id = user;
                                response.SecurityToken = securityToken;
                                return response;
                            }
                            else
                            {
                                //Log Failure
                                response.Status = "error";
                                response.Message = "user was not created";
                                response.Id = errorGuid;
                                response.SecurityToken = errorGuid;
                                return response;
                            }
                        }
                        else
                        {
                            //Log Failure
                            response.Status = "error";
                            response.Message = "group was not created";
                            response.Id = errorGuid;
                            response.SecurityToken = errorGuid;
                            return response;
                        }
                    }
                    else
                    {
                        //Log Failure
                        response.Status = "error";
                        response.Message = "organization was not created";
                        response.Id = errorGuid;
                        response.SecurityToken = errorGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "initialization failed");

                response.Status = "error";
                response.Message = "initialization failed";
                response.Id = errorGuid;
                return response;
            }
        }

        private bool TestForExistingOrganization(Models.OrganizationRequest value)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT COUNT(*) FROM organizations WHERE name = @name";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.OrganizationName);
                        command.Prepare();
                        var count = Convert.ToInt64(command.ExecuteScalar());
                        if (count >= 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization name verification failed");
                return false;
            }
        }

        private Guid CreateOrganization(Models.OrganizationRequest value, Guid userId)
        {
            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO organizations (id, name, description, created, created_by) " +
                    "VALUES (@id, @name, @description, @created, @created_by)";

                Guid idGuid = Guid.NewGuid();                
                
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.OrganizationName);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.OrganizationDescription);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, userId);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        return idGuid;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization creation failed");

                return errorGuid;
            }
        }



        private Guid CreateGroup(Guid organization, Guid userId)
        {
            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO groups (id, name, description, organization, primary_group, created, created_by) " +
                    "VALUES (@id, @name, @description, @organization, @primary_group, @created, @created_by)";

                Guid idGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, "Global");
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, "Global Group");
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@primary_group", NpgsqlTypes.NpgsqlDbType.Bigint, 1);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, userId);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        return idGuid;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "group creation failed");

                return errorGuid;
            }
        }


        private Guid CreateUser(Models.OrganizationRequest value, Guid organization, Guid userId, out Guid securityTokenOut)
        {
            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO users (id, first_name, last_name, description, email_address, password, security_token, organization, primary_user, role, created, created_by) " +
                    "VALUES (@id, @first_name, @last_name, @description, @email_address, @password, @security_token, @organization, @primary_user, @role, @created, @created_by)";

                //Guid idGuid = Guid.NewGuid();
                Guid securityTokenGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, userId);
                        command.Parameters.AddWithValue("@first_name", NpgsqlTypes.NpgsqlDbType.Text, value.FirstName);
                        command.Parameters.AddWithValue("@last_name", NpgsqlTypes.NpgsqlDbType.Text, value.LastName);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.UserDescription);
                        command.Parameters.AddWithValue("@email_address", NpgsqlTypes.NpgsqlDbType.Text, value.UserEmailAddress);
                        command.Parameters.AddWithValue("@password", NpgsqlTypes.NpgsqlDbType.Text, value.UserPassword);
                        command.Parameters.AddWithValue("@security_token", NpgsqlTypes.NpgsqlDbType.Uuid, securityTokenGuid);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@primary_user", NpgsqlTypes.NpgsqlDbType.Bigint, 1);
                        command.Parameters.AddWithValue("@role", NpgsqlTypes.NpgsqlDbType.Bigint, 1);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, userId);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        securityTokenOut = securityTokenGuid;
                        return userId;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "user creation failed");
                securityTokenOut = errorGuid;

                return errorGuid;
            }
        }


        public Models.Response DeleteOrganization(Guid id)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM organizations WHERE id = @id";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Return Success
                        response.Status = "success";
                        response.Message = "organization deleted";
                        response.Id = id;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization deletion failed");

                response.Status = "error";
                response.Message = "organization deletion failed";
                response.Id = id;
                return response;
            }
        }

    }
}
