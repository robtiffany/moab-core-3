using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class DigitalThreadEventService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.DigitalThreadEventController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public DigitalThreadEventService(IConfiguration config, ILogger<Controllers.DigitalThreadEventController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateDigitalThreadEvent(Models.DigitalThreadEventRequest value, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO digital_thread_events (id, name, value, digital_twin, organization, created, created_by) " +
                    "VALUES (@id, @name, @value, @digital_twin, @organization, @created, @created_by)";

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
                        command.Parameters.AddWithValue("@digital_twin", NpgsqlTypes.NpgsqlDbType.Uuid, value.DigitalTwin);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);
                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "Digital Thread Event created";
                        response.Id = idGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Thread Event creation failed");
                response.Status = "error";
                response.Message = "Digital Thread Event creation failed";
                response.Id = errorGuid;
                return response;
            }
        }


        public List<Models.DigitalThreadEvent> GetAllDigitalThreadEvents(Guid digitalTwin, Guid organization)
        {
            List<Models.DigitalThreadEvent> digitalThreadEventsList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_thread_events WHERE digital_twin = @digital_twin AND organization = @organization";

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
                                //Initialize a Digital Thread Event 
                                Models.DigitalThreadEvent digitalThreadEvent = null;
                                //Create a List to hold multiple Digital Thread Events
                                digitalThreadEventsList = new List<Models.DigitalThreadEvent>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    digitalThreadEvent = new Models.DigitalThreadEvent();

                                    digitalThreadEvent.Id = Guid.Parse(reader["id"].ToString());
                                    digitalThreadEvent.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalThreadEvent.Value = Convert.ToString(reader["value"]).Trim();
                                    digitalThreadEvent.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    digitalThreadEvent.Created = Convert.ToDateTime(reader["created"]);
                                    digitalThreadEvent.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    digitalThreadEventsList.Add(digitalThreadEvent);
                                }
                            }
                        }
                    }
                }
                return digitalThreadEventsList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital thread events");
                return digitalThreadEventsList;
            }
        }

        public Models.DigitalThreadEvent GetDigitalThreadEvent(Guid id, Guid digitalTwin, Guid organization)
        {
            Models.DigitalThreadEvent digitalThreadEvent = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM digital_thread_events WHERE id = @id AND digital_twin = @digital_twin AND organization = @organization";

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
                                    digitalThreadEvent = new Models.DigitalThreadEvent();

                                    digitalThreadEvent.Id = Guid.Parse(reader["id"].ToString());
                                    digitalThreadEvent.Name = Convert.ToString(reader["name"]).Trim();
                                    digitalThreadEvent.Value = Convert.ToString(reader["value"]).Trim();
                                    digitalThreadEvent.DigitalTwin = Guid.Parse(reader["digital_twin"].ToString());
                                    digitalThreadEvent.Created = Convert.ToDateTime(reader["created"]);
                                    digitalThreadEvent.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return digitalThreadEvent;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving digital thread event");
                return digitalThreadEvent;
            }
        }


        public Models.Response DeleteDigitalThreadEvent(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM digital_thread_events WHERE id = @id AND organization = @organization";

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
                            response.Message = "Digital Thread Event deleted";
                            response.Id = id;
                        }
                        else
                        {
                            //Return Failure
                            response.Status = "error";
                            response.Message = "Digital Thread Event deletion failed";
                            response.Id = id;
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "Digital Thread Event deletion failed");
                response.Status = "error";
                response.Message = "Digital Thread Event deletion failed";
                response.Id = id;
                return response;
            }
        }


    }
}