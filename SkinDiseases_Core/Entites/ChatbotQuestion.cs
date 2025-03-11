using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_Core.Entites
{
    public class ChatbotQuestion
    {
        [Key]
        public int Id { get; set; }

        public string? QuestionText { get; set; }

        public string? AnswerText { get; set; }

        public int? ParentId { get; set; }

        public int? NextQuestionId { get; set; }

        public string? BotResponse { get; set; }

        [ForeignKey("NextQuestionId")]
        public virtual ChatbotQuestion? NextQuestion { get; set; }

        [ForeignKey("ParentId")]
        public virtual ChatbotQuestion? Parent { get; set; }

        public virtual ICollection<ChatbotQuestion> InverseNextQuestion { get; set; } = new List<ChatbotQuestion>();

        public virtual ICollection<ChatbotQuestion> InverseParent { get; set; } = new List<ChatbotQuestion>();
    }
}
