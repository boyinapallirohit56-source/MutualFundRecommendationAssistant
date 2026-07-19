namespace MutualFundAPI.Models.DTOs;

public class RiskQuestionDTO
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public List<RiskOptionDTO> Options { get; set; } = new();
}

public class RiskOptionDTO
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
}

public class SubmitAssessmentDTO
{
    public List<AnswerDTO> Answers { get; set; } = new();
}

public class AnswerDTO
{
    public int QuestionId { get; set; }
    public int SelectedOptionId { get; set; }
}

public class RiskAssessmentResultDTO
{
    public int AssessmentId { get; set; }
    public int TotalScore { get; set; }
    public int NormalizedScore { get; set; } // 0-100
    public string RiskProfile { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}
