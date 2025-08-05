using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lulu_diary_backend.Models.Database;

[Table("FriendRequests")]
public class FriendRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("RequesterProfile")]
    public int RequesterProfileId { get; set; }

    [Required]
    [ForeignKey("RequestedProfile")]
    public int RequestedProfileId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "pending";

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}
