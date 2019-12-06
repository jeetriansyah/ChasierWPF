using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDBootcamp32.Model
{
    [Table("tb_m_user")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTimeOffset RegisterDate { get; set; }

        public Role Roles { get; set; }

        public User() {}

        public User(string name, string email, string password, Role role)
        {
            this.Username = name;
            this.Email = email;
            this.Password = password;
            this.Roles = role;
            this.RegisterDate = DateTimeOffset.Now.LocalDateTime;
        }
    }
}
