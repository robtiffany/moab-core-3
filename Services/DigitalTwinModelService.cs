using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class DigitalTwinModelService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.DigitalTwinModelController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public DigitalTwinModelService(IConfiguration config, ILogger<Controllers.DigitalTwinModelController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateDigitalTwinModel(Models.DigitalTwinModelRequest value, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO digital_twin_models (id, name, description, version, organization, created, created_by) " +
                    "VALUES (@id, @name, @description, @version, @organization, @created, @created_by)";

                //Create a new Model Id UUID
                Guid modelIdGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, modelIdGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.Name);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.Description);
                        command.Parameters.AddWithValue("@version", NpgsqlTypes.NpgsqlDbType.Bigint, value.Version);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "digital twin model created";
                        response.Id = modelIdGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "digital twin model creation failed");
                response.Status = "error";
                response.Message = "digital twin model creation failed";
                response.Id = errorGuid;
                return response;
            }
        }


        public List<Models.DigitalTwinModel> GetAllDigitalTwinModels(Guid organization)
        {
            List<Models.DigitalTwinModel> digitalTwinModelList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_models WHERE organization = @organization";

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
                                //Initialize a Digital Twin Model 
                                Models.DigitalTwinModel digitalTwinModel = null;
                                //Create a List to hold multiple Digital Twin Models
                                digitalTwinModelList = new List<Models.DigitalTwinModel>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    digitalTwinModel = new Models.DigitalTwinModel();

                                    digitalTwinModel.Id = Guid.Parse(reader["id"].ToString());
                                    digitalTwinModel.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalTwinModel.Description = Convert.ToString(reader["description"]).Trim();
                                    digitalTwinModel.Version = Convert.ToInt64(reader["version"]);
                                    digitalTwinModel.Created = Convert.ToDateTime(reader["created"]);
                                    digitalTwinModel.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    digitalTwinModelList.Add(digitalTwinModel);
                                }
                            }
                        }
                    }
                }
                return digitalTwinModelList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital twin models");
                return digitalTwinModelList;
            }
        }


        public Models.DigitalTwinModel GetDigitalTwinModel(Guid id, Guid organization)
        {
            Models.DigitalTwinModel digitalTwinModel = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_twin_models WHERE id = @id AND organization = @organization";

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
                                    digitalTwinModel = new Models.DigitalTwinModel();

                                    digitalTwinModel.Id = Guid.Parse(reader["id"].ToString());
                                    digitalTwinModel.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalTwinModel.Description = Convert.ToString(reader["description"]).Trim();
                                    digitalTwinModel.Version = Convert.ToInt64(reader["version"]);
                                    digitalTwinModel.Created = Convert.ToDateTime(reader["created"]);
                                    digitalTwinModel.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return digitalTwinModel;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital twin model");
                return digitalTwinModel;
            }
        }






        public Models.Response DeleteDigitalTwinModel(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM digital_twin_models WHERE id = @id AND organization = @organization";

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
                        response.Message = "digital twin model deleted";
                        response.Id = id;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "digital twin model deletion failed");
                response.Status = "error";
                response.Message = "digital twin model deletion failed";
                response.Id = id;
                return response;
            }
        }



    }
}
