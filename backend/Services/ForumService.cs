﻿using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Pokemon_Forum_API.DTO.ForumDTO;
using Pokemon_Forum_API.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using utils;

namespace Pokemon_Forum_API.Services
{
    public class ForumService
    {
        string connectionString = Utils.ConnectionString;

        UserService userService = new UserService();
        public ForumService() { }

        /// <summary>
        /// Method to get all forums from DB
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public async Task<List<Forums>> GetAllForums(string connString)
        {

            List<Forums> forums = new List<Forums>();

            using (MySqlConnection conn = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM forums", conn))
            {
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        int forum_id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string description = reader.GetString(2);
                        var forum = new Forums(forum_id, name, description);
                        //forum.subforums = 
                        //forum.threads =
                        forums.Add(forum);
                    }
                }

            }
            return forums;
        }

        /// <summary>
        /// Method to get one forum by his ID from DB
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public async Task<Forums> GetForumById(string connString, int _id)
        {

            using (MySqlConnection conn = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM forums where forum_id=@forum_id", conn))
            {
                await conn.OpenAsync();
                cmd.Parameters.AddWithValue("@forum_id", _id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        int forum_id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string description = reader.GetString(2);
                        var forum = new Forums(forum_id, name, description);
                        //forum.subforums = 
                        //forum.threads =
                        return forum;
                    }
                }

            }

            return new Forums();
        }

        /// <summary>
        /// Method to create a forum
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="forum"></param>
        /// <returns></returns>
        public async Task<Forums> CreateForum(string connString, ForumDto forum)
        {
            try
            {

                string sqlQuery = "INSERT INTO Forums (name, description) " +
                                                   "VALUES (@name, @description);";

                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                    {

                        cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = forum.name;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = forum.description;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return new Forums(forum.name, forum.description);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return new Forums();
            }
        }

        /// <summary>
        /// Method to update a forum by his ID
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="id"></param>
        /// <param name="forum"></param>
        /// <returns></returns>
        public async Task<Forums> UpdateForum(string connString, int id, ForumDto forum)
        {
            var tempForum = await GetForumById(connectionString, id);
            if (tempForum != null)
            {
                try
                {
                    string sqlQuery = "UPDATE forums SET name = @name," +
                                                      " description =  @description," +
                                                      " WHERE forum_id = @forum_id;";
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                    {
                        await conn.OpenAsync();
                        cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = forum.name;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = forum.description;
                        cmd.Parameters.Add("@forum_id", MySqlDbType.Int32).Value = id;
                        await cmd.ExecuteNonQueryAsync();
                        return new Forums(forum.name, forum.description);

                    }
                }
                catch (Exception ex)
                {
                    return new Forums();
                }
            }
            else
            {
                return new Forums();
            }

        }

        /// <summary>
        /// Method to delete a forum by his ID
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Forums> DeleteForum(string connString, int id)
        {

            var tempForum = await GetForumById(connectionString, id);
            if (tempForum != null)
            {
                try
                {
                    string sqlQuery = "DELETE FROM forums WHERE forum_id = @forum_id;";
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, conn))
                    {

                        await conn.OpenAsync();
                        cmd.Parameters.AddWithValue("@forum_id", id);

                        await cmd.ExecuteNonQueryAsync();
                        return tempForum;

                    }
                }
                catch (Exception ex)
                {
                    return new Forums();
                }
            }
            else
            {
                return new Forums();
            }
        }
    }
}