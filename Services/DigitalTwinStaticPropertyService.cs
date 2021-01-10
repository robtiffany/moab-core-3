using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class DigitalTwinStaticPropertyService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.DigitalTwinStaticPropertyController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public DigitalTwinStaticPropertyService(IConfiguration config, ILogger<Controllers.DigitalTwinStaticPropertyController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateDigitalTwinStaticProperty(Models.DigitalTwinStaticPropertyRequest value, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO digital_twin_static_properties (id, name, value, digital_twin, organization, created, created_by) " +
                    "VALUES (@id, @name, @value, @digital_twin, @organization, @created, @created_by)";

                //Create a new Id UUID
                Guid idGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@name", value.Name);
                        command.Parameters.AddWithValue("@value", value.Value);
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, value.DigitalTwin);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "Digital Twin Static Property created";
                        response.Id = idGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Twin Static Property creation failed");
                response.Status = "error";
                response.Message = "Digital Twin Static Property creation failed";
                response.Id = errorGuid;
                return response;
            }
        }


        public List<Models.DigitalTwinStaticProperty> GetAllDigitalTwinStaticProperties(Guid digitalTwin, Guid organization)
        {
            List<Models.DigitalTwinStaticProperty> digitalTwinStaticPropertyList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_static_properties WHERE digital_twin = @digital_twin AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwin);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                //Initialize a Digital Twin Static Property 
                                Models.DigitalTwinStaticProperty twinStaticProperty = null;
                                //Create a List to hold multiple Digital Twin Static Properties
                                digitalTwinStaticPropertyList = new List<Models.DigitalTwinStaticProperty>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    twinStaticProperty = new Models.DigitalTwinStaticProperty();

                                    twinStaticProperty.Id = Guid.Parse(reader["id"].ToString());
                                    twinStaticProperty.Name = Convert.ToString(reader["name"]).Trim();
                                    twinStaticProperty.Value = Convert.ToString(reader["value"]).Trim();
                                    twinStaticProperty.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    twinStaticProperty.Created = Convert.ToDateTime(reader["created"]);
                                    twinStaticProperty.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    digitalTwinStaticPropertyList.Add(twinStaticProperty);
                                }
                            }
                        }
                    }
                }
                return digitalTwinStaticPropertyList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving Digital Twin Static Property");
                return digitalTwinStaticPropertyList;
            }
        }

        public Models.DigitalTwinStaticProperty GetDigitalTwinStaticProperty(Guid id, Guid digitalTwin, Guid organization)
        {
            Models.DigitalTwinStaticProperty twinStaticProperty = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_static_properties WHERE id = @id AND digital_twin = @digital_twin AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwin);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    twinStaticProperty = new Models.DigitalTwinStaticProperty();

                                    twinStaticProperty.Id = Guid.Parse(reader["id"].ToString());
                                    twinStaticProperty.Name = Convert.ToString(reader["name"]).Trim();
                                    twinStaticProperty.Value = Convert.ToString(reader["value"]).Trim();
                                    twinStaticProperty.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    twinStaticProperty.Created = Convert.ToDateTime(reader["created"]);
                                    twinStaticProperty.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return twinStaticProperty;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving Digital Twin Static Property");
                return twinStaticProperty;
            }
        }


        public Models.Response DeleteDigitalTwinStaticProperty(Guid id, Guid digitalTwin, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM digital_twin_static_properties WHERE id = @id AND digital_twin = @digital_twin AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwin);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();
                        int rows = command.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            //Return Success
                            response.Status = "success";
                            response.Message = "Digital Twin Static Property deleted";
                            response.Id = id;
                        }
                        else
                        {
                            //Return Failure
                            response.Status = "error";
                            response.Message = "Digital Twin Static Property deletion failed";
                            response.Id = id;
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Twin Static Property deletion failed");
                response.Status = "error";
                response.Message = "Digital Twin Static Property deletion failed";
                response.Id = id;
                return response;
            }
        }


    }
}