﻿namespace NucpaBalloonsApi.Models.DTOs.Admin;

public class LoginRequestDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
