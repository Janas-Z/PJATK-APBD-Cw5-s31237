using System;
using System.Collections.Generic;
using System.Linq;
using PJATK.Api.Models;

namespace PJATK.Api.Helpers;

public static class InMemoryDatabase
{
    public static List<Room> Rooms { get; } = new List<Room>();
    public static List<Reservation> Reservations { get; } = new List<Reservation>();

    public static void Initialize()
    {
        if (Rooms.Any()) return;

        Rooms.AddRange(new[]
        {
            new Room { Id = 1, Name = "Lab 101", BuildingCode = "A", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true },
            new Room { Id = 2, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true, IsActive = true },
            new Room { Id = 3, Name = "Conference 1", BuildingCode = "A", Floor = 3, Capacity = 50, HasProjector = true, IsActive = true },
            new Room { Id = 4, Name = "Small 5", BuildingCode = "C", Floor = 1, Capacity = 10, HasProjector = false, IsActive = false },
            new Room { Id = 5, Name = "Room 12", BuildingCode = "B", Floor = 1, Capacity = 20, HasProjector = false, IsActive = true }
        });

        Reservations.AddRange(new[]
        {
            new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Nowak", Topic = "Intro to C#", Date = DateTime.Today.AddDays(1), StartTime = TimeSpan.Parse("09:00:00"), EndTime = TimeSpan.Parse("11:00:00"), Status = ReservationStatus.Confirmed },
            new Reservation { Id = 2, RoomId = 2, OrganizerName = "Anna Kowalska", Topic = "REST workshop", Date = DateTime.Today.AddDays(2), StartTime = TimeSpan.Parse("10:00:00"), EndTime = TimeSpan.Parse("12:30:00"), Status = ReservationStatus.Confirmed },
            new Reservation { Id = 3, RoomId = 3, OrganizerName = "Piotr Zal", Topic = "Database", Date = DateTime.Today.AddDays(3), StartTime = TimeSpan.Parse("13:00:00"), EndTime = TimeSpan.Parse("15:00:00"), Status = ReservationStatus.Planned },
            new Reservation { Id = 4, RoomId = 1, OrganizerName = "Kasia", Topic = "Unit Testing", Date = DateTime.Today.AddDays(-1), StartTime = TimeSpan.Parse("14:00:00"), EndTime = TimeSpan.Parse("16:00:00"), Status = ReservationStatus.Cancelled },
            new Reservation { Id = 5, RoomId = 5, OrganizerName = "Adam", Topic = "Design Patterns", Date = DateTime.Today.AddDays(5), StartTime = TimeSpan.Parse("09:00:00"), EndTime = TimeSpan.Parse("11:00:00"), Status = ReservationStatus.Planned }
        });
    }
}
