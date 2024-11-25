using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class valve
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public string ValveName { get; set; }
    }
}
