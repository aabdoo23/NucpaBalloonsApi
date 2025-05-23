﻿using NucpaBalloonsApi.Models.Codeforces;
using NucpaBalloonsApi.Models.DTOs;
using NucpaBalloonsApi.Models.SystemModels;

namespace NucpaBalloonsApi.Interfaces.Services
{
    public interface IBalloonService
    {
        Task<BalloonRequest> CreateBalloonRequestAsync(string teamName, string problemSolved, string balloonColor);
        Task<BalloonRequest?> UpdateBalloonStatusAsync(string id, BalloonStatus status, string? deliveredBy = null);
        Task<List<BalloonRequestDTO>> GetPendingBalloonsAsync();
        Task<List<BalloonRequestDTO>> GetReadyForPickupBalloonsAsync();
        Task<List<BalloonRequestDTO>> GetPickedUpBalloonsAsync();
        Task<List<BalloonRequestDTO>> GetDeliveredBalloonsAsync();
        Task ProcessNewSubmissions(List<Submission> submissions);
        Task<List<BalloonRequestDTO>> GetFirstSolve();
    }
} 