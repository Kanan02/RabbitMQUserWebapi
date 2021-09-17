using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using UserAuthentification.Dtos;
using UserAuthentification.Models;
using UserAuthentification.RabbitClient;

namespace UserAuthentification.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQClient _rabbitMQClient;
        public UsersController(IConfiguration configuration, IRabbitMQClient rabbitMQClient)
        {
            _configuration = configuration;
            _rabbitMQClient = rabbitMQClient;
      
        }

        [HttpGet("GetAll")]
        public List<User> GetAll()
        {
            
            return LoadListFromDB();
            
        }
        [HttpPost]
        public ActionResult CreateUser(UserDto user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("UserAuthentificationConnection");
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"INSERT INTO public.users( username, password)VALUES( '{user.Username}', '{user.Password}'); ";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }

            var message = new { Name = "Producer", Message = "User is Created!",User=user };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _rabbitMQClient.Publish("", "main-queue", body);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteActivity(int id)
        {
            List<User> users = LoadListFromDB();
            User u = users.Find(x => x.Id == id);
            if (u==null)
            {
                return NotFound();
            }
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("UserAuthentificationConnection");
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = $"DELETE FROM public.users where username='{u.Username}' and password='{u.Password}'";
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }

            var message = new { Name = "Producer", Message = "User is Deleted!", User = u };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _rabbitMQClient.Publish("", "main-queue", body);
            return Ok();
        }
        private List<User> LoadListFromDB()
       {
            List<User> users = new List<User>();
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = _configuration.GetConnectionString("UserAuthentificationConnection");
                connection.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "select * from getAllUsers()";
                cmd.CommandType = CommandType.Text;
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmd.Dispose();
                connection.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    User user = new User();
                    user.Id = Int32.Parse(dt.Rows[i]["id"].ToString());
                    user.Username = dt.Rows[i]["username"].ToString();
                    user.Password = dt.Rows[i]["password"].ToString();
                    users.Add(user);
                }
            }
            return users;
        }
    
    }
}
