using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lulu_diary_backend.Models.Database;

[Table("Friends")]
public class Friend
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("ProfileA")]
    public int ProfileAId { get; set; }

    [Required]
    [ForeignKey("ProfileB")]
    public int ProfileBId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
