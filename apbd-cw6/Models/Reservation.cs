using System.ComponentModel.DataAnnotations;

namespace apbd_cw6.Models;

public class Reservation
{
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required(ErrorMessage ="Organizer's name is required")]
    public string OrganizerName { get; set; }
    
    [Required(ErrorMessage ="Topic is required")]
    public string Topic { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }

    public string Status { get; set; } = "planned";

    public static ValidationResult? ValidateTimeRange(TimeSpan end, ValidationContext ctx)
    {
        var instance = (Reservation)ctx.ObjectInstance;

        if (instance.EndTime <= instance.StartTime)
            return new ValidationResult("endTime must be later than startTime");
        return ValidationResult.Success;
    }
}