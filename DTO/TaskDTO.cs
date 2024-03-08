using Task_Mangment_Api.Models;

namespace Task_Mangment_Api.DTO
{
    public class TaskDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsFinished { get; set; }

        public string? UserID { get; set; }
        public string? UserName { get; set; }



    }
}
