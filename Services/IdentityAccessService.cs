using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class IdentityAccessService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.UserController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public IdentityAccessService(IConfiguration config, ILogger<Controllers.UserController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public bool IsEntityAuthorized(Microsoft.AspNetCore.Http.HttpContext context, out Guid organizationOut, out Guid identityOut, out Guid digitalTwinModelOut)
        {
            organizationOut = errorGuid;
            identityOut = errorGuid;
            digitalTwinModelOut = errorGuid;

            try
            {
                //Get contents of Authorization Header and stip out Bearer
                var credentials = context.Request.Headers["Authorization"].First().Substring(7);

                //Test for presence of Identity and Security Token
                if (credentials.Any())
                {
                    var credentialArray = credentials.Split('.');
                    Guid identity = Guid.Parse(credentialArray[0]);
                    Guid securityToken = Guid.Parse(credentialArray[1]);

                    //Validate Identity and Security Token
                    if (ValidateEntityCredentials(identity, securityToken))
                    {
                        //Make Idenity Available to Controller Action Method
                        identityOut = identity;

                        //Make Organization Available to Controller Action Method
                        organizationOut = GetEntityOrganization(identity, securityToken);

                        //Make Digital Twin Model Available to Controller Action Method
                        digitalTwinModelOut = GetDigitalTwinModel(identity);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }



        private bool ValidateEntityCredentials(Guid id, Guid securityToken)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT COUNT(*) FROM entity_registry WHERE id = @id AND security_token = @securitytoken";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@securitytoken", NpgsqlTypes.NpgsqlDbType.Uuid, securityToken);
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
                _logger.LogError(ex, "credential validation failed");
                return false;
            }
        }


        private Guid GetEntityOrganization(Guid id, Guid securityToken)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT organization FROM entity_registry WHERE id = @id AND security_token = @securitytoken";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@securitytoken", NpgsqlTypes.NpgsqlDbType.Uuid, securityToken);
                        command.Prepare();
                        return Guid.Parse(command.ExecuteScalar().ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization retrieval failed");
                return errorGuid;
            }
        }


        private Guid GetDigitalTwinModel(Guid id)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT digital_twin_model FROM entity_registry WHERE id = @id";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Prepare();
                        return Guid.Parse(command.ExecuteScalar().ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "model retrieval failed");
                return errorGuid;
            }
        }




        public bool IsUserAuthorized(Microsoft.AspNetCore.Http.HttpContext context, out Guid organizationOut, out Guid identityOut, out long roleOut)
        {
            organizationOut = errorGuid;
            identityOut = errorGuid;
            roleOut = 0;

            try
            {
                //Get contents of Authorization Header and stip out Bearer
                var credentials = context.Request.Headers["Authorization"].First().Substring(7);

                //Test for presence of Identity and Security Token
                if (credentials.Any())
                {
                    //var credentialArray = credentials.Split(".");
                    var credentialArray = credentials.Split('.');

                    Guid identity = Guid.Parse(credentialArray[0]);
                    Guid securityToken = Guid.Parse(credentialArray[1]);

                    //Validate Identity and Security Token
                    if (ValidateUserCredentials(identity, securityToken))
                    {
                        //Make Idenity Available to Controller Action Method
                        identityOut = identity;

                        //Make Organization Available to Controller Action Method
                        organizationOut = GetUserOrganization(identity, securityToken);

                        //Make Role available to Controller Action Method
                        roleOut = GetUserRole(identity, securityToken);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        private bool ValidateUserCredentials(Guid id, Guid securityToken)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT COUNT(*) FROM users WHERE id = @id AND security_token = @securitytoken";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@securitytoken", NpgsqlTypes.NpgsqlDbType.Uuid, securityToken);
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
                _logger.LogError(ex, "credential validation failed");
                return false;
            }
        }


        private Guid GetUserOrganization(Guid id, Guid securityToken)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT organization FROM users WHERE id = @id AND security_token = @securitytoken";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@securitytoken", NpgsqlTypes.NpgsqlDbType.Uuid, securityToken);
                        command.Prepare();
                        return Guid.Parse(command.ExecuteScalar().ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization retrieval failed");
                return errorGuid;
            }
        }


        private long GetUserRole(Guid id, Guid securityToken)
        {
            try
            {
                //SQL Statement
                var sqlString = "SELECT role FROM users WHERE id = @id AND security_token = @securitytoken";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@securitytoken", NpgsqlTypes.NpgsqlDbType.Uuid, securityToken);
                        command.Prepare();
                        return Convert.ToInt64(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "organization retrieval failed");
                return 0;
            }
        }

    }
}
