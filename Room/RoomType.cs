using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer.Room
{
    public enum RoomType
    {
        Safe, // Safe zone
        Normal, // Regular room
        Boss, // Boss room
        Event, // Items or a key inside the room
        None // Wall or an empty space
    }
}
