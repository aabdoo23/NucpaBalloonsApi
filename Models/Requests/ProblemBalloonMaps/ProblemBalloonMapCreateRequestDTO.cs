using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Models.Requests.ProblemBalloonMaps
{
    public class ProblemBalloonMapCreateRequestDTO
    {
        public string ProblemIndex { get; set; }
        public string BalloonColor { get; set; }
    }
}