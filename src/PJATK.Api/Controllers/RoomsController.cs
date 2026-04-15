using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PJATK.Api.Models;
using PJATK.Api.Helpers;

namespace PJATK.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> Get([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var rooms = InMemoryDatabase.Rooms.AsEnumerable();
        if (minCapacity.HasValue) rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
        if (hasProjector.HasValue) rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);
        if (activeOnly.HasValue && activeOnly.Value) rooms = rooms.Where(r => r.IsActive);
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Room> Get(int id)
    {
        var room = InMemoryDatabase.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound();
        return Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetByBuilding(string buildingCode)
    {
        var rooms = InMemoryDatabase.Rooms.Where(r => string.Equals(r.BuildingCode, buildingCode, StringComparison.OrdinalIgnoreCase));
        return Ok(rooms);
    }

    [HttpPost]
    public ActionResult<Room> Create([FromBody] Room room)
    {
        var nextId = InMemoryDatabase.Rooms.Any() ? InMemoryDatabase.Rooms.Max(r => r.Id) + 1 : 1;
        room.Id = nextId;
        InMemoryDatabase.Rooms.Add(room);
        return CreatedAtAction(nameof(Get), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Room> Update(int id, [FromBody] Room room)
    {
        var existing = InMemoryDatabase.Rooms.FirstOrDefault(r => r.Id == id);
        if (existing == null) return NotFound();
        existing.Name = room.Name;
        existing.BuildingCode = room.BuildingCode;
        existing.Floor = room.Floor;
        existing.Capacity = room.Capacity;
        existing.HasProjector = room.HasProjector;
        existing.IsActive = room.IsActive;
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = InMemoryDatabase.Rooms.FirstOrDefault(r => r.Id == id);
        if (existing == null) return NotFound();
        var now = DateTime.Today;
        var hasFuture = InMemoryDatabase.Reservations.Any(res => res.RoomId == id && res.Date >= now);
        if (hasFuture) return Conflict(new { message = "Cannot delete room with future reservations." });
        InMemoryDatabase.Rooms.Remove(existing);
        return NoContent();
    }
}
