using System.ComponentModel.DataAnnotations;

namespace OKNODOM.DTOs
{
    public class AssignBrigadeModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Выберите хотя бы одного монтажника")]
        public List<int> BrigadierIds { get; set; } = new();
    }
}
                                