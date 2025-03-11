using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkinScan_API.Common;
using SkinScan_BL.Contracts;
using SkinScan_Core.Contexts;
using SkinScan_Core.Entites;
using System.Security.Claims;

namespace SkinScan_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotQuestionController : ControllerBase
    {
        private readonly IGenericRepository<ChatbotQuestion> _chatbotQuestionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatbotQuestionController(
            UserManager<ApplicationUser> userManager,
            IGenericRepository<ChatbotQuestion> chatbotQuestionRepository)
        {
            _userManager = userManager;
            _chatbotQuestionRepository = chatbotQuestionRepository;
        }


        [HttpGet("start")]
        public async Task<IActionResult> StartChat()
        {
            
            var firstQuestionList = await _chatbotQuestionRepository.GetByConditionAsync(q => q.ParentId == null);
            var firstQuestion = firstQuestionList.FirstOrDefault();

            if (firstQuestion == null)
                return BadRequest(ResponseModel<string>.ErrorResponse("No questions available", 404));

            
            var answers = await _chatbotQuestionRepository.GetByConditionAsync(q => q.ParentId == firstQuestion.Id);

            var response = new
            {
                QuestionText = firstQuestion.QuestionText,
                Answers = answers.Select(a => new
                {
                    a.Id,
                    AnswerText = a.AnswerText ?? "No text available",
                    a.BotResponse,
                    NextQuestionId = a.NextQuestionId ?? -1
                })
            };

            return Ok(ResponseModel<object>.SuccessResponse(response, "Chat started successfully", 200));
        }

        [HttpGet("next/{answerId}")]
        public async Task<IActionResult> GetNextQuestion(int answerId)
        {
            var selectedAnswerList = await _chatbotQuestionRepository.GetByConditionAsync(q => q.Id == answerId);
            var selectedAnswer = selectedAnswerList.FirstOrDefault();

            if (selectedAnswer == null)
                return NotFound(ResponseModel<string>.ErrorResponse("Answer not found.", 404));

            if (!string.IsNullOrEmpty(selectedAnswer.BotResponse) && selectedAnswer.NextQuestionId == null)
            {
                return Ok(ResponseModel<object>.SuccessResponse(new { BotResponse = selectedAnswer.BotResponse }, "End of conversation.", 200));
            }

            // Fetch the next question
            var nextQuestionList = await _chatbotQuestionRepository.GetByConditionAsync(q => q.Id == selectedAnswer.NextQuestionId);
            var nextQuestion = nextQuestionList.FirstOrDefault();

            if (nextQuestion == null)
                return Ok(ResponseModel<string>.SuccessResponse("End of conversation. Thank you!", "Conversation ended.", 200));
            var answers = await _chatbotQuestionRepository.GetByConditionAsync(q => q.ParentId == nextQuestion.Id);

            var response = new
            {
                QuestionText = nextQuestion.QuestionText,
                Answers = answers.Select(a => new
                {
                    a.Id,
                    a.AnswerText,
                    a.BotResponse,
                    a.NextQuestionId
                }),
                SelectedAnswer = new { selectedAnswer.Id, selectedAnswer.BotResponse }
            };

            return Ok(ResponseModel<object>.SuccessResponse(response, "Next question retrieved successfully.", 200));
        }





    }
}
