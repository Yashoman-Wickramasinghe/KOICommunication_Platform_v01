using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KOICommunicationPlatform.Models.ViewModels
{
    public class TutorialViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Day")]
        public string? Day { get; set; }

        [Required]
        [Display(Name = "From Time")]
        public string? FromTime { get; set; }

        [Required]
        [Display(Name = "To Time")]
        public string? ToTime { get; set; }

        public string? CourseName { get; set; }
        public string? SubjectName { get; set; }

        [Required]
        [Display(Name = "Trimester")]
        public string? Trimester { get; set; }

        public IEnumerable<SelectListItem>? TrimesterList { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public IEnumerable<SelectListItem>? CourseList { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }
        public IEnumerable<SelectListItem>? SubjectList { get; set; }

        public IEnumerable<SelectListItem>? DayList { get; set; }

        public IEnumerable<SelectListItem>? FromTimeList { get; set; }

        public IEnumerable<SelectListItem>? ToTimeList { get; set; }

    }
}
