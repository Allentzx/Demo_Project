using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.Data.Entities
{
    public class FileTracking
    {
        [Key]
        public int Id { get; set; }
        public Guid? TraceId { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public string? FileUrl { get; set; }
        public string? Owner { get; set; }
        public DateTime DateUpLoad { get; set; }
    }
}
