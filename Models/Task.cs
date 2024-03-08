using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Mangment_Api.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }=DateTime.Now;
        public bool IsFinished { get; set; }
        public ICollection<UserTask> AssignedUsers { get; set; } = new List<UserTask>();

      

    }
}
