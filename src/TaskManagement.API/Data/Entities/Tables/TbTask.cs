using TaskManagement.API.Enums;

namespace TaskManagement.API.Data.Entities.Tables
{
    public class TbTask
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; }
        public required TaskStatusEn Status { get; set; }
        public required bool Pending { get; set; }
        public required bool InProgress { get; set; }
        public required bool Completed { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required int Priority { get; set; }
    }
}
