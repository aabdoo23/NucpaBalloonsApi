﻿namespace NucpaBalloonsApi.Models.Requests.AdminSettings
{
    public class AdminSettingsUpdateRequestDTO
    {
        public string Id { get; set; }
        public string? AdminUsername { get; set; }
        public int? ContestId { get; set; }
        public string? CodeforcesApiKey { get; set; }
        public string? CodeforcesApiSecret { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
