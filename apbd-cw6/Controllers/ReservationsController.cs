using Microsoft.AspNetCore.Mvc;
using apbd_cw6.Data;
using apbd_cw6.Models;

namespace apbd_cw6.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> GetReservations(
        [FromQuery] DateTime? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var query = DataStore.Reservations.AsEnumerable();

        if (date.HasValue)
        {
            query = query.Where(r => r.Date.Date == date.Value.Date);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (roomId.HasValue)
        {
            query = query.Where(r => r.RoomId == roomId.Value);
        }

        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<Reservation> GetReservationById(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound($"Reservation with ID {id} not found");
        }

        return Ok(reservation);
    }

    [HttpPost]
    public ActionResult<Reservation> CreateReservation([FromBody] Reservation newReservation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == newReservation.RoomId);
        if (room == null)
        {
            return BadRequest($"Room with ID {newReservation.RoomId} does not exist");
        }

        if (!room.IsActive)
        {
            return BadRequest($"Room '{room.Name}' is not active. Cannot create reservation.");
        }

        bool hasConflict = CheckTimeConflict(newReservation);
        if (hasConflict)
        {
            return Conflict("Time slot overlaps with an existing reservation for this room");
        }

        newReservation.Id = DataStore.GetNextReservationId();
        DataStore.Reservations.Add(newReservation);

        return CreatedAtAction(nameof(GetReservationById), new { id = newReservation.Id }, newReservation);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingReservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (existingReservation == null)
        {
            return NotFound($"Reservation with ID {id} not found");
        }

        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null)
        {
            return BadRequest($"Room with ID {updatedReservation.RoomId} does not exist");
        }

        if (!room.IsActive)
        {
            return BadRequest($"Room '{room.Name}' is not active. Cannot update reservation.");
        }

        bool hasConflict = CheckTimeConflict(updatedReservation, excludeId: id);
        if (hasConflict)
        {
            return Conflict("Time slot overlaps with an existing reservation for this room");
        }

        existingReservation.RoomId = updatedReservation.RoomId;
        existingReservation.OrganizerName = updatedReservation.OrganizerName;
        existingReservation.Topic = updatedReservation.Topic;
        existingReservation.Date = updatedReservation.Date;
        existingReservation.StartTime = updatedReservation.StartTime;
        existingReservation.EndTime = updatedReservation.EndTime;
        existingReservation.Status = updatedReservation.Status;

        return Ok(existingReservation);
    }

    
    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound($"Reservation with ID {id} not found");
        }

        DataStore.Reservations.Remove(reservation);

        return NoContent();
    }

    private bool CheckTimeConflict(Reservation newReservation, int? excludeId = null)
    {
        return DataStore.Reservations.Any(existing =>
            (excludeId == null || existing.Id != excludeId) &&
            existing.RoomId == newReservation.RoomId &&
            existing.Date.Date == newReservation.Date.Date &&
            (
                (newReservation.StartTime >= existing.StartTime && newReservation.StartTime < existing.EndTime) ||
                (newReservation.EndTime > existing.StartTime && newReservation.EndTime <= existing.EndTime) ||
                (newReservation.StartTime <= existing.StartTime && newReservation.EndTime >= existing.EndTime)
            )
        );
    }
}