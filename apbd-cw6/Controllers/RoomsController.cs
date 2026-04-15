
using Microsoft.AspNetCore.Mvc;
using apbd_cw6.Data;
using apbd_cw6.Models;

namespace apbd_cw6.Controllers;


[ApiController]
[Route("api/[controller]")]

public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetRooms(
        [FromQuery] int? minCapicity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly
    )
    {
        var query = DataStore.Rooms.AsEnumerable();

        if (minCapicity.HasValue)
            query = query.Where(r => r.Capacity >= minCapicity.Value); 
        
        if (hasProjector.HasValue)
            query = query.Where(r => r.HasProjector == hasProjector.Value);
        
        if (activeOnly.HasValue && activeOnly.Value)
            query = query.Where(r => r.IsActive);
        
        return Ok(query);
    }
    
    
    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetRoomsByBuilding(string buildingCode)
    {
        var rooms = DataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase));
        
        return Ok(rooms);
    }
    
    
    [HttpPost]
    public ActionResult<Room> CreateRoom([FromBody] Room newRoom)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        newRoom.Id = DataStore.GetNextRoomId();
        
        DataStore.Rooms.Add(newRoom);
        
        return CreatedAtAction(nameof(GetRoom), new { id = newRoom.Id }, newRoom);
    }
    
    
    [HttpGet("{id}")]
    public ActionResult<Room> GetRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
    
        if (room == null)
            return NotFound($"Room with ID {id} not found");
    
        return Ok(room);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var existingRoom = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        
        if (existingRoom == null)
            return NotFound($"Room with id {id} not found");
        
        existingRoom.Name = updatedRoom.Name;
        existingRoom.BuildingCode = updatedRoom.BuildingCode;
        existingRoom.Floor = updatedRoom.Floor;
        existingRoom.Capacity = updatedRoom.Capacity;
        existingRoom.HasProjector = updatedRoom.HasProjector;
        existingRoom.IsActive = updatedRoom.IsActive;
        
        return Ok(existingRoom);
    }
    
    
    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        
        if (room == null)
            return NotFound($"Room with id {id} not found");
        
        var futureReservations = DataStore.Reservations
            .Where(r => r.RoomId == id && r.Date >= DateTime.Today);
        
        if (futureReservations.Any())
        {
            return Conflict($"Cannot delete room with id {id} because it has future reservations");
        }
        
        DataStore.Rooms.Remove(room);
        
        return NoContent();
    }

}