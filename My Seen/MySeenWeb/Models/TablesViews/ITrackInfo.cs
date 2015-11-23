using System.Collections.Generic;

namespace MySeenWeb.Models.TablesViews
{
    public class ITrackInfo
    {
        public List<Location> Path { get; set; }
        public Location Min { get; set; }
        public Location Center { get; set; }
        public Location Max { get; set; }

        public void CallcMinMaxCenter()
        {
            double minX = Path[0].Latitude;
            double maxX = Path[0].Latitude;

            double minY = Path[0].Longitude;
            double maxY = Path[0].Longitude;

            foreach (Location l in Path)
            {
                if (minX > l.Latitude) minX = l.Latitude;
                if (maxX < l.Latitude) maxX = l.Latitude;
                if (maxY < l.Longitude) maxY = l.Longitude;
                if (minY > l.Longitude) minY = l.Longitude;
            }
            Max = new Location(maxX, maxY);
            Min = new Location(minX, minY);
            Center = new Location((maxX + minX) / 2, (maxY + minY) / 2);
        }
    }
}