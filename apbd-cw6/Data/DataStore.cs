using apbd_cw6.Models;

namespace apbd_cw6.Data;

public class DataStore
{
     public static List<Room> Rooms { get; private set; } = new();
    public static List<Reservation> Reservations { get; private set; } = new();
    private static int _nextRoomId = 1;
    private static int _nextReservationId = 1;

    public static void Initialize()
    {
        Rooms = new List<Room>
        {
            new Room { Id = _nextRoomId++, Name = "Aula Magna", BuildingCode = "A", Floor = 1, Capacity = 100, HasProjector = true, IsActive = true },
            new Room { Id = _nextRoomId++, Name = "Lab 101", BuildingCode = "B", Floor = 1, Capacity = 20, HasProjector = true, IsActive = true },
            new Room { Id = _nextRoomId++, Name = "Conference B2", BuildingCode = "B", Floor = 2, Capacity = 30, HasProjector = false, IsActive = true },
            new Room { Id = _nextRoomId++, Name = "Seminar Room 3", BuildingCode = "C", Floor = 0, Capacity = 15, HasProjector = false, IsActive = false },
            new Room { Id = _nextRoomId++, Name = "Workshop Hall", BuildingCode = "A", Floor = 2, Capacity = 50, HasProjector = true, IsActive = true }
        };

        Reservations = new List<Reservation>
        {
            new Reservation 
            { 
                Id = _nextReservationId++, 
                RoomId = 1, 
                OrganizerName = "Jan Kowalski", 
                Topic = "Opening Ceremony", 
                Date = new DateTime(2026, 5, 10), 
                StartTime = TimeSpan.Parse("09:00"), 
                EndTime = TimeSpan.Parse("11:00"), 
                Status = "confirmed" 
            },
            new Reservation 
            { 
                Id = _nextReservationId++, 
                RoomId = 2, 
                OrganizerName = "Anna Nowak", 
                Topic = "C# Workshop", 
                Date = new DateTime(2026, 5, 10), 
                StartTime = TimeSpan.Parse("10:00"), 
                EndTime = TimeSpan.Parse("13:00"), 
                Status = "planned" 
            },
            new Reservation 
            { 
                Id = _nextReservationId++, 
                RoomId = 1, 
                OrganizerName = "Piotr Wiśniewski", 
                Topic = "Lunch Break", 
                Date = new DateTime(2026, 5, 10), 
                StartTime = TimeSpan.Parse("12:00"), 
                EndTime = TimeSpan.Parse("13:30"), 
                Status = "confirmed" 
            },
            new Reservation 
            { 
                Id = _nextReservationId++, 
                RoomId = 3, 
                OrganizerName = "Ewa Zielińska", 
                Topic = "Design Thinking", 
                Date = new DateTime(2026, 5, 11), 
                StartTime = TimeSpan.Parse("14:00"), 
                EndTime = TimeSpan.Parse("17:00"), 
                Status = "confirmed" 
            },
            new Reservation 
            { 
                Id = _nextReservationId++, 
                RoomId = 2, 
                OrganizerName = "Tomasz Lewandowski", 
                Topic = "Code Review", 
                Date = new DateTime(2026, 5, 12), 
                StartTime = TimeSpan.Parse("09:30"), 
                EndTime = TimeSpan.Parse("11:00"), 
                Status = "cancelled" 
            }
        };
    }

    public static int GetNextRoomId()
    {
        return ++_nextRoomId;
    }

    public static int GetNextReservationId()
    {
        return ++_nextReservationId;
    }
}