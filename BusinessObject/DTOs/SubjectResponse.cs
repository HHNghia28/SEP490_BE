using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SubjectResponse
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Grade { get; set; }

        public List<ComponentScoreResponse> ComponentScores { get; set; }
        public List<LessonPlanResponse> LessonPlans { get; set; }
    }
}
