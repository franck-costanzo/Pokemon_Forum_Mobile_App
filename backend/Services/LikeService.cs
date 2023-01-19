﻿using Microsoft.Data.SqlClient;
using Pokemon_Forum_API.DTO.LikeDTO;
using Pokemon_Forum_API.Entities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Pokemon_Forum_API.Services
{
    public class LikeService
    {
        string connectionString = Tools.Tools.connectionString;

        UserService userService = new UserService();
        PostService postService = new PostService();
        public LikeService() {}

        /// <summary>
        /// Method to get one like by his ID from DB
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public async Task<Likes> GetLikeById(string connString, int _id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM likes where like_id=@like_id", conn))
                {
                    await conn.OpenAsync();
                    cmd.Parameters.AddWithValue("@like_id", _id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            int like_id = reader.GetInt32(0);
                            int post_id = reader.GetInt32(1);
                            int user_id = reader.GetInt32(2);
                            var like = new Likes(like_id, post_id, user_id);
                            like.user = await userService.GetUserById(connString, user_id);
                            like.post = await postService.GetPostById(connString, post_id);

                            return like;
                        }
                    }

                }

                

            }
            catch(Exception ex)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// Method to create a like
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="like"></param>
        /// <returns></returns>
        public async Task<Likes> CreateLike(string connString, LikeDto like)
        {
            try
            {

                string sqlQuery = "INSERT INTO likes (post_id, user_id) " +
                                                   "VALUES (@post_id, @user_id);";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {

                        cmd.Parameters.Add("@post_id", SqlDbType.Int).Value = like.post_id;
                        cmd.Parameters.Add("@user_id", SqlDbType.Int).Value = like.user_id;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return new Likes(like.post_id, like.user_id);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return null;
            }
        }


        /// <summary>
        /// Method to delete a like by his ID
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Likes> DeleteLike(string connString, int id)
        {

            var tempLike = await GetLikeById(connectionString, id);
            if (tempLike != null)
            {
                try
                {
                    string sqlQuery = "DELETE FROM likes WHERE like_id = @like_id;";
                    using (SqlConnection conn = new SqlConnection(connString))
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {

                        await conn.OpenAsync();
                        cmd.Parameters.AddWithValue("@like_id", id);

                        await cmd.ExecuteNonQueryAsync();
                        return tempLike;

                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


    }

}