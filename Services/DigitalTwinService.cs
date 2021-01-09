using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class DigitalTwinService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.DigitalTwinController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public DigitalTwinService(IConfiguration config, ILogger<Controllers.DigitalTwinController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.DigitalTwinResponse CreateDigitalTwin(Models.DigitalTwinRequest value, Guid organization)
        {
            Models.DigitalTwinResponse response = new Models.DigitalTwinResponse();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO digital_twins (id, name, description, security_token, digital_twin_model, organization, enabled, group_, created, created_by) " +
                    "VALUES (@id, @name, @description, @security_token, @digital_twin_model, @organization, @enabled, @group_, @created, @created_by)";

                //Create a new Id UUID
                Guid idGuid = Guid.NewGuid();

                //Create a new Security Token UUID
                Guid securityTokenGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.Name);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.Description);
                        command.Parameters.AddWithValue("@security_token", NpgsqlTypes.NpgsqlDbType.Uuid, securityTokenGuid);
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, value.DigitalTwinModel);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@enabled", NpgsqlTypes.NpgsqlDbType.Bigint, value.Enabled);
                        command.Parameters.AddWithValue("@group_", NpgsqlTypes.NpgsqlDbType.Uuid, value.Group);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "digital twin created";
                        response.Id = idGuid;
                        response.SecurityToken = securityTokenGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "digital twin creation failed");
                response.Status = "error";
                response.Message = "digital twin creation failed";
                response.Id = errorGuid;
                response.SecurityToken = errorGuid;
                return response;
            }
        }


        public List<Models.DigitalTwin> GetAllDigitalTwins(Guid organization)
        {
            List<Models.DigitalTwin> digitalTwinList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twins WHERE organization = @organization";

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
                                //Initialize a Digital Twin 
                                Models.DigitalTwin digitalTwin = null;
                                //Create a List to hold multiple Digital Twins
                                digitalTwinList = new List<Models.DigitalTwin>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    digitalTwin = new Models.DigitalTwin();

                                    digitalTwin.Id = Guid.Parse(reader["id"].ToString());
                                    digitalTwin.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalTwin.Description = Convert.ToString(reader["description"]).Trim();
                                    digitalTwin.SecurityToken = Guid.Parse(reader["security_token"].ToString());
                                    digitalTwin.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    digitalTwin.Enabled = Convert.ToInt64(reader["enabled"]);
                                    digitalTwin.Group = Guid.Parse(reader["group_"].ToString());
                                    digitalTwin.Created = Convert.ToDateTime(reader["created"]);
                                    digitalTwin.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    digitalTwinList.Add(digitalTwin);
                                }
                            }
                        }
                    }
                }
                return digitalTwinList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital twins");
                return digitalTwinList;
            }
        }


        public Models.DigitalTwin GetDigitalTwin(Guid id, Guid organization)
        {
            Models.DigitalTwin digitalTwin = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twins WHERE id = @id AND organization = @organization";

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
                                    digitalTwin = new Models.DigitalTwin();

                                    digitalTwin.Id = Guid.Parse(reader["id"].ToString());
                                    digitalTwin.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalTwin.Description = Convert.ToString(reader["description"]).Trim();
                                    digitalTwin.SecurityToken = Guid.Parse(reader["security_token"].ToString());
                                    digitalTwin.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    digitalTwin.Enabled = Convert.ToInt64(reader["enabled"]);
                                    digitalTwin.Group = Guid.Parse(reader["group_"].ToString());
                                    digitalTwin.Created = Convert.ToDateTime(reader["created"]);
                                    digitalTwin.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return digitalTwin;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital twin");
                return digitalTwin;
            }
        }






        public Models.Response DeleteDigitalTwin(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM digital_twins WHERE id = @id AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Return Success
                        response.Status = "success";
                        response.Message = "digital twin deleted";
                        response.Id = id;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "digital twin deletion failed");
                response.Status = "error";
                response.Message = "digital twin deletion failed";
                response.Id = id;
                return response;
            }
        }



    }
}
