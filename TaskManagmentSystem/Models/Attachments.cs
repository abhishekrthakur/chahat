using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagmentSystem.Models
{
    public class Attachments
    {
        public int AttachmentId { get; set; }
        public int TaskId { get; set; }
        public string FilePath { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedUser { get; set; }
        public DateTime UploadDate { get; set; }

    }
}
