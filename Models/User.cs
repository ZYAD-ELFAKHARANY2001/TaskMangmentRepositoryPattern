using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Mangment_Api.Models
{
    public class User:IdentityUser
    {
        
        public string Name { get; set; }
        public byte[] ProfilePicture { get; set; }
        public ICollection<UserTask> UserTasks { get; set; }
        public ICollection<UserTask> UserTasksCreated { get; set; }


    }
}
