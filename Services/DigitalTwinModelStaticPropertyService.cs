using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class DigitalTwinModelStaticPropertyService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.DigitalTwinModelStaticPropertyController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public DigitalTwinModelStaticPropertyService(IConfiguration config, ILogger<Controllers.DigitalTwinModelStaticPropertyController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateDigitalTwinModelStaticProperty(Models.DigitalTwinModelStaticPropertyRequest value, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO digital_twin_model_static_properties (id, name, value, digital_twin_model, organization, created, created_by) " +
                    "VALUES (@id, @name, @value, @digital_twin_model, @organization, @created, @created_by)";

                //Create a new Id UUID
                Guid idGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.Name);
                        command.Parameters.AddWithValue("@value", NpgsqlTypes.NpgsqlDbType.Text, value.Value);
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, value.DigitalTwinModel);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "Digital Twin Model Static Property created";
                        response.Id = idGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Twin Model Static Property creation failed");
                response.Status = "error";
                response.Message = "Digital Twin Model Static Property creation failed";
                response.Id = errorGuid;
                return response;
            }
        }


        public List<Models.DigitalTwinModelStaticProperty> GetAllDigitalTwinModelStaticProperties(Guid digitalTwinModel, Guid organization)
        {
            List<Models.DigitalTwinModelStaticProperty> digitalTwinModelStaticPropertyList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_model_static_properties WHERE digital_twin_model = @digital_twin_model AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwinModel);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                //Initialize a Digital Twin Model Static Property 
                                Models.DigitalTwinModelStaticProperty twinModelStaticProperty = null;
                                //Create a List to hold multiple Digital Twin Model Static Properties
                                digitalTwinModelStaticPropertyList = new List<Models.DigitalTwinModelStaticProperty>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    twinModelStaticProperty = new Models.DigitalTwinModelStaticProperty();

                                    twinModelStaticProperty.Id = Guid.Parse(reader["id"].ToString());
                                    twinModelStaticProperty.Name = Convert.ToString(reader["name"]).Trim();
                                    twinModelStaticProperty.Value = Convert.ToString(reader["value"]).Trim();
                                    twinModelStaticProperty.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    twinModelStaticProperty.Created = Convert.ToDateTime(reader["created"]);
                                    twinModelStaticProperty.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    digitalTwinModelStaticPropertyList.Add(twinModelStaticProperty);
                                }
                            }
                        }
                    }
                }
                return digitalTwinModelStaticPropertyList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving Digital Twin Model Static Property");
                return digitalTwinModelStaticPropertyList;
            }
        }

        public Models.DigitalTwinModelStaticProperty GetDigitalTwinModelStaticProperty(Guid id, Guid digitalTwinModel, Guid organization)
        {
            Models.DigitalTwinModelStaticProperty twinModelStaticProperty = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_model_static_properties WHERE id = @id AND digital_twin_model = @digital_twin_model AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwinModel);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null && reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    twinModelStaticProperty = new Models.DigitalTwinModelStaticProperty();

                                    twinModelStaticProperty.Id = Guid.Parse(reader["id"].ToString());
                                    twinModelStaticProperty.Name = Convert.ToString(reader["name"]).Trim();
                                    twinModelStaticProperty.Value = Convert.ToString(reader["value"]).Trim();
                                    twinModelStaticProperty.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    twinModelStaticProperty.Created = Convert.ToDateTime(reader["created"]);
                                    twinModelStaticProperty.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return twinModelStaticProperty;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving Digital Twin Model Static Property");
                return twinModelStaticProperty;
            }
        }


        public Models.Response DeleteDigitalTwinModelStaticProperty(Guid id, Guid digitalTwinModel, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM digital_twin_model_static_properties WHERE id = @id AND digital_twin_model = @digital_twin_model AND organization = @organization";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, id);
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwinModel);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();
                        int rows = command.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            //Return Success
                            response.Status = "success";
                            response.Message = "Digital Twin Model Static Property deleted";
                            response.Id = id;
                        }
                        else
                        {
                            //Return Failure
                            response.Status = "error";
                            response.Message = "Digital Twin Model Static Property deletion failed";
                            response.Id = id;
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Twin Model Static Property deletion failed");
                response.Status = "error";
                response.Message = "Digital Twin Model Static Property deletion failed";
                response.Id = id;
                return response;
            }
        }


    }
}