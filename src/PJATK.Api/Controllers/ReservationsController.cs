using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PJATK.Api.Models;
using PJATK.Api.Helpers;

namespace PJATK.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> Get([FromQuery] DateTime? date, [FromQuery] ReservationStatus? status, [FromQuery] int? roomId)
    {
        var res = InMemoryDatabase.Reservations.AsEnumerable();
        if (date.HasValue) res = res.Where(r => r.Date.Date == date.Value.Date);
        if (status.HasValue) res = res.Where(r => r.Status == status.Value);
        if (roomId.HasValue) res = res.Where(r => r.RoomId == roomId.Value);
        return Ok(res);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reservation> Get(int id)
    {
        var r = InMemoryDatabase.Reservations.FirstOrDefault(x => x.Id == id);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public ActionResult<Reservation> Create([FromBody] Reservation reservation)
    {
        var room = InMemoryDatabase.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null) return NotFound(new { message = "Room not found." });
        if (!room.IsActive) return Conflict(new { message = "Cannot create reservation for inactive room." });

        if (HasConflictingReservation(reservation, null)) return Conflict(new { message = "Reservation conflicts with existing reservation." });

        reservation.Id = InMemoryDatabase.Reservations.Any() ? InMemoryDatabase.Reservations.Max(r => r.Id) + 1 : 1;
        InMemoryDatabase.Reservations.Add(reservation);
        return CreatedAtAction(nameof(Get), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Reservation> Update(int id, [FromBody] Reservation reservation)
    {
        var existing = InMemoryDatabase.Reservations.FirstOrDefault(r => r.Id == id);
        if (existing == null) return NotFound();
        var room = InMemoryDatabase.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null) return NotFound(new { message = "Room not found." });
        if (!room.IsActive) return Conflict(new { message = "Cannot create reservation for inactive room." });
        if (HasConflictingReservation(reservation, id)) return Conflict(new { message = "Reservation conflicts with existing reservation." });

        existing.RoomId = reservation.RoomId;
        existing.OrganizerName = reservation.OrganizerName;
        existing.Topic = reservation.Topic;
        existing.Date = reservation.Date;
        existing.StartTime = reservation.StartTime;
        existing.EndTime = reservation.EndTime;
        existing.Status = reservation.Status;
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = InMemoryDatabase.Reservations.FirstOrDefault(r => r.Id == id);
        if (existing == null) return NotFound();
        InMemoryDatabase.Reservations.Remove(existing);
        return NoContent();
    }

    private bool HasConflictingReservation(Reservation reservation, int? ignoreReservationId)
    {
        return InMemoryDatabase.Reservations.Any(r =>
            r.RoomId == reservation.RoomId
            && r.Date.Date == reservation.Date.Date
            && (!ignoreReservationId.HasValue || r.Id != ignoreReservationId.Value)
            && reservation.StartTime < r.EndTime
            && r.StartTime < reservation.EndTime);
    }
}
