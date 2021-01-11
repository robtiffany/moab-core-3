using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace MoabCore3.Services
{
    public class TelemetryService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.TelemetryController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public TelemetryService(IConfiguration config, ILogger<Controllers.TelemetryController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateTelemetry(JsonElement digitalTwinData, Guid digitalTwin, Guid organization, Guid digitalTwinModel)
        {
            Models.Response response = new Models.Response();

            var dtData = digitalTwinData.ToString();

            try
            {
                //SQL Statement
                string sqlString = "INSERT INTO telemetry (id, digital_twin, digital_twin_data, digital_twin_model, created, organization) " +
                    "VALUES (@id, @digital_twin, @digital_twin_data, @digital_twin_model, @created, @organization)";

                //Create a new Id UUID
                Guid idGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, idGuid);
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwin);
                        command.Parameters.AddWithValue("@digital_twin_data", NpgsqlTypes.NpgsqlDbType.Text, dtData);
                        command.Parameters.AddWithValue("@digital_twin_model", NpgsqlTypes.NpgsqlDbType.Uuid, digitalTwinModel);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "telemetry saved";
                        response.Id = idGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "telemetry save failed");
                response.Status = "error";
                response.Message = "telemetry save failed";
                response.Id = errorGuid;
                return response;
            }
        }

        public List<Models.Telemetry> GetAllTelemetry(Guid organization)
        {
            List<Models.Telemetry> telemetryList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM telemetry WHERE organization = @organization";

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
                                //Initialize Telemetry Object
                                Models.Telemetry telemetry = null;
                                //Create a List to hold multiple Telemetry Objects
                                telemetryList = new List<Models.Telemetry>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    telemetry = new Models.Telemetry();

                                    telemetry.Id = Guid.Parse(reader["id"].ToString());
                                    telemetry.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    telemetry.DigitalTwinData = Convert.ToString(reader["digital_twin_data"]).Trim();
                                    telemetry.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    telemetry.Created = Convert.ToDateTime(reader["created"]);

                                    //Add to List
                                    telemetryList.Add(telemetry);
                                }
                            }
                        }
                    }
                }
                return telemetryList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving telemetry");
                return telemetryList;
            }
        }




        public List<Models.Telemetry> GetTelemetry(Guid id, Guid organization)
        {
            List<Models.Telemetry> telemetryList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM telemetry WHERE id = @id AND organization = @organization";

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
                                //Initialize Telemetry Object
                                Models.Telemetry telemetry = null;
                                //Create a List to hold multiple Telemetry Objects
                                telemetryList = new List<Models.Telemetry>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    telemetry = new Models.Telemetry();

                                    telemetry.Id = Guid.Parse(reader["id"].ToString());
                                    telemetry.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    telemetry.DigitalTwinData = Convert.ToString(reader["digital_twin_data"]).Trim();
                                    telemetry.DigitalTwinModel = Guid.Parse(reader["digital_twin_model"].ToString());
                                    telemetry.Created = Convert.ToDateTime(reader["created"]);

                                    //Add to List
                                    telemetryList.Add(telemetry);
                                }
                            }
                        }
                    }
                }
                return telemetryList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving telemetry");
                return telemetryList;
            }
        }





        public Models.Response DeleteTelemetry(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM telemetry WHERE id = @id AND organization = @organization";

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
                        response.Message = "telemetry deleted";
                        response.Id = id;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "telemetry deletion failed");
                response.Status = "error";
                response.Message = "telemetry deletion failed";
                response.Id = id;
                return response;
            }
        }




    }
}