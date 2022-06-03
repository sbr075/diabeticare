using System;

namespace Diabeticare.Models
{
    public class GroupModel
    {
        // Group model for 'View Data' display, used to gather group data to display to user
        public DateTime GroupDate { get; set; }
        public float GroupAvg { get; set; }
        public TimeSpan GroupDuration { get; set; }
    }
}
