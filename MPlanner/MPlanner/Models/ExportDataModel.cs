using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MPlanner.Models
{
    [NotMapped]
    public class ExportDataModel
    {
        [DisplayName("Monday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? MondayStartTime { get; set; }
        [DisplayName("Tuesday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? TuesdayStartTime { get; set; }
        [DisplayName("Wednesday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? WednesdayStartTime { get; set; }
        [DisplayName("Thursday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? ThursdayStartTime { get; set; }
        [DisplayName("Friday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? FridayStartTime { get; set; }
        [DisplayName("Saturday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? SaturdayStartTime { get; set; }
        [DisplayName("Sunday start time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? SundayStartTime { get; set; }

        [DisplayName("Monday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? MondayEndTime { get; set; }
        [DisplayName("Tuesday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? TuesdayEndTime { get; set; }
        [DisplayName("Wednesday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? WednesdayEndTime { get; set; }
        [DisplayName("Thursday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? ThursdayEndTime { get; set; }
        [DisplayName("Friday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? FridayEndTime { get; set; }
        [DisplayName("Saturday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? SaturdayEndTime { get; set; }
        [DisplayName("Sunday end time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm")]
        public DateTime? SundayEndTime { get; set; }

        public bool IsMondayValid { get { return MondayStartTime.HasValue && MondayEndTime.HasValue && MondayStartTime.Value < MondayEndTime.Value; } }
        public bool IsTuesdaValid { get { return TuesdayStartTime.HasValue && TuesdayEndTime.HasValue && TuesdayStartTime.Value < TuesdayEndTime.Value; } }
        public bool IsWednesdayValid { get { return WednesdayStartTime.HasValue && WednesdayEndTime.HasValue && WednesdayStartTime.Value < WednesdayEndTime.Value; } }
        public bool IsThursdayValid { get { return ThursdayStartTime.HasValue && ThursdayEndTime.HasValue && ThursdayStartTime.Value < ThursdayEndTime.Value; } }
        public bool IsFridayValid { get { return FridayStartTime.HasValue && FridayEndTime.HasValue && FridayStartTime.Value < FridayEndTime.Value; } }
        public bool IsSaturdayValid { get { return SaturdayStartTime.HasValue && SaturdayEndTime.HasValue && SaturdayStartTime.Value < SaturdayEndTime.Value; } }
        public bool IsSundayValid { get { return SundayStartTime.HasValue && SundayEndTime.HasValue && SundayStartTime.Value < SundayEndTime.Value; } }

        public int MondayAmount { get { return IsMondayValid ? (int)(MondayEndTime.Value.TimeOfDay - MondayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int TuesdayAmount { get { return IsTuesdaValid ? (int)(TuesdayEndTime.Value.TimeOfDay - TuesdayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int WednesdayAmount { get { return IsWednesdayValid ? (int)(WednesdayEndTime.Value.TimeOfDay - WednesdayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int ThursdayAmount { get { return IsThursdayValid ? (int)(ThursdayEndTime.Value.TimeOfDay - ThursdayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int FridayAmount { get { return IsFridayValid ? (int)(FridayEndTime.Value.TimeOfDay - FridayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int SaturdayAmount { get { return IsSaturdayValid ? (int)(SaturdayEndTime.Value.TimeOfDay - SaturdayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
        public int SundayAmount { get { return IsSundayValid ? (int)(SundayEndTime.Value.TimeOfDay - SundayStartTime.Value.TimeOfDay).TotalMinutes : 0; } }
    }
}
