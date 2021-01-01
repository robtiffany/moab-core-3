using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoabCore3.Services
{
    public class GroupService
    {
        //Connection String
        private readonly string connectionString;

        private readonly ILogger<Controllers.GroupController> _logger;
        Guid errorGuid = new Guid("00000000000000000000000000000000");

        public GroupService(IConfiguration config, ILogger<Controllers.GroupController> logger)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public Models.Response CreateGroup(Models.GroupRequest value, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "INSERT INTO groups (id, name, description, organization, primary_group, created, created_by) " +
                    "VALUES (@id, @name, @description, @organization, @primary_group, @created, @created_by)";

                //Create a new Group Id UUID
                Guid groupIdGuid = Guid.NewGuid();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(sqlString, connection))
                    {
                        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Uuid, groupIdGuid);
                        command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Text, value.Name);
                        command.Parameters.AddWithValue("@description", NpgsqlTypes.NpgsqlDbType.Text, value.Description);
                        command.Parameters.AddWithValue("@organization", NpgsqlTypes.NpgsqlDbType.Uuid, organization);
                        command.Parameters.AddWithValue("@primary_group", NpgsqlTypes.NpgsqlDbType.Bigint, 0);
                        command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.TimestampTz, DateTime.UtcNow);
                        command.Parameters.AddWithValue("@created_by", NpgsqlTypes.NpgsqlDbType.Uuid, value.CreatedBy);

                        command.Prepare();
                        command.ExecuteNonQuery();

                        //Log Success
                        response.Status = "success";
                        response.Message = "group created";
                        response.Id = groupIdGuid;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "group creation failed");
                response.Status = "error";
                response.Message = "group creation failed";
                response.Id = errorGuid;
                return response;
            } 
        }


        public List<Models.Group> GetAllGroups(Guid organization)
        {
            List<Models.Group> GroupList = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM groups WHERE organization = @organization";

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
                                //Initialize a Group 
                                Models.Group group = null;
                                //Create a List to hold multiple Groups
                                GroupList = new List<Models.Group>();

                                while (reader.Read())
                                {
                                    //Create and hydrate a new Object
                                    group = new Models.Group();

                                    group.Id = Guid.Parse(reader["id"].ToString());
                                    group.Name = Convert.ToString(reader["name"]).Trim();
                                    group.Description = Convert.ToString(reader["description"]).Trim();
                                    group.Created = Convert.ToDateTime(reader["created"]);
                                    group.CreatedBy = Guid.Parse(reader["created_by"].ToString());

                                    //Add to List
                                    GroupList.Add(group);
                                }
                            }
                        }
                    }
                }
                return GroupList;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving groups");
                return GroupList;
            }
        }

        public Models.Group GetGroup(Guid id, Guid organization)
        {
            Models.Group group = null;

            try
            {
                //SQL Statement
                var sqlString = "SELECT * FROM groups WHERE id = @id AND organization = @organization";

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
                                    group = new Models.Group();

                                    group.Id = Guid.Parse(reader["id"].ToString());
                                    group.Name = Convert.ToString(reader["name"]).Trim();
                                    group.Description = Convert.ToString(reader["description"]).Trim();
                                    group.Created = Convert.ToDateTime(reader["created"]);
                                    group.CreatedBy = Guid.Parse(reader["created_by"].ToString());
                                }
                            }
                        }
                    }
                }
                return group;
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "error retrieving group");
                return group;
            }
        }


        public Models.Response DeleteGroup(Guid id, Guid organization)
        {
            Models.Response response = new Models.Response();

            try
            {
                //SQL Statement
                var sqlString = "DELETE FROM groups WHERE id = @id AND organization = @organization AND primary_group <> 1";

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
                            response.Message = "group deleted";
                            response.Id = id;
                        }
                        else
                        {
                            //Return Failure
                            response.Status = "error";
                            response.Message = "group deletion failed";
                            response.Id = id;
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log Exception
                _logger.LogError(ex, "group deletion failed");
                response.Status = "error";
                response.Message = "group deletion failed";
                response.Id = id;
                return response;
            }
        }





    }
}
