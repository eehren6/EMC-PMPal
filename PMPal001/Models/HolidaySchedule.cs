using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMPal.Models
{
    class HolidaySchedule : Schedule
    {
        public string ID { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
