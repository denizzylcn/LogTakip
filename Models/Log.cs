using System.ComponentModel.DataAnnotations;

namespace LogTakipAPI.Models
{
    public class Log
    {
        [Key] // Bu satır primary key olduğunu belirtir
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Tarih { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Durum { get; set; } = string.Empty;
    }
}
