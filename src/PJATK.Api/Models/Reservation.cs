using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PJATK.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReservationStatus
{
    Planned,
    Confirmed,
    Cancelled
}

public class Reservation : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public string OrganizerName { get; set; } = string.Empty;

    [Required]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReservationStatus Status { get; set; } = ReservationStatus.Planned;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult("EndTime must be later than StartTime", new[] { nameof(EndTime), nameof(StartTime) });
        }
    }
}
